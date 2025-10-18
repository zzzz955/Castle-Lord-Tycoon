# Security Guidelines

## Overview

Castle Lord Tycoon 프로젝트의 보안 정책 및 구현 가이드입니다.

**Security Principles**:
1. **Defense in Depth**: 다층 보안 구조
2. **Server Authority**: 서버가 모든 게임 로직 검증
3. **Zero Trust**: 모든 클라이언트 입력 검증
4. **Least Privilege**: 최소 권한 원칙

---

## 1. Anti-Cheat Strategy

### 1.1 Server-Side Validation

**모든 게임 로직은 서버에서 검증**

```csharp
// ❌ BAD: 클라이언트가 자원 차감
public class BuildingRequest {
    public int NewGoldAmount { get; set; } // 클라이언트가 계산
}

// ✅ GOOD: 서버가 자원 차감 계산
public class BuildingRequest {
    public Guid BuildingTypeId { get; set; }
    public Position Position { get; set; }
}

// 서버 로직
public async Task<Result> BuildBuilding(Guid userId, BuildingRequest request) {
    var cost = await _buildingService.GetCost(request.BuildingTypeId);
    var userResources = await _resourceService.GetResources(userId);

    // 서버에서 검증
    if (!userResources.CanAfford(cost)) {
        return Error(INSUFFICIENT_RESOURCES);
    }

    // 서버에서 차감
    await _resourceService.DeductResources(userId, cost);
    await _buildingService.CreateBuilding(userId, request);

    return Success();
}
```

---

### 1.2 Time-Based Validation

**시간 기반 작업은 서버 타임스탬프 사용**

```csharp
public class ConstructionService {
    public async Task<Result> CompleteConstruction(Guid buildingId) {
        var building = await _db.Buildings.FindAsync(buildingId);

        // 서버 시간으로 검증
        if (building.ConstructionEndTime > DateTime.UtcNow) {
            return Error(CONSTRUCTION_NOT_FINISHED, new {
                RemainingTime = (building.ConstructionEndTime - DateTime.UtcNow).TotalSeconds
            });
        }

        building.IsUnderConstruction = false;
        building.CompletedAt = DateTime.UtcNow; // 서버 타임스탬프

        await _db.SaveChangesAsync();
        return Success();
    }
}
```

---

### 1.3 Resource Production Verification

**자원 생산량 계산은 서버에서만**

```csharp
public class ResourceCollectionService {
    public async Task<ResourceCollection> CollectResources(Guid buildingId) {
        var building = await _db.Buildings
            .Include(b => b.BuildingType)
            .FirstOrDefaultAsync(b => b.Id == buildingId);

        if (building == null) {
            throw new NotFoundException();
        }

        // 서버에서 생산량 계산
        var timeSinceLastCollection = DateTime.UtcNow - building.LastCollectionTime;
        var maxProductionTime = TimeSpan.FromHours(8); // 최대 8시간 누적

        var effectiveTime = timeSinceLastCollection > maxProductionTime
            ? maxProductionTime
            : timeSinceLastCollection;

        var productionRate = building.BuildingType.BaseProductionRate
            * (1 + building.Level * 0.1); // 레벨당 10% 증가

        var producedAmount = (int)(productionRate * effectiveTime.TotalSeconds);

        // 이상치 검증
        var maxReasonableAmount = productionRate * maxProductionTime.TotalSeconds * 1.1;
        if (producedAmount > maxReasonableAmount) {
            await LogSuspiciousActivity(building.UserId, "EXCESSIVE_PRODUCTION");
            producedAmount = (int)maxReasonableAmount;
        }

        building.LastCollectionTime = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return new ResourceCollection {
            ResourceType = building.BuildingType.ResourceType,
            Amount = producedAmount
        };
    }
}
```

---

### 1.4 Action Rate Limiting

**비정상적인 요청 속도 감지**

