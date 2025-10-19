# Google Sign-In 통합 가이드

## 📋 개요

Unity 클라이언트에서 Google Sign-In Plugin을 사용하여 Google 계정 로그인을 구현하고, ASP.NET Core 서버에서 토큰을 검증하는 전체 프로세스를 설명합니다.

**핵심 특징**:
- ✅ 완전 무료 (Firebase 불필요)
- ✅ Google 공식 UI (계정 선택, 권한 동의)
- ✅ 사용량 제한 없음
- ✅ 간단한 구현

---

## 🎯 전체 흐름

```
[Unity Client]
    ↓
Google Sign-In Plugin.SignIn()
    ↓
[Google 공식 UI] (Chrome Custom Tab)
    ↓ 사용자 로그인
Google ID Token 발급
    ↓
POST /api/auth/google-login
    ↓
[ASP.NET Core Server]
    ↓
Google API로 토큰 검증
    ↓
사용자 DB 조회/생성
    ↓
자체 JWT 발급
    ↓
[Unity Client]
    ↓
게임 플레이
```

---

## 📝 사전 준비사항

### 1. Google Cloud Console 설정 필요
- Google Cloud 프로젝트 생성
- OAuth 2.0 Client ID 생성 (Android)
- SHA-1 지문 등록

### 2. Unity Plugin 설치 필요
- Google Sign-In Plugin for Unity

### 3. 서버 패키지 설치 필요
- Google.Apis.Auth (NuGet)

---

## 🔧 1단계: Google Cloud Console 설정

### 1.1 Google Cloud 프로젝트 생성

1. **Google Cloud Console 접속**
   - https://console.cloud.google.com/

2. **새 프로젝트 생성**
   ```
   프로젝트 이름: Castle Lord Tycoon
   조직: 없음 (개인 프로젝트)
   ```

3. **프로젝트 선택**
   - 상단 프로젝트 선택 드롭다운에서 방금 생성한 프로젝트 선택

---

### 1.2 OAuth 동의 화면 구성

1. **APIs & Services → OAuth consent screen**

2. **User Type 선택**
   ```
   ◉ External (외부)
   - 누구나 Google 계정으로 로그인 가능
   - 테스트 모드: 100명 제한
   - 배포 후: 제한 없음
   ```

3. **앱 정보 입력**
   ```yaml
   앱 이름: Castle Lord Tycoon
   사용자 지원 이메일: your-email@gmail.com
   앱 로고: (선택) 512x512 PNG
   앱 도메인:
     - 앱 홈페이지: https://castle.yourdomain.com (나중에)
     - 개인정보처리방침: https://castle.yourdomain.com/privacy (나중에)
     - 서비스 약관: https://castle.yourdomain.com/terms (나중에)
   승인된 도메인: yourdomain.com
   개발자 연락처: your-email@gmail.com
   ```

4. **범위 (Scopes) 설정**
   ```
   ✅ .../auth/userinfo.email
   ✅ .../auth/userinfo.profile
   ✅ openid

   (기본 프로필과 이메일만 필요)
   ```

5. **테스트 사용자 추가 (개발 중)**
   ```
   your-email@gmail.com
   tester1@gmail.com
   tester2@gmail.com

   (배포 전까지는 이 계정들만 로그인 가능)
   ```

6. **저장 후 대시보드로 이동**

---

### 1.3 OAuth 2.0 Client ID 생성 (Android)

1. **APIs & Services → Credentials**

2. **+ CREATE CREDENTIALS → OAuth client ID**

3. **Application type 선택**
   ```
   ◉ Android
   ```

4. **Android 앱 정보 입력**

   **Name**:
   ```
   Castle Lord Tycoon - Android
   ```

   **Package name**:
   ```
   com.yourcompany.castlelordtycoon

   ⚠️ Unity의 Package Name과 정확히 일치해야 함!
   (Edit → Project Settings → Player → Android → Package Name)
   ```

   **SHA-1 certificate fingerprint**:
   ```
   (아래 "SHA-1 지문 생성" 섹션 참조)
   ```

5. **CREATE 클릭**

6. **Client ID 저장**
   ```
   생성된 Client ID를 메모장에 복사

   예시:
   123456789012-abcdefghijklmnopqrstuvwxyz123456.apps.googleusercontent.com
   ```

