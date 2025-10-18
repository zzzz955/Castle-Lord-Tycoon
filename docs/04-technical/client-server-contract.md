# Client-Server Contract Specification

## Overview

이 문서는 Unity 클라이언트와 ASP.NET Core 서버 간의 API 계약을 정의합니다.

**통신 프로토콜**:
- REST API: HTTP/HTTPS (상태 변경, 조회)
- SignalR: WebSocket (실시간 업데이트)

**인증 방식**: JWT Bearer Token

---

## 1. REST API Endpoints

### 1.1 Authentication

#### POST /api/auth/register
신규 사용자 등록

**Request**:
```json
{
  "username": "string (4-20자, 영문+숫자)",
  "password": "string (8-32자, 영문+숫자+특수문자)",
  "email": "string (선택)"
}
```

**Response** (201 Created):
```json
{
  "success": true,
  "data": {
    "userId": "guid",
    "username": "string",
    "createdAt": "datetime"
  }
}
```

**Errors**:
- 400: `USERNAME_ALREADY_EXISTS`, `INVALID_PASSWORD_FORMAT`
- 429: `TOO_MANY_REQUESTS`

---

#### POST /api/auth/login
로그인 및 JWT 토큰 발급

**Request**:
```json
{
  "username": "string",
  "password": "string"
}
```

**Response** (200 OK):
```json
{
  "success": true,
  "data": {
    "accessToken": "string (JWT)",
    "refreshToken": "string",
    "expiresIn": 3600,
    "userId": "guid",
    "username": "string"
  }
}
```

**Errors**:
- 401: `INVALID_CREDENTIALS`
- 429: `TOO_MANY_REQUESTS`

---

#### POST /api/auth/refresh
액세스 토큰 갱신

**Request**:
```json
{
  "refreshToken": "string"
}
```

**Response** (200 OK):
```json
{
  "success": true,
  "data": {
    "accessToken": "string",
    "expiresIn": 3600
  }
}
```

**Errors**:
- 401: `INVALID_REFRESH_TOKEN`, `REFRESH_TOKEN_EXPIRED`

---

### 1.2 User Management

#### GET /api/users/me
현재 사용자 정보 조회

**Headers**: `Authorization: Bearer {token}`

**Response** (200 OK):
```json
{
  "success": true,
  "data": {
    "userId": "guid",
    "username": "string",
    "level": 12,
    "experience": 2450,
    "gold": 15000,
    "gem": 50,
    "lastLoginAt": "datetime",
    "createdAt": "datetime"
  }
}
```

---

#### PATCH /api/users/me/settings
사용자 설정 업데이트

**Request**:
```json
{
  "settings": {
    "soundEnabled": true,
    "musicEnabled": false,
    "language": "ko"
  }
}
```

**Response** (200 OK):
```json
{
  "success": true,
  "data": {
    "settings": { /* 업데이트된 설정 */ }
  }
}
```

---

### 1.3 Castle Management

#### GET /api/castles/{castleId}
성 상태 조회

**Response** (200 OK):
```json
{
  "success": true,
  "data": {
    "castleId": "guid",
    "userId": "guid",
    "level": 5,
    "resources": {
      "gold": 15000,
      "wood": 500,
      "stone": 300,
      "food": 1000
    },
    "buildings": [
      {
        "buildingId": "guid",
        "buildingType": "TOWNHALL",
        "level": 3,
        "position": { "x": 10, "y": 5 },
        "isUnderConstruction": false,
        "productionRate": 10.5
      }
    ],
    "units": [
      {
        "unitType": "KNIGHT",
        "count": 25
      }
    ]
  }
}
```

**Errors**:
- 404: `CASTLE_NOT_FOUND`
- 403: `ACCESS_DENIED`

---

#### POST /api/castles/{castleId}/buildings
건물 건설 시작

**Request**:
```json
{
  "buildingType": "BARRACKS",
  "position": {
    "x": 15,
    "y": 10
  }
}
```

**Response** (202 Accepted):
```json
{
  "success": true,
  "data": {
    "buildingId": "guid",
    "constructionEndTime": "datetime",
    "resourceCost": {
      "gold": 500,
      "wood": 200,
      "stone": 100
    }
  }
}
```

**Errors**:
- 400: `INVALID_POSITION`, `INSUFFICIENT_RESOURCES`, `MAX_BUILDINGS_REACHED`
- 409: `POSITION_OCCUPIED`

---

#### POST /api/castles/{castleId}/buildings/{buildingId}/upgrade
건물 업그레이드