```csharp
public class ActionRateLimiter {
    private readonly IDistributedCache _cache;
    private readonly ILogger<ActionRateLimiter> _logger;

    public async Task<bool> IsAllowed(Guid userId, string actionType, int maxActions, TimeSpan window) {
        var key = $"ratelimit:{userId}:{actionType}";
        var countStr = await _cache.GetStringAsync(key);

        int currentCount = string.IsNullOrEmpty(countStr) ? 0 : int.Parse(countStr);

        if (currentCount >= maxActions) {
            _logger.LogWarning("Rate limit exceeded: UserId={UserId}, Action={Action}, Count={Count}",
                userId, actionType, currentCount);
            return false;
        }

        await _cache.SetStringAsync(key, (currentCount + 1).ToString(),
            new DistributedCacheEntryOptions {
                AbsoluteExpirationRelativeToNow = window
            });

        return true;
    }
}

// 사용 예시
[HttpPost("collect")]
public async Task<IActionResult> CollectResources(Guid buildingId) {
    var userId = User.GetUserId();

    // 1분에 최대 20회
    if (!await _rateLimiter.IsAllowed(userId, "COLLECT_RESOURCES", 20, TimeSpan.FromMinutes(1))) {
        return StatusCode(429, Error("TOO_MANY_REQUESTS"));
    }

    var result = await _resourceService.CollectResources(buildingId);
    return Ok(result);
}
```

---

### 1.5 Data Integrity Checks

**데이터 무결성 검증**

```csharp
public class IntegrityValidator {
    public async Task<ValidationResult> ValidateUserData(Guid userId) {
        var errors = new List<string>();

        var user = await _db.Users.Include(u => u.Resources).FirstAsync(u => u.Id == userId);

        // 자원 합리성 검증
        if (user.Resources.Gold < 0 || user.Resources.Gold > 999_999_999) {
            errors.Add($"Invalid gold amount: {user.Resources.Gold}");
        }

        // 레벨-경험치 일치 검증
        var expectedLevel = CalculateLevelFromExperience(user.Experience);
        if (user.Level != expectedLevel) {
            errors.Add($"Level mismatch: Level={user.Level}, Expected={expectedLevel}");
        }

        // 건물 수 제한 검증
        var buildingCount = await _db.Buildings.CountAsync(b => b.UserId == userId);
        var maxBuildings = GetMaxBuildingsForLevel(user.Level);
        if (buildingCount > maxBuildings) {
            errors.Add($"Too many buildings: {buildingCount} > {maxBuildings}");
        }

        if (errors.Any()) {
            await LogIntegrityViolation(userId, errors);
        }

        return new ValidationResult {
            IsValid = !errors.Any(),
            Errors = errors
        };
    }
}
```

---

### 1.6 Cheat Detection Patterns

```yaml
cheat_detection_rules:
  # 1. 비정상적인 자원 증가
  resource_spike:
    trigger: "자원 증가량 > (생산율 * 시간 * 1.5)"
    action: "로그 기록, 관리자 알림"

  # 2. 불가능한 시간 진행
  time_manipulation:
    trigger: "건설 완료 시간 < 현재 시간"
    action: "요청 거부, 로그 기록"

  # 3. 중복 요청
  duplicate_requests:
    trigger: "동일 요청 5초 내 3회 이상"
    action: "일시 차단 (5분)"

  # 4. 순간이동
  teleportation:
    trigger: "위치 변화 > 최대 이동 속도 * 시간"
    action: "위치 롤백, 로그 기록"

  # 5. 불법 아이템 소유
  illegal_items:
    trigger: "아이템 ID가 DB에 없음"
    action: "아이템 제거, 계정 플래그"
```

---

## 2. Authentication & Authorization

### 2.1 JWT Configuration

```csharp
// Startup.cs
public void ConfigureServices(IServiceCollection services) {
    var jwtSettings = Configuration.GetSection("JWT").Get<JwtSettings>();

    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options => {
            options.TokenValidationParameters = new TokenValidationParameters {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                ClockSkew = TimeSpan.Zero // 시간 오차 허용 안 함
            };

            // SignalR WebSocket 지원
            options.Events = new JwtBearerEvents {
                OnMessageReceived = context => {
                    var accessToken = context.Request.Query["access_token"];
                    var path = context.HttpContext.Request.Path;

                    if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs")) {
                        context.Token = accessToken;
                    }

                    return Task.CompletedTask;
                }
            };
        });
}
```