---

### 1.4 SHA-1 지문 생성

#### 방법 1: Unity Debug Keystore (개발용)

**Windows**:
```bash
# Unity 기본 keystore 위치
C:\Users\YourUsername\.android\debug.keystore

# SHA-1 추출
keytool -list -v -keystore "%USERPROFILE%\.android\debug.keystore" -alias androiddebugkey -storepass android -keypass android
```

**Mac/Linux**:
```bash
# Unity 기본 keystore 위치
~/.android/debug.keystore

# SHA-1 추출
keytool -list -v -keystore ~/.android/debug.keystore -alias androiddebugkey -storepass android -keypass android
```

**출력 예시**:
```
Certificate fingerprints:
    SHA1: A1:B2:C3:D4:E5:F6:01:23:45:67:89:AB:CD:EF:01:23:45:67:89:AB
    SHA256: ...
```

→ **SHA1 값을 Google Cloud Console에 입력**

---

#### 방법 2: 배포용 Keystore (릴리스용)

**1. 새 Keystore 생성**:
```bash
keytool -genkey -v -keystore castle-lord-tycoon.keystore -alias castle_key -keyalg RSA -keysize 2048 -validity 10000

# 입력 사항:
- Keystore password: (비밀번호 입력 - 잊지 마세요!)
- Key password: (동일하게 입력)
- First and last name: Your Name
- Organizational unit: Game Development
- Organization: Your Company
- City: Seoul
- State: Seoul
- Country code: KR
```

**2. SHA-1 추출**:
```bash
keytool -list -v -keystore castle-lord-tycoon.keystore -alias castle_key
```

**3. Keystore 안전하게 보관**:
```
⚠️ 중요: 이 파일과 비밀번호를 잃어버리면 앱 업데이트 불가!

저장 위치:
- 로컬: 안전한 폴더 (Git에 올리지 말 것!)
- 백업: 클라우드 스토리지 (암호화)
```

**4. Unity 설정**:
```
Edit → Project Settings → Player → Android → Publishing Settings
  ✅ Custom Keystore
  ✅ Browse... → castle-lord-tycoon.keystore 선택
  Keystore password: (입력)
  Alias: castle_key
  Password: (입력)
```

---

### 1.5 추가 Client ID 생성 (선택)

#### Web Client ID (서버 검증용)

Google ID Token을 서버에서 검증할 때 필요할 수 있습니다.

1. **+ CREATE CREDENTIALS → OAuth client ID**
2. **Application type: Web application**
3. **Name**: Castle Lord Tycoon - Backend
4. **Authorized redirect URIs**: (비워둠 - 서버 검증만 사용)
5. **CREATE**
6. **Client ID 저장** (서버 설정에 사용)

---

## 🎮 2단계: Unity 클라이언트 설정

### 2.1 Google Sign-In Plugin 설치

#### 방법 1: Unity Package Manager (추천)

1. **GitHub 릴리즈에서 다운로드**
   - https://github.com/googlesamples/google-signin-unity/releases
   - `google-signin-plugin-X.X.X.unitypackage` 다운로드

2. **Unity에서 Import**
   ```
   Assets → Import Package → Custom Package...
   → google-signin-plugin-X.X.X.unitypackage 선택
   → Import
   ```

---

#### 방법 2: Git URL로 설치

**Unity Package Manager**:
```
Window → Package Manager
  + (Add package from git URL...)

Git URL:
https://github.com/googlesamples/google-signin-unity.git
```

---

### 2.2 Google Sign-In 설정 파일 생성

1. **Assets 폴더에 `GoogleSignInConfiguration` 생성**

   ```
   Assets → Create → Google Sign-In Configuration
   ```

2. **Inspector에서 설정**

   ```yaml
   Web Client ID:
     (Google Cloud Console에서 생성한 Android Client ID)
     123456789012-abcdefghijklmnopqrstuvwxyz123456.apps.googleusercontent.com

   Use Game Sign In: ❌ (체크 해제)
   Request Id Token: ✅ (체크)
   Request Email: ✅ (체크)
   Request Profile: ✅ (체크)
   Request Auth Code: ❌ (체크 해제)
   Hide Popup During Load: ✅ (체크)
   ```