**Response** (202 Accepted):
```json
{
  "success": true,
  "data": {
    "newLevel": 4,
    "upgradeEndTime": "datetime",
    "resourceCost": {
      "gold": 1000,
      "wood": 400
    }
  }
}
```

**Errors**:
- 400: `MAX_LEVEL_REACHED`, `INSUFFICIENT_RESOURCES`
- 409: `UPGRADE_IN_PROGRESS`

---

#### POST /api/castles/{castleId}/buildings/{buildingId}/collect
자원 수집

**Response** (200 OK):
```json
{
  "success": true,
  "data": {
    "collectedResources": {
      "gold": 250
    },
    "newTotal": {
      "gold": 15250
    }
  }
}
```

---

### 1.4 Production System

#### POST /api/castles/{castleId}/units/train
유닛 훈련 시작

**Request**:
```json
{
  "unitType": "KNIGHT",
  "quantity": 10,
  "buildingId": "guid (훈련소)"
}
```

**Response** (202 Accepted):
```json
{
  "success": true,
  "data": {
    "trainingId": "guid",
    "completionTime": "datetime",
    "resourceCost": {
      "gold": 500,
      "food": 200
    }
  }
}
```

**Errors**:
- 400: `INSUFFICIENT_RESOURCES`, `INVALID_BUILDING`
- 409: `TRAINING_QUEUE_FULL`

---

### 1.5 Research System

#### GET /api/research
연구 가능 항목 목록

**Response** (200 OK):
```json
{
  "success": true,
  "data": {
    "availableResearch": [
      {
        "researchId": "MINING_EFFICIENCY",
        "name": "채굴 효율 향상",
        "level": 2,
        "requirements": {
          "townhallLevel": 3
        },
        "cost": {
          "gold": 2000,
          "researchTime": 3600
        },
        "effects": {
          "goldProductionBonus": 0.15
        }
      }
    ]
  }
}
```

---

#### POST /api/research/{researchId}/start
연구 시작

**Response** (202 Accepted):
```json
{
  "success": true,
  "data": {
    "completionTime": "datetime",
    "resourceCost": { "gold": 2000 }
  }
}
```

---

### 1.6 Quest System

#### GET /api/quests
퀘스트 목록 조회

**Query Parameters**:
- `status`: `active`, `completed`, `available`

**Response** (200 OK):
```json
{
  "success": true,
  "data": {
    "quests": [
      {
        "questId": "guid",
        "title": "첫 번째 병영 건설",
        "description": "병영을 건설하여 군대를 양성하세요",
        "type": "MAIN",
        "objectives": [
          {
            "type": "BUILD_BUILDING",
            "target": "BARRACKS",
            "current": 0,
            "required": 1
          }
        ],
        "rewards": {
          "gold": 500,
          "experience": 100
        },
        "status": "ACTIVE"
      }
    ]
  }
}
```

---

#### POST /api/quests/{questId}/complete
퀘스트 완료 보상 수령

**Response** (200 OK):
```json
{
  "success": true,
  "data": {
    "rewards": {
      "gold": 500,
      "experience": 100
    },
    "newLevel": 2,
    "nextQuests": ["guid"]
  }
}
```

---

## 2. SignalR Hub Methods

### 2.1 Connection

#### Hub: `/hubs/game`

**클라이언트 → 서버**:
```csharp
// 연결 시 인증
await connection.InvokeAsync("Authenticate", jwtToken);

// 특정 성 구독
await connection.InvokeAsync("SubscribeToCastle", castleId);
```

**서버 → 클라이언트**:
```csharp
// 연결 성공
connection.On<ConnectionResult>("Connected", (result) => {
    Debug.Log($"Connected: {result.SessionId}");
});

// 인증 실패
connection.On<string>("AuthenticationFailed", (reason) => {
    Debug.LogError($"Auth failed: {reason}");
});
```

---

### 2.2 Real-time Updates

#### 자원 업데이트
**서버 → 클라이언트**:
```csharp
connection.On<ResourceUpdate>("OnResourceUpdate", (update) => {
    // update.ResourceType: GOLD, WOOD, STONE, FOOD
    // update.NewAmount: 현재 수량
    // update.Delta: 변화량
    UpdateResourceUI(update);
});
```

**DTO**:
```json
{
  "resourceType": "GOLD",
  "newAmount": 15250,
  "delta": 250,
  "timestamp": "datetime"
}
```

---