---

### 2.2 Password Security

```csharp
public class PasswordHasher {
    private const int SaltSize = 16; // 128 bits
    private const int HashSize = 32; // 256 bits
    private const int Iterations = 100_000; // OWASP 권장

    public static string HashPassword(string password) {
        using var rng = RandomNumberGenerator.Create();
        var salt = new byte[SaltSize];
        rng.GetBytes(salt);

        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
        var hash = pbkdf2.GetBytes(HashSize);

        var hashBytes = new byte[SaltSize + HashSize];
        Array.Copy(salt, 0, hashBytes, 0, SaltSize);
        Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

        return Convert.ToBase64String(hashBytes);
    }

    public static bool VerifyPassword(string password, string hashedPassword) {
        var hashBytes = Convert.FromBase64String(hashedPassword);

        var salt = new byte[SaltSize];
        Array.Copy(hashBytes, 0, salt, 0, SaltSize);

        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
        var hash = pbkdf2.GetBytes(HashSize);

        for (int i = 0; i < HashSize; i++) {
            if (hashBytes[i + SaltSize] != hash[i]) {
                return false;
            }
        }

        return true;
    }
}
```

**비밀번호 정책**:
```yaml
password_requirements:
  min_length: 8
  max_length: 128
  require_uppercase: true
  require_lowercase: true
  require_digit: true
  require_special_character: true
  disallow_common_passwords: true
  disallow_username_in_password: true
```

---

### 2.3 Refresh Token Strategy

```csharp
public class RefreshTokenService {
    public async Task<RefreshToken> GenerateRefreshToken(Guid userId) {
        var refreshToken = new RefreshToken {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = GenerateSecureToken(),
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            IsRevoked = false
        };

        await _db.RefreshTokens.AddAsync(refreshToken);
        await _db.SaveChangesAsync();

        return refreshToken;
    }

    public async Task<bool> ValidateRefreshToken(string token) {
        var refreshToken = await _db.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == token);

        if (refreshToken == null || refreshToken.IsRevoked || refreshToken.ExpiresAt < DateTime.UtcNow) {
            return false;
        }

        return true;
    }

    public async Task RevokeRefreshToken(string token) {
        var refreshToken = await _db.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);
        if (refreshToken != null) {
            refreshToken.IsRevoked = true;
            refreshToken.RevokedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }
    }

    private string GenerateSecureToken() {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }
}
```

---

### 2.4 Authorization Policies

```csharp
public static class Policies {
    public const string RequireVerifiedAccount = "RequireVerifiedAccount";
    public const string RequireAdminRole = "RequireAdminRole";
    public const string RequirePremiumUser = "RequirePremiumUser";
}

// Startup.cs
services.AddAuthorization(options => {
    options.AddPolicy(Policies.RequireVerifiedAccount, policy =>
        policy.RequireClaim("EmailVerified", "true"));

    options.AddPolicy(Policies.RequireAdminRole, policy =>
        policy.RequireRole("Admin"));

    options.AddPolicy(Policies.RequirePremiumUser, policy =>
        policy.RequireClaim("SubscriptionTier", "Premium", "VIP"));
});

// 사용 예시
[Authorize(Policy = Policies.RequireVerifiedAccount)]
[HttpPost("premium-feature")]
public async Task<IActionResult> UsePremiumFeature() {
    // ...
}
```

---

## 3. Input Validation

### 3.1 Request DTO Validation