3. **파일 저장**
   ```
   Assets/GoogleSignInConfiguration.asset
   ```

---

### 2.3 Android Resolver 설정

Google Sign-In Plugin은 Android 네이티브 라이브러리를 사용합니다.

1. **External Dependency Manager 확인**
   ```
   Assets → External Dependency Manager → Android Resolver → Settings

   ✅ Enable Auto-Resolution
   ✅ Enable Resolution On Build
   ```

2. **의존성 해결**
   ```
   Assets → External Dependency Manager → Android Resolver → Resolve

   (Google Play Services 라이브러리 자동 다운로드)
   ```

---

### 2.4 Unity 스크립트 작성

#### GoogleSignInManager.cs

```csharp
using System;
using UnityEngine;
using Google;

namespace CastleLordTycoon.Network
{
    /// <summary>
    /// Google Sign-In 관리
    /// </summary>
    public class GoogleSignInManager : MonoBehaviour
    {
        private static GoogleSignInManager _instance;
        public static GoogleSignInManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("GoogleSignInManager");
                    _instance = go.AddComponent<GoogleSignInManager>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        private GoogleSignInConfiguration _configuration;

        // 이벤트
        public event Action<string> OnSignInSuccess;      // ID Token
        public event Action<string> OnSignInFailed;

        private void Awake()
        {
            // GoogleSignInConfiguration 로드
            _configuration = Resources.Load<GoogleSignInConfiguration>("GoogleSignInConfiguration");

            if (_configuration == null)
            {
                Debug.LogError("GoogleSignInConfiguration을 찾을 수 없습니다!");
                return;
            }

            // Google Sign-In 초기화
            GoogleSignIn.Configuration = _configuration;
            GoogleSignIn.Configuration.UseGameSignIn = false;
            GoogleSignIn.Configuration.RequestIdToken = true;
            GoogleSignIn.Configuration.RequestEmail = true;
        }

        /// <summary>
        /// Google 로그인 시작
        /// </summary>
        public void SignIn()
        {
            Debug.Log("Google Sign-In 시작...");

            GoogleSignIn.DefaultInstance.SignIn().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    // 에러 처리
                    using (IEnumerator<System.Exception> enumerator = task.Exception.InnerExceptions.GetEnumerator())
                    {
                        if (enumerator.MoveNext())
                        {
                            GoogleSignIn.SignInException error = (GoogleSignIn.SignInException)enumerator.Current;
                            Debug.LogError($"Google Sign-In 실패: {error.Status} - {error.Message}");
                            OnSignInFailed?.Invoke(error.Message);
                        }
                    }
                }
                else if (task.IsCanceled)
                {
                    Debug.Log("Google Sign-In 취소됨");
                    OnSignInFailed?.Invoke("사용자가 로그인을 취소했습니다.");
                }
                else
                {
                    // 성공
                    GoogleSignInUser user = task.Result;

                    Debug.Log($"Google Sign-In 성공!");
                    Debug.Log($"  이름: {user.DisplayName}");
                    Debug.Log($"  이메일: {user.Email}");
                    Debug.Log($"  ID: {user.UserId}");
                    Debug.Log($"  ID Token: {user.IdToken.Substring(0, 20)}...");

                    // ID Token을 서버로 전송
                    OnSignInSuccess?.Invoke(user.IdToken);
                }
            });
        }

        /// <summary>
        /// Google 로그아웃
        /// </summary>
        public void SignOut()
        {
            GoogleSignIn.DefaultInstance.SignOut();
            Debug.Log("Google Sign-Out 완료");
        }

        /// <summary>
        /// Google 연결 해제
        /// </summary>
        public void Disconnect()
        {
            GoogleSignIn.DefaultInstance.Disconnect();
            Debug.Log("Google 연결 해제 완료");
        }
    }
}
```

---

### 2.5 AuthService 업데이트

기존 `AuthService.cs`에 Google 로그인 메서드 추가:

```csharp
/// <summary>
/// Google 로그인
/// </summary>
public void LoginWithGoogle(string idToken, Action<bool, string> callback)
{
    var request = new GoogleLoginRequest
    {
        idToken = idToken
    };

    HttpClient.Instance.Post<GoogleLoginRequest, LoginResponse>(
        ApiConfig.Auth.GOOGLE_LOGIN,
        request,
        response =>
        {
            if (response.success)
            {
                // 토큰 저장
                HttpClient.Instance.SetAccessToken(response.data.accessToken);
                HttpClient.Instance.SetRefreshToken(response.data.refreshToken);

                // 사용자 정보 저장
                _currentUserId = response.data.userId;
                _currentUsername = response.data.username;

                // PlayerPrefs에 저장
                PlayerPrefs.SetString("RefreshToken", response.data.refreshToken);
                PlayerPrefs.SetString("UserId", response.data.userId);
                PlayerPrefs.SetString("Username", response.data.username);
                PlayerPrefs.Save();

                Debug.Log($"Google 로그인 성공: {_currentUsername}");
                OnLoginSuccess?.Invoke(_currentUsername);
                callback?.Invoke(true, "Google 로그인 성공");
            }
            else
            {
                Debug.LogWarning($"Google 로그인 실패: {response.error.message}");
                OnLoginFailed?.Invoke(response.error.message);
                callback?.Invoke(false, response.error.message);
            }
        },
        error =>
        {
            Debug.LogError($"Google 로그인 요청 실패: {error}");
            OnLoginFailed?.Invoke(error);
            callback?.Invoke(false, error);
        }
    );
}
```

---

### 2.6 데이터 모델 추가

`AuthModels.cs`에 Google 로그인 요청 추가:

```csharp
/// <summary>
/// Google 로그인 요청
/// </summary>
[Serializable]
public class GoogleLoginRequest
{
    public string idToken;
}
```

---

### 2.7 API 엔드포인트 추가

`ApiConfig.cs`의 Auth 클래스에 추가:

```csharp
public static class Auth
{
    public const string REGISTER = "/api/auth/register";
    public const string LOGIN = "/api/auth/login";
    public const string GOOGLE_LOGIN = "/api/auth/google-login";  // ← 추가
    public const string REFRESH = "/api/auth/refresh";
    public const string LOGOUT = "/api/auth/logout";
}
```

---

## 🖥️ 3단계: ASP.NET Core 서버 설정

### 3.1 NuGet 패키지 설치

```bash
cd CastleLordTycoon.Server

dotnet add package Google.Apis.Auth
```

**패키지 정보**:
- `Google.Apis.Auth`: Google ID Token 검증용
- 완전 무료, 사용량 제한 없음

---

### 3.2 appsettings.json 설정

```json
{
  "GoogleAuth": {
    "ClientId": "123456789012-abcdefghijklmnopqrstuvwxyz123456.apps.googleusercontent.com"
  }
}
```

⚠️ **주의**:
- Android Client ID를 입력
- Web Client ID가 아님!

---

### 3.3 DTO 모델 생성

`Models/Auth/GoogleLoginRequest.cs`:

```csharp
namespace CastleLordTycoon.Server.Models.Auth
{
    public class GoogleLoginRequest
    {
        public string IdToken { get; set; }
    }
}
```

---

### 3.4 Google Token 검증 서비스

`Services/GoogleAuthService.cs`:

```csharp
using System;
using System.Threading.Tasks;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CastleLordTycoon.Server.Services
{
    public class GoogleAuthService
    {
        private readonly string _clientId;
        private readonly ILogger<GoogleAuthService> _logger;

        public GoogleAuthService(IConfiguration configuration, ILogger<GoogleAuthService> logger)
        {
            _clientId = configuration["GoogleAuth:ClientId"];
            _logger = logger;
        }

        /// <summary>
        /// Google ID Token 검증
        /// </summary>
        public async Task<GoogleJsonWebSignature.Payload> ValidateIdTokenAsync(string idToken)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _clientId }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

                _logger.LogInformation($"Google 토큰 검증 성공: {payload.Email}");

                return payload;
            }
            catch (InvalidJwtException ex)
            {
                _logger.LogWarning($"Google 토큰 검증 실패: {ex.Message}");
                throw new UnauthorizedAccessException("유효하지 않은 Google 토큰입니다.", ex);
            }
        }
    }
}
```

---

### 3.5 AuthController 업데이트