#### 건설 완료 알림
**서버 → 클라이언트**:
```csharp
connection.On<BuildingComplete>("OnBuildingComplete", (data) => {
    ShowBuildingCompleteAnimation(data.BuildingId);
});
```

**DTO**:
```json
{
  "buildingId": "guid",
  "buildingType": "BARRACKS",
  "level": 1,
  "position": { "x": 15, "y": 10 },
  "completedAt": "datetime"
}
```

---

#### 유닛 훈련 완료
**서버 → 클라이언트**:
```csharp
connection.On<UnitTrainingComplete>("OnUnitTrainingComplete", (data) => {
    AddUnitsToArmy(data.UnitType, data.Quantity);
});
```

---

#### 공격 알림
**서버 → 클라이언트**:
```csharp
connection.On<AttackNotification>("OnAttackReceived", (attack) => {
    ShowAttackAlert(attack);
});
```

**DTO**:
```json
{
  "attackId": "guid",
  "attackerName": "PlayerXYZ",
  "estimatedArrivalTime": "datetime",
  "attackForce": {
    "knights": 50,
    "archers": 30
  }
}
```

---

### 2.3 Client Commands

#### 즉시 완료 (Gem 사용)
**클라이언트 → 서버**:
```csharp
await connection.InvokeAsync<InstantCompleteResult>(
    "InstantCompleteBuilding",
    buildingId,
    useGems: true
);
```

**Response**:
```json
{
  "success": true,
  "gemCost": 10,
  "newGemBalance": 40
}
```

---

## 3. Error Code Definitions

### 3.1 Error Response Format

```json
{
  "success": false,
  "error": {
    "code": "INSUFFICIENT_RESOURCES",
    "message": "자원이 부족합니다",
    "details": {
      "required": { "gold": 1000 },
      "current": { "gold": 500 }
    },
    "timestamp": "datetime"
  }
}
```

---

### 3.2 Error Code Categories

#### Authentication (1xxx)
```yaml
1001: INVALID_CREDENTIALS
1002: TOKEN_EXPIRED
1003: INVALID_TOKEN
1004: USERNAME_ALREADY_EXISTS
1005: TOO_MANY_REQUESTS
1006: REFRESH_TOKEN_EXPIRED
```

#### Authorization (2xxx)
```yaml
2001: ACCESS_DENIED
2002: INSUFFICIENT_PERMISSIONS
2003: CASTLE_NOT_OWNED
```

#### Validation (3xxx)
```yaml
3001: INVALID_INPUT
3002: INVALID_POSITION
3003: INVALID_BUILDING_TYPE
3004: INVALID_UNIT_TYPE
```

#### Business Logic (4xxx)
```yaml
4001: INSUFFICIENT_RESOURCES
4002: MAX_LEVEL_REACHED
4003: REQUIREMENTS_NOT_MET
4004: POSITION_OCCUPIED
4005: UPGRADE_IN_PROGRESS
4006: TRAINING_QUEUE_FULL
4007: MAX_BUILDINGS_REACHED
```

#### System (5xxx)
```yaml
5001: INTERNAL_SERVER_ERROR
5002: DATABASE_ERROR
5003: SERVICE_UNAVAILABLE
```

---

## 4. API Versioning

### 4.1 Versioning Strategy

**URL Path Versioning**:
```
/api/v1/castles/{castleId}
/api/v2/castles/{castleId}
```

**지원 버전**:
- v1: 현재 안정 버전 (기본)
- v2: 베타 기능 포함 (선택)

---

### 4.2 Version Negotiation

**Request Header**:
```
API-Version: 1
Accept: application/json
```

**Response Header**:
```
API-Version: 1
Deprecated: false
Sunset: 2025-12-31 (v1 종료 예정일)
```

---

### 4.3 Breaking Changes Policy

1. 최소 3개월 사전 공지
2. Deprecated 헤더 포함
3. 이전 버전 6개월 지원 보장
4. 마이그레이션 가이드 제공

---

## 5. Client-Server Responsibility

### 5.1 Server Responsibilities

**Authority**:
- 모든 게임 로직 검증
- 자원 소모/획득 계산
- 시간 기반 진행 관리
- 치트 방지

**Data Persistence**:
- 데이터베이스 저장/조회
- 트랜잭션 무결성 보장
- 백업 및 복구

**Security**:
- 인증/인가 처리
- 입력 검증
- Rate Limiting

---

### 5.2 Client Responsibilities

**Presentation**:
- UI/UX 렌더링
- 애니메이션 재생
- 사운드 관리