```csharp
public class BuildingCreateRequest {
    [Required]
    public Guid BuildingTypeId { get; set; }

    [Required]
    [Range(-100, 100, ErrorMessage = "Position X must be between -100 and 100")]
    public int PositionX { get; set; }

    [Required]
    [Range(-100, 100, ErrorMessage = "Position Y must be between -100 and 100")]
    public int PositionY { get; set; }
}

public class RegisterRequest {
    [Required]
    [StringLength(20, MinimumLength = 4, ErrorMessage = "Username must be 4-20 characters")]
    [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Username can only contain letters, numbers, and underscores")]
    public string Username { get; set; }

    [Required]
    [StringLength(128, MinimumLength = 8)]
    [PasswordComplexity] // Custom validation attribute
    public string Password { get; set; }

    [EmailAddress]
    public string Email { get; set; }
}
```

---

### 3.2 Custom Validation Attributes

```csharp
public class PasswordComplexityAttribute : ValidationAttribute {
    protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
        var password = value as string;

        if (string.IsNullOrEmpty(password)) {
            return new ValidationResult("Password is required");
        }

        var errors = new List<string>();

        if (!password.Any(char.IsUpper)) {
            errors.Add("Password must contain at least one uppercase letter");
        }

        if (!password.Any(char.IsLower)) {
            errors.Add("Password must contain at least one lowercase letter");
        }

        if (!password.Any(char.IsDigit)) {
            errors.Add("Password must contain at least one digit");
        }

        if (!password.Any(ch => !char.IsLetterOrDigit(ch))) {
            errors.Add("Password must contain at least one special character");
        }

        if (CommonPasswords.Contains(password.ToLower())) {
            errors.Add("Password is too common");
        }

        return errors.Any()
            ? new ValidationResult(string.Join("; ", errors))
            : ValidationResult.Success;
    }

    private static readonly HashSet<string> CommonPasswords = new() {
        "password", "123456", "password123", "qwerty", "admin", "letmein"
        // 실제로는 더 많은 일반적인 비밀번호 목록 사용
    };
}
```

---

### 3.3 SQL Injection Prevention

**Entity Framework Core 사용 (파라미터화 자동)**:
```csharp
// ✅ GOOD: EF Core - 자동 파라미터화
var user = await _db.Users
    .FirstOrDefaultAsync(u => u.Username == username);

// ✅ GOOD: Raw SQL with parameters
var users = await _db.Users
    .FromSqlRaw("SELECT * FROM Users WHERE Username = {0}", username)
    .ToListAsync();

// ❌ BAD: String concatenation (절대 금지)
var query = $"SELECT * FROM Users WHERE Username = '{username}'";
// SQL Injection 취약점!
```

---

### 3.4 XSS Prevention

```csharp
// 모든 사용자 입력 HTML 인코딩
public class XssProtectionService {
    public string SanitizeInput(string input) {
        if (string.IsNullOrEmpty(input)) {
            return input;
        }

        // HTML 인코딩
        return HtmlEncoder.Default.Encode(input);
    }

    public string SanitizeForJson(string input) {
        if (string.IsNullOrEmpty(input)) {
            return input;
        }

        // JSON 인코딩
        return JsonEncodedText.Encode(input).ToString();
    }
}

// 사용 예시
public async Task<User> UpdateProfile(UpdateProfileRequest request) {
    var user = await _db.Users.FindAsync(userId);

    user.DisplayName = _xssProtection.SanitizeInput(request.DisplayName);
    user.Bio = _xssProtection.SanitizeInput(request.Bio);

    await _db.SaveChangesAsync();
    return user;
}
```

---

## 4. Rate Limiting

### 4.1 AspNetCoreRateLimit Configuration

```csharp
// appsettings.json
{
  "IpRateLimiting": {
    "EnableEndpointRateLimiting": true,
    "StackBlockedRequests": false,
    "RealIpHeader": "X-Real-IP",
    "ClientIdHeader": "X-ClientId",
    "HttpStatusCode": 429,
    "GeneralRules": [
      {
        "Endpoint": "*",
        "Period": "1m",
        "Limit": 100
      },
      {
        "Endpoint": "*",
        "Period": "1h",
        "Limit": 1000
      }
    ],
    "EndpointWhitelist": [
      "get:/health"
    ],
    "ClientWhitelist": [
      "127.0.0.1"
    ],
    "SpecificEndpointRules": [
      {
        "Endpoint": "post:/api/auth/login",
        "Period": "15m",
        "Limit": 5
      },
      {
        "Endpoint": "post:/api/auth/register",
        "Period": "1h",
        "Limit": 3
      }
    ]
  }
}
```