`Controllers/AuthController.cs`에 Google 로그인 엔드포인트 추가:

```csharp
[HttpPost("google-login")]
public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
{
    try
    {
        // 1. Google ID Token 검증
        var payload = await _googleAuthService.ValidateIdTokenAsync(request.IdToken);

        // 2. 사용자 정보 추출
        string googleUserId = payload.Subject;      // Google User ID
        string email = payload.Email;
        string name = payload.Name;

        // 3. DB에서 사용자 조회
        var user = await _userRepository.GetByGoogleIdAsync(googleUserId);

        // 4. 신규 사용자면 생성
        if (user == null)
        {
            user = new User
            {
                GoogleId = googleUserId,
                Email = email,
                Username = name,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.CreateAsync(user);
            _logger.LogInformation($"Google 신규 사용자 생성: {email}");
        }

        // 5. JWT 발급
        var accessToken = _jwtService.GenerateAccessToken(user.Id);
        var refreshToken = _jwtService.GenerateRefreshToken();

        // 6. Refresh Token 저장
        await _userRepository.UpdateRefreshTokenAsync(user.Id, refreshToken);

        // 7. 응답
        return Ok(new
        {
            success = true,
            data = new
            {
                accessToken,
                refreshToken,
                expiresIn = 3600,
                userId = user.Id,
                username = user.Username
            }
        });
    }
    catch (UnauthorizedAccessException ex)
    {
        return Unauthorized(new
        {
            success = false,
            error = new { code = "INVALID_GOOGLE_TOKEN", message = ex.Message }
        });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Google 로그인 실패");
        return StatusCode(500, new
        {
            success = false,
            error = new { code = "INTERNAL_ERROR", message = "서버 오류가 발생했습니다." }
        });
    }
}
```

---

### 3.6 User 모델 업데이트

Google ID 필드 추가:

```csharp
public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string? Email { get; set; }
    public string? PasswordHash { get; set; }      // Email 로그인용
    public string? GoogleId { get; set; }          // ← 추가
    public DateTime CreatedAt { get; set; }
}
```

---

### 3.7 데이터베이스 마이그레이션

```bash
# Entity Framework 사용 시
dotnet ef migrations add AddGoogleIdToUser
dotnet ef database update
```

**PostgreSQL 직접 수정**:
```sql
ALTER TABLE players ADD COLUMN google_id VARCHAR(255) UNIQUE;
CREATE INDEX idx_players_google_id ON players(google_id);
```

---

## 🧪 4단계: 테스트

### 4.1 Unity 테스트

1. **테스트 씬 생성**
   ```
   Assets/Scenes/GoogleSignInTest.unity
   ```

2. **테스트 UI 생성**
   - Button: "Google 로그인"
   - Text: 로그인 결과 표시

3. **테스트 스크립트**

```csharp
using UnityEngine;
using UnityEngine.UI;
using CastleLordTycoon.Network;

public class GoogleSignInTest : MonoBehaviour
{
    public Button signInButton;
    public Text resultText;

    private void Start()
    {
        signInButton.onClick.AddListener(OnSignInClicked);

        // 이벤트 리스너 등록
        GoogleSignInManager.Instance.OnSignInSuccess += OnGoogleSignInSuccess;
        GoogleSignInManager.Instance.OnSignInFailed += OnGoogleSignInFailed;
    }

    private void OnSignInClicked()
    {
        resultText.text = "Google 로그인 중...";
        GoogleSignInManager.Instance.SignIn();
    }

    private void OnGoogleSignInSuccess(string idToken)
    {
        resultText.text = $"Google 로그인 성공!\nID Token: {idToken.Substring(0, 20)}...";

        // 서버로 ID Token 전송
        AuthService.Instance.LoginWithGoogle(idToken, (success, message) =>
        {
            if (success)
            {
                resultText.text = "서버 인증 성공!";
            }
            else
            {
                resultText.text = $"서버 인증 실패: {message}";
            }
        });
    }

    private void OnGoogleSignInFailed(string error)
    {
        resultText.text = $"Google 로그인 실패: {error}";
    }
}
```

---

### 4.2 서버 테스트

1. **서버 실행**
   ```bash
   cd CastleLordTycoon.Server
   dotnet run
   ```