**Prediction**:
- 낙관적 UI 업데이트 (Optimistic UI)
- 서버 응답 대기 중 로컬 상태 표시
- 서버 검증 실패 시 롤백

**Caching**:
- 정적 데이터 로컬 캐싱 (건물 정보, 유닛 스탯)
- 오프라인 모드 지원 (읽기 전용)

---

### 5.3 Validation Rules

#### Client-Side Validation (UX)
```csharp
// 빠른 피드백, 서버 재검증 필수
if (playerGold < buildingCost) {
    ShowError("자원 부족");
    return;
}
await BuildBuilding(buildingType);
```

#### Server-Side Validation (Authority)
```csharp
// 모든 요청 검증
if (!HasSufficientResources(userId, cost)) {
    return Error(INSUFFICIENT_RESOURCES);
}

// 시간 검증
if (constructionEndTime > DateTime.UtcNow) {
    return Error(CONSTRUCTION_NOT_FINISHED);
}

// 권한 검증
if (castle.UserId != requestUserId) {
    return Error(ACCESS_DENIED);
}
```

---

## 6. Rate Limiting

### 6.1 Rate Limit Rules

```yaml
authentication:
  endpoint: /api/auth/login
  limit: 5 requests / 15 minutes
  burst: 10

general_api:
  endpoint: /api/**
  limit: 100 requests / minute
  burst: 150

signalr_messages:
  limit: 30 messages / second
  burst: 50
```

---

### 6.2 Rate Limit Headers

**Response Headers**:
```
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 45
X-RateLimit-Reset: 1640000000 (Unix timestamp)
```

**429 Response**:
```json
{
  "success": false,
  "error": {
    "code": "TOO_MANY_REQUESTS",
    "message": "요청 한도 초과",
    "retryAfter": 60
  }
}
```

---

## 7. Pagination

### 7.1 Request Parameters

```
GET /api/quests?page=1&pageSize=20&sortBy=createdAt&order=desc
```

---

### 7.2 Response Format

```json
{
  "success": true,
  "data": {
    "items": [ /* 퀘스트 목록 */ ],
    "pagination": {
      "currentPage": 1,
      "pageSize": 20,
      "totalItems": 150,
      "totalPages": 8,
      "hasNext": true,
      "hasPrevious": false
    }
  }
}
```

---

## 8. WebSocket Reconnection

### 8.1 Reconnection Strategy

```csharp
// Unity Client
private async Task<bool> ReconnectWithBackoff() {
    int[] backoffMs = { 1000, 2000, 5000, 10000, 30000 };

    for (int i = 0; i < backoffMs.Length; i++) {
        await Task.Delay(backoffMs[i]);

        try {
            await connection.StartAsync();
            return true;
        } catch {
            Debug.Log($"Reconnect attempt {i+1} failed");
        }
    }

    return false;
}
```

---

### 8.2 State Synchronization

**연결 복구 후 상태 동기화**:
```csharp
connection.On<StateSyncData>("OnReconnected", async (syncData) => {
    // 서버에서 최신 상태 수신
    await ApplyServerState(syncData);

    // 로컬 변경사항 확인 후 재전송
    await ResendPendingActions();
});
```

---

## 9. Testing

### 9.1 Mock Server

**로컬 개발용 Mock Responses**:
```csharp
#if UNITY_EDITOR
public class MockApiClient : IApiClient {
    public async Task<Castle> GetCastle(Guid castleId) {
        await Task.Delay(100); // 네트워크 지연 시뮬레이션
        return MockData.SampleCastle;
    }
}
#endif
```

---

### 9.2 Integration Tests

```bash
# Server Integration Tests
dotnet test CastleLordTycoon.Tests --filter "Category=Integration"

# 주요 테스트 시나리오
- API 인증 흐름
- 건물 건설/업그레이드 시퀀스
- 자원 소비/생산 검증
- SignalR 연결 및 재연결
```

---

## 10. Documentation Tools

### 10.1 Swagger/OpenAPI

**접근 URL**: `https://api.castlelordtycoon.com/swagger`

**자동 생성 설정**:
```csharp
// Startup.cs
services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo {
        Title = "Castle Lord Tycoon API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
});
```

---

## 11. Change Log

### v1.0.0 (2025-01-15)
- 초기 API 버전 릴리스
- 기본 CRUD 엔드포인트
- SignalR 실시간 통신

### v1.1.0 (계획)
- 배틀 시스템 API 추가
- 길드 시스템 엔드포인트
- 랭킹 시스템 통합