```csharp
// Startup.cs
public void ConfigureServices(IServiceCollection services) {
    services.AddMemoryCache();
    services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));
    services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
    services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
    services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
    services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
}

public void Configure(IApplicationBuilder app) {
    app.UseIpRateLimiting();
    // ...
}
```

---

### 4.2 Custom Rate Limiting per User

```csharp
[ServiceFilter(typeof(UserRateLimitFilter))]
[HttpPost("expensive-operation")]
public async Task<IActionResult> ExpensiveOperation() {
    // 비용이 많이 드는 작업
}

public class UserRateLimitFilter : IAsyncActionFilter {
    private readonly IDistributedCache _cache;

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next) {
        var userId = context.HttpContext.User.GetUserId();
        var endpoint = context.HttpContext.Request.Path;
        var key = $"user_ratelimit:{userId}:{endpoint}";

        var count = await _cache.GetStringAsync(key);
        var currentCount = string.IsNullOrEmpty(count) ? 0 : int.Parse(count);

        if (currentCount >= 10) { // 10분에 10회
            context.Result = new StatusCodeResult(429);
            return;
        }

        await _cache.SetStringAsync(key, (currentCount + 1).ToString(),
            new DistributedCacheEntryOptions {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });

        await next();
    }
}
```

---

## 5. Data Encryption

### 5.1 Sensitive Data Encryption

```csharp
public class EncryptionService {
    private readonly byte[] _key;
    private readonly byte[] _iv;

    public EncryptionService(IConfiguration config) {
        _key = Convert.FromBase64String(config["Encryption:Key"]);
        _iv = Convert.FromBase64String(config["Encryption:IV"]);
    }

    public string Encrypt(string plainText) {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;

        var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write)) {
            using var sw = new StreamWriter(cs);
            sw.Write(plainText);
        }

        return Convert.ToBase64String(ms.ToArray());
    }

    public string Decrypt(string cipherText) {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;

        var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

        using var ms = new MemoryStream(Convert.FromBase64String(cipherText));
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);

        return sr.ReadToEnd();
    }
}

// 사용 예시
public class User {
    public Guid Id { get; set; }
    public string Username { get; set; }

    // 암호화된 이메일
    private string _encryptedEmail;
    public string Email {
        get => _encryptionService.Decrypt(_encryptedEmail);
        set => _encryptedEmail = _encryptionService.Encrypt(value);
    }
}
```

---

### 5.2 Connection String Encryption

```bash
# appsettings.json에 민감 정보 저장하지 않기
# User Secrets 사용 (개발)
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;..."

# Azure Key Vault 사용 (운영)
```

```csharp
// Program.cs
public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((context, config) => {
            if (context.HostingEnvironment.IsProduction()) {
                var builtConfig = config.Build();
                var keyVaultName = builtConfig["KeyVaultName"];
                var keyVaultUri = new Uri($"https://{keyVaultName}.vault.azure.net/");

                config.AddAzureKeyVault(keyVaultUri, new DefaultAzureCredential());
            }
        })
        .ConfigureWebHostDefaults(webBuilder => {
            webBuilder.UseStartup<Startup>();
        });
```

---

## 6. Security Headers

### 6.1 ASP.NET Core Security Headers

```csharp
public void Configure(IApplicationBuilder app) {
    // HSTS (HTTP Strict Transport Security)
    app.UseHsts();

    // HTTPS 리디렉션
    app.UseHttpsRedirection();

    // 커스텀 보안 헤더
    app.Use(async (context, next) => {
        context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Add("X-Frame-Options", "DENY");
        context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
        context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");

        // Content Security Policy
        context.Response.Headers.Add("Content-Security-Policy",
            "default-src 'self'; " +
            "script-src 'self'; " +
            "style-src 'self' 'unsafe-inline'; " +
            "img-src 'self' data: https:; " +
            "font-src 'self'; " +
            "connect-src 'self' wss://ws.castlelordtycoon.com");

        await next();
    });
}
```