2. **Swagger 확인**
   ```
   http://localhost:10010/swagger
   → POST /api/auth/google-login 확인
   ```

3. **수동 테스트** (Postman/curl)
   ```bash
   curl -X POST http://localhost:10010/api/auth/google-login \
     -H "Content-Type: application/json" \
     -d '{"idToken": "eyJhbGciOiJSUzI1NiIsImtpZCI6..."}'
   ```

---

### 4.3 통합 테스트

1. **Unity Play 모드 실행**
2. **"Google 로그인" 버튼 클릭**
3. **Google 계정 선택 UI 확인**
4. **권한 동의 화면 확인**
5. **서버 인증 성공 확인**
6. **Unity Console 로그 확인**

**예상 로그**:
```
Google Sign-In 시작...
Google Sign-In 성공!
  이름: Hong Gildong
  이메일: hong@gmail.com
  ID: 123456789012345678901
  ID Token: eyJhbGciOiJSUzI1Ni...
Google 로그인 성공: Hong Gildong
```

---

## 🚨 문제 해결

### 문제 1: "Developer Error" 표시

**원인**:
- SHA-1 지문 불일치
- Package Name 불일치

**해결**:
1. Unity Package Name 확인
2. Keystore SHA-1 재확인
3. Google Cloud Console에서 Client ID 재생성

---

### 문제 2: "API not enabled"

**원인**: Google+ API 비활성화

**해결**:
```
Google Cloud Console
→ APIs & Services
→ Library
→ "Google+ API" 검색
→ Enable
```

---

### 문제 3: "Token verification failed"

**원인**:
- 서버의 Client ID 불일치
- 만료된 토큰

**해결**:
1. `appsettings.json`의 ClientId 확인
2. Unity에서 토큰 재발급

---

### 문제 4: 테스트 사용자만 로그인 가능

**원인**: OAuth 동의 화면이 "Testing" 상태

**해결**:
```
Google Cloud Console
→ APIs & Services
→ OAuth consent screen
→ PUBLISH APP (배포 후)
```

⚠️ **주의**: 배포 전에는 테스트 사용자 추가 필요

---

## 📊 보안 고려사항

### 1. ID Token 검증 필수

```
❌ 클라이언트가 보낸 토큰을 무조건 신뢰
✅ 서버에서 Google API로 반드시 검증
```

### 2. HTTPS 사용

```
개발: HTTP (localhost)
배포: HTTPS 필수
```

### 3. Token 만료 처리

```csharp
// ID Token은 1시간 유효
// 만료 시 재로그인 필요
```

### 4. Refresh Token 관리

```csharp
// 자체 Refresh Token 발급 (7일)
// Google Refresh Token은 사용 안함
```

---

## 📱 5단계: Android 빌드 테스트

### 5.1 빌드 설정

```
File → Build Settings
  Platform: Android
  ✅ Development Build (테스트용)

Build
```

### 5.2 실제 기기 테스트

1. **APK 설치**
2. **앱 실행**
3. **Google 로그인 버튼 클릭**
4. **실제 Google 계정으로 로그인**
5. **서버 연결 확인**

---

## 🎯 다음 단계

### Phase 1 완료 체크리스트

- ✅ Google Cloud Console 설정
- ✅ Unity Plugin 설치
- ✅ 서버 토큰 검증 구현
- ✅ 테스트 성공
- ✅ Android 빌드 테스트

### Phase 2: UI 개선

- 로그인 화면 디자인
- 로딩 애니메이션
- 에러 메시지 UI
- 로그아웃 기능

### Phase 3: 추가 기능

- 게스트 로그인
- 계정 연결 (Email → Google)
- 자동 로그인
- 로그인 보상

---

## 📚 참고 자료

### 공식 문서

- **Google Sign-In for Unity**: https://github.com/googlesamples/google-signin-unity
- **Google Identity**: https://developers.google.com/identity
- **OAuth 2.0**: https://developers.google.com/identity/protocols/oauth2

### NuGet 패키지

- **Google.Apis.Auth**: https://www.nuget.org/packages/Google.Apis.Auth

---

**최종 수정**: 2025-10-19
**문서 상태**: 🟢 구현 준비 완료
**예상 구현 시간**: 2-3일