---

### 6.2 CORS Configuration

```csharp
public void ConfigureServices(IServiceCollection services) {
    services.AddCors(options => {
        options.AddPolicy("AllowUnityClient", builder => {
            builder
                .WithOrigins("https://castlelordtycoon.com", "https://www.castlelordtycoon.com")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials(); // SignalR 필요

            // 개발 환경
            if (_env.IsDevelopment()) {
                builder.SetIsOriginAllowed(origin => true);
            }
        });
    });
}

public void Configure(IApplicationBuilder app) {
    app.UseCors("AllowUnityClient");
}
```

---

## 7. Logging and Monitoring

### 7.1 Security Event Logging

```csharp
public class SecurityLogger {
    private readonly ILogger<SecurityLogger> _logger;

    public void LogAuthenticationFailure(string username, string ipAddress) {
        _logger.LogWarning(
            "Authentication failure: Username={Username}, IP={IpAddress}",
            username, ipAddress);
    }

    public void LogSuspiciousActivity(Guid userId, string activityType, object details) {
        _logger.LogWarning(
            "Suspicious activity detected: UserId={UserId}, Type={Type}, Details={Details}",
            userId, activityType, JsonSerializer.Serialize(details));
    }

    public void LogRateLimitExceeded(string clientId, string endpoint) {
        _logger.LogWarning(
            "Rate limit exceeded: ClientId={ClientId}, Endpoint={Endpoint}",
            clientId, endpoint);
    }

    public void LogDataIntegrityViolation(Guid userId, List<string> violations) {
        _logger.LogError(
            "Data integrity violation: UserId={UserId}, Violations={Violations}",
            userId, string.Join(", ", violations));
    }
}
```

---

### 7.2 Audit Trail

```csharp
public class AuditLog {
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Action { get; set; }
    public string EntityType { get; set; }
    public Guid? EntityId { get; set; }
    public string OldValue { get; set; }
    public string NewValue { get; set; }
    public DateTime Timestamp { get; set; }
    public string IpAddress { get; set; }
}

public class AuditService {
    public async Task LogAction(Guid userId, string action, string entityType, Guid? entityId,
        object oldValue, object newValue, string ipAddress) {

        var auditLog = new AuditLog {
            Id = Guid.NewGuid(),
            UserId = userId,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            OldValue = oldValue != null ? JsonSerializer.Serialize(oldValue) : null,
            NewValue = newValue != null ? JsonSerializer.Serialize(newValue) : null,
            Timestamp = DateTime.UtcNow,
            IpAddress = ipAddress
        };

        await _db.AuditLogs.AddAsync(auditLog);
        await _db.SaveChangesAsync();
    }
}

// 사용 예시
public async Task<User> UpdateUserLevel(Guid userId, int newLevel) {
    var user = await _db.Users.FindAsync(userId);
    var oldLevel = user.Level;

    user.Level = newLevel;
    await _db.SaveChangesAsync();

    await _auditService.LogAction(userId, "UPDATE_LEVEL", "User", userId,
        new { Level = oldLevel }, new { Level = newLevel }, GetClientIpAddress());

    return user;
}
```

---

## 8. Server Authority Checklist

### 8.1 Critical Operations Checklist

**모든 중요 작업은 서버에서 검증**:

```yaml
authority_checklist:
  resource_operations:
    - [ ] 자원 획득: 서버가 생산량 계산
    - [ ] 자원 소비: 서버가 잔액 확인 및 차감
    - [ ] 자원 거래: 서버가 양측 잔액 검증

  time_operations:
    - [ ] 건설 완료: 서버 시간으로 검증
    - [ ] 훈련 완료: 서버 시간으로 검증
    - [ ] 연구 완료: 서버 시간으로 검증
    - [ ] 쿨다운: 서버 타임스탬프 사용

  gameplay_operations:
    - [ ] 레벨업: 서버가 경험치 계산
    - [ ] 퀘스트 완료: 서버가 목표 달성 검증
    - [ ] 업적 획득: 서버가 조건 검증
    - [ ] 보상 지급: 서버가 보상 계산

  combat_operations:
    - [ ] 전투 결과: 서버가 승패 계산
    - [ ] 피해량: 서버가 데미지 계산
    - [ ] 전리품: 서버가 드롭 결정

  economic_operations:
    - [ ] 상점 구매: 서버가 가격 및 재고 관리
    - [ ] 유료 결제: 서버가 영수증 검증
    - [ ] 거래소: 서버가 거래 중개
```

---

### 8.2 Never Trust the Client

**클라이언트에서 절대 신뢰하지 않는 데이터**:

```csharp
// ❌ 클라이언트가 보내는 계산 결과 신뢰 금지
public class BadBattleRequest {
    public int DamageDealt { get; set; } // 클라이언트가 계산
    public bool IsVictory { get; set; } // 클라이언트가 결정
}

// ✅ 서버가 모든 계산 수행
public class GoodBattleRequest {
    public Guid DefenderId { get; set; }
    public List<Guid> AttackingUnitIds { get; set; }
}

public async Task<BattleResult> ProcessBattle(GoodBattleRequest request) {
    // 서버에서 모든 검증 및 계산
    var attacker = await GetAttackingForce(request.AttackingUnitIds);
    var defender = await GetDefendingForce(request.DefenderId);

    var damage = CalculateDamage(attacker, defender);
    var isVictory = damage >= defender.HitPoints;

    // 서버가 결과 결정
    return new BattleResult {
        Damage = damage,
        IsVictory = isVictory,
        Loot = isVictory ? CalculateLoot(defender) : null
    };
}
```

---

## 9. Penetration Testing

### 9.1 Security Test Scenarios

```yaml
penetration_tests:
  authentication:
    - test: "Brute force login"
      expected: "Rate limiting blocks after 5 attempts"

    - test: "Token replay attack"
      expected: "Expired tokens rejected"

    - test: "JWT manipulation"
      expected: "Invalid signature rejected"

  authorization:
    - test: "Access other user's castle"
      expected: "403 Forbidden"

    - test: "Escalate privileges"
      expected: "Role claims verified"

  input_validation:
    - test: "SQL injection in username"
      expected: "Parameterized query prevents injection"

    - test: "XSS in profile bio"
      expected: "HTML encoded, script not executed"

    - test: "Negative resource amounts"
      expected: "Validation error, transaction rolled back"

  business_logic:
    - test: "Complete building instantly"
      expected: "Server validates construction time"

    - test: "Duplicate resource collection"
      expected: "Idempotency check prevents double claim"

    - test: "Exceed building limit"
      expected: "Server enforces max building count"
```

---

### 9.2 Automated Security Scanning

```bash
# OWASP Dependency Check
dotnet tool install --global dependency-check
dependency-check --project "CastleLordTycoon" --scan ./server

# SonarQube 정적 분석
dotnet sonarscanner begin /k:"CastleLordTycoon" /d:sonar.host.url="http://localhost:9000"
dotnet build server/CastleLordTycoon.sln
dotnet sonarscanner end

# Docker 이미지 취약점 스캔
docker scan castle-lord-api:latest
```

---

## 10. Incident Response

### 10.1 Security Incident Playbook

```yaml
incident_response:
  detection:
    - monitor: "로그에서 의심 활동 패턴 감지"
    - alert: "관리자에게 즉시 알림 (Slack, Email)"

  containment:
    - action: "의심 계정 일시 정지"
    - action: "IP 주소 차단"
    - action: "영향받은 데이터 격리"

  investigation:
    - action: "로그 분석 (auth, audit, security)"
    - action: "데이터베이스 무결성 검사"
    - action: "영향 범위 파악"

  eradication:
    - action: "취약점 패치"
    - action: "불법 데이터 제거"
    - action: "비밀번호/토큰 무효화"

  recovery:
    - action: "백업에서 데이터 복구"
    - action: "서비스 재시작"
    - action: "모니터링 강화"

  post_incident:
    - action: "사후 보고서 작성"
    - action: "보안 정책 업데이트"
    - action: "사용자 공지 (필요시)"
```

---

### 10.2 Rollback Procedure

```bash
#!/bin/bash
# scripts/rollback.sh

echo "Starting emergency rollback..."

# 1. 현재 버전 백업
docker-compose exec postgres pg_dump -U admin castlelord_prod > /tmp/pre-rollback-backup.sql

# 2. 이전 Docker 이미지로 롤백
docker-compose down
docker-compose pull castle-lord-api:previous-stable
docker-compose up -d

# 3. 데이터베이스 마이그레이션 롤백 (필요시)
# docker-compose run --rm api dotnet ef database update PreviousMigration

# 4. 헬스 체크
sleep 10
curl -f https://api.castlelordtycoon.com/health || echo "Rollback failed!"

echo "Rollback completed. Check logs: docker-compose logs api"
```

---

## 11. Compliance

### 11.1 GDPR Compliance

```csharp
public class UserDataController : ControllerBase {
    // 개인정보 다운로드 (GDPR Article 15)
    [HttpGet("me/data-export")]
    public async Task<IActionResult> ExportUserData() {
        var userId = User.GetUserId();
        var userData = await _userService.ExportAllUserData(userId);

        var json = JsonSerializer.Serialize(userData, new JsonSerializerOptions {
            WriteIndented = true
        });

        return File(Encoding.UTF8.GetBytes(json), "application/json", "my-data.json");
    }

    // 계정 삭제 (GDPR Article 17 - Right to be forgotten)
    [HttpDelete("me")]
    public async Task<IActionResult> DeleteAccount([FromBody] DeleteAccountRequest request) {
        var userId = User.GetUserId();

        // 비밀번호 재확인
        if (!await _authService.VerifyPassword(userId, request.Password)) {
            return Unauthorized();
        }

        // 개인정보 익명화 (완전 삭제 대신)
        await _userService.AnonymizeUser(userId);

        return NoContent();
    }
}
```

---

### 11.2 Data Retention Policy

```yaml
data_retention:
  user_accounts:
    active: "무기한 보관"
    inactive_3_years: "이메일 알림 → 30일 후 삭제"
    deleted: "즉시 익명화, 30일 후 완전 삭제"

  logs:
    application_logs: "90일 보관"
    security_logs: "1년 보관"
    audit_logs: "7년 보관 (법적 요구사항)"

  backups:
    daily_backups: "7일 보관"
    weekly_backups: "4주 보관"
    monthly_backups: "12개월 보관"
```

---

## 12. Security Training

### 12.1 Developer Security Checklist

**개발자 보안 체크리스트**:

```markdown
- [ ] 모든 사용자 입력 검증 (클라이언트 + 서버)
- [ ] SQL Injection 방지 (파라미터화 쿼리)
- [ ] XSS 방지 (HTML 인코딩)
- [ ] CSRF 방지 (Anti-Forgery Token)
- [ ] 비밀번호 안전하게 저장 (해시 + 솔트)
- [ ] JWT 토큰 안전하게 관리 (짧은 만료 시간)
- [ ] HTTPS 강제 (HSTS)
- [ ] 민감 정보 로그 출력 금지
- [ ] 에러 메시지에 시스템 정보 노출 금지
- [ ] 권한 검증 (모든 엔드포인트)
- [ ] Rate Limiting 적용
- [ ] 의존성 최신 버전 유지
```

---

## 13. Resources

**보안 참고 자료**:
- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [CWE Top 25](https://cwe.mitre.org/top25/)
- [ASP.NET Core Security](https://docs.microsoft.com/en-us/aspnet/core/security/)
- [Unity Networking Best Practices](https://docs.unity3d.com/Manual/BestPracticeUnderstandingPerformanceInUnity.html)

---

## 14. Contact

**보안 취약점 제보**:
- Email: security@castlelordtycoon.com
- Responsible Disclosure Policy 준수
- Bug Bounty Program (향후)
