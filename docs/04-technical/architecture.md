# 시스템 아키텍처

## 전체 구조 (Client-Server)

```
┌─────────────────────────────────────────────────┐
│                UNITY CLIENT                      │
│            (Android - C# 9.0)                   │
│                                                  │
│  ┌─────────────────────────────────────────┐   │
│  │      Presentation Layer                  │   │
│  │  (UI, Rendering, Input, Animation)      │   │
│  └──────────────┬───────────────────────────┘   │
│                 │                                │
│  ┌──────────────▼───────────────────────────┐   │
│  │      Client Game Logic                   │   │
│  │  (Movement Prediction, UI State,         │   │
│  │   Local Validation, Cache)               │   │
│  └──────────────┬───────────────────────────┘   │
│                 │                                │
│  ┌──────────────▼───────────────────────────┐   │
│  │      Network Layer                       │   │
│  │  - SignalR Client (WebSocket)            │   │
│  │  - HTTP Client (REST API)                │   │
│  └──────────────┬───────────────────────────┘   │
└─────────────────┼───────────────────────────────┘
                  │
          HTTPS + WebSocket (SSL)
                  │
┌─────────────────▼───────────────────────────────┐
│              NGINX (Reverse Proxy)               │
│  - SSL Termination (Let's Encrypt)              │
│  - Load Balancing (Future)                      │
│  - castle.yourdomain.com                        │
└─────────────────┬───────────────────────────────┘
                  │
┌─────────────────▼───────────────────────────────┐
│          ASP.NET CORE GAME SERVER                │
│            (C# 12.0 - .NET 8)                   │
│                                                  │
│  ┌─────────────────────────────────────────┐   │
│  │         API Layer                        │   │
│  │  - REST Controllers                      │   │
│  │  - SignalR Hubs                          │   │
│  │  - JWT Authentication                    │   │
│  │  - Swagger Documentation                 │   │
│  └──────────────┬───────────────────────────┘   │
│                 │                                │
│  ┌──────────────▼───────────────────────────┐   │
│  │      Server Game Logic (Authority)       │   │
│  │  - Combat Calculation                    │   │
│  │  - Territory Validation                  │   │
│  │  - Drop/Reward RNG                       │   │
│  │  - Anti-Cheat Validation                 │   │
│  └──────────────┬───────────────────────────┘   │
│                 │                                │
│  ┌──────────────▼───────────────────────────┐   │
│  │      Services Layer                      │   │
│  │  - AuthService (JWT)                     │   │
│  │  - CombatService                         │   │
│  │  - TerritoryService                      │   │
│  │  - PlayerService                         │   │
│  │  - LogQueueService (비동기)              │   │
│  └──────────────┬───────────────────────────┘   │
│                 │                                │
│  ┌──────────────▼───────────────────────────┐   │
│  │      Data Access Layer                   │   │
│  │  - EF Core DbContext                     │   │
│  │  - Repository Pattern                    │   │
│  │  - Redis Cache                           │   │
│  └──────────────┬───────────────────────────┘   │
│                 │                                │
│  ┌──────────────▼───────────────────────────┐   │
│  │      Background Workers                  │   │
│  │  - LogProcessorWorker (MongoDB)          │   │
│  │  - SessionCleanupWorker                  │   │
│  └──────────────────────────────────────────┘   │
└─────┬────────────┬────────────┬─────────────────┘
      │            │            │
┌─────▼──────┐ ┌──▼──────┐ ┌───▼────────┐
│ PostgreSQL │ │  Redis  │ │  MongoDB   │
│     17     │ │    7    │ │     7      │
│            │ │         │ │            │
│ Game State │ │  Cache  │ │   Logs     │
└────────────┘ └─────────┘ └────────────┘
```

## 클라이언트 아키텍처 (Unity)

### 레이어 구조

```yaml
presentation_layer:
  responsibility: "UI 렌더링, 사용자 입력, 애니메이션"
  components:
    - Canvas (UI Toolkit)
    - 아이소메트릭 카메라
    - 타일맵 렌더러
    - 파티클 시스템
    - 애니메이션 컨트롤러

client_logic_layer:
  responsibility: "로컬 게임 상태, 예측, 캐싱"
  components:
    - GameStateManager (서버 동기화 상태)
    - MovementPredictor (부드러운 이동)
    - UIStateController
    - LocalCache (최근 조회 데이터)

  note: "민감한 계산은 하지 않음 (서버 검증 필요)"

network_layer:
  responsibility: "서버 통신"
  components:
    http_client:
      - UnityWebRequest
      - REST API 호출
      - NSwag 자동 생성 클라이언트

    signalr_client:
      - SignalR Unity Client
      - 실시간 이벤트 수신
      - 자동 재연결

data_layer:
  responsibility: "로컬 데이터 관리"
  components:
    - PlayerPrefs (설정)
    - 서버 응답 캐시
    - 임시 전투 상태
```

### 클라이언트 데이터 흐름

```
사용자 입력 (터치/키보드)
    ↓
Input Handler → 입력 검증
    ↓
Client Prediction (움직임 예측)
    ↓
Server API 호출 (검증 요청)
    ↓
Server Response → 상태 동기화
    ↓
UI Update + Rendering
```

### 클라이언트 책임

```yaml
permitted:
  - 렌더링 및 애니메이션
  - 사용자 입력 처리
  - 이동 예측 (서버 검증 전)
  - UI 상태 관리
  - 로컬 캐싱

forbidden:
  - 전투 데미지 계산 (서버 전용)
  - 드랍 아이템 결정 (서버 RNG)
  - 경험치 분배 계산
  - 골드/아이템 추가
  - 영토 소유권 변경
```

## 서버 아키텍처 (ASP.NET Core)

### 프로젝트 구조

```
CastleLordTycoon.Server/
├── Controllers/              # REST API 엔드포인트
│   ├── AuthController.cs     # 로그인, 회원가입, JWT
│   ├── PlayerController.cs   # 플레이어 정보 조회/수정
│   ├── CombatController.cs   # 전투 시작/종료
│   ├── TerritoryController.cs # 깃발 설치, 영토 조회
│   ├── HeroController.cs     # 영웅 관리
│   └── AdminController.cs    # 관리자 기능
│
├── Hubs/                     # SignalR 실시간 통신
│   └── GameHub.cs            # 이벤트 push, 공지
│
├── Services/                 # 비즈니스 로직
│   ├── Auth/
│   │   ├── IAuthService.cs
│   │   └── AuthService.cs    # JWT 생성, 검증
│   ├── Game/
│   │   ├── ICombatService.cs
│   │   ├── CombatService.cs  # 전투 계산 (서버 권한)
│   │   ├── ITerritoryService.cs
│   │   └── TerritoryService.cs
│   ├── Player/
│   │   └── PlayerService.cs
│   └── Logging/
│       ├── ILogQueue.cs
│       └── LogQueueService.cs # 비동기 로그 큐
│
├── Workers/                  # Background Services
│   └── LogProcessorWorker.cs # MongoDB 로그 저장
│
├── Data/                     # 데이터 액세스
│   ├── ApplicationDbContext.cs # EF Core
│   ├── Repositories/
│   │   ├── IPlayerRepository.cs
│   │   ├── PlayerRepository.cs
│   │   └── ...
│   └── Entities/             # DB 엔티티 (C# 클래스)
│
├── Models/                   # DTO (Data Transfer Objects)
│   ├── Requests/
│   └── Responses/
│
├── Middleware/
│   ├── AuthenticationMiddleware.cs
│   ├── ExceptionHandlerMiddleware.cs
│   └── RateLimitMiddleware.cs
│
└── Program.cs               # 진입점, DI 설정
```

### 서버 책임 (Server Authority)

```yaml
authentication:
  - JWT 토큰 발급/검증
  - 세션 관리
  - 권한 확인

game_logic:
  combat:
    - 데미지 계산 (공격-방어, 속성 상성)
    - 크리티컬/회피 판정
    - 전투 결과 확정
    - 경험치 분배
    - 드랍 아이템 결정 (RNG 서버)

  territory:
    - 깃발 설치 검증 (위치, 소유권, 비용)
    - 영토 편입 계산
    - 마을 자동 점령 판정

  progression:
    - 레벨업 처리
    - 계급 상승 판정
    - 보상 지급

anti_cheat:
  - 이동 속도 검증
  - 액션 레이트 리밋
  - 데이터 일관성 체크
  - 불가능한 상태 탐지

data_persistence:
  - PostgreSQL 영구 저장
  - Redis 캐싱
  - MongoDB 로그 저장
```

## 네트워크 통신

### REST API (HTTP/HTTPS)

```yaml
purpose: "상태 변경, 데이터 조회"

endpoints:
  auth:
    - POST /api/auth/register  # 회원가입
    - POST /api/auth/login     # 로그인
    - POST /api/auth/refresh   # 토큰 갱신

  player:
    - GET /api/player          # 플레이어 정보
    - PUT /api/player/position # 위치 동기화
    - GET /api/player/heroes   # 영웅 목록

  combat:
    - POST /api/combat/start   # 전투 시작 (서버 계산)
    - POST /api/combat/end     # 전투 종료 (보상 처리)

  territory:
    - POST /api/territory/flag # 깃발 설치
    - GET /api/territory       # 영토 조회

  admin:
    - POST /api/admin/announcement # 공지사항

security:
  - JWT Bearer Token (Authorization: Bearer {token})
  - HTTPS 암호화
  - Rate Limiting (10 req/sec per user)
```

### SignalR (WebSocket)

```yaml
purpose: "서버→클라이언트 실시간 push"

connection_lifecycle:
  connect: "게임 시작 시 WebSocket 연결"
  maintain: "게임 플레이 중 유지 (heartbeat 30초)"
  disconnect: "백그라운드 전환 시 자동 해제"
  reconnect: "포그라운드 복귀 시 자동 재연결"

server_to_client_events:
  - OnAnnouncement(message)        # 운영 공지
  - OnCombatStart(enemyData)       # 조우 발생
  - OnTownConquered(townId)        # 마을 점령 (다른 플레이어)
  - OnRankUp(newRank)              # 계급 상승 알림
  - OnEventTrigger(eventData)      # 이벤트 발생

client_to_server_rpc:
  - SendHeartbeat()                # 연결 유지
  - ReportPosition(x, y)           # 주기적 위치 보고 (5-10초)

overhead:
  heartbeat: "30초마다 수십 바이트"
  position_sync: "5-10초마다 수백 바이트"
  total: "매우 낮음 (배터리 효율적)"
```

## 데이터베이스 설계

### PostgreSQL (Primary DB)

```yaml
purpose: "영구 게임 상태 저장"

schema:
  players:
    - id (UUID, PK)
    - username (VARCHAR, UNIQUE)
    - password_hash (VARCHAR)
    - email (VARCHAR)
    - created_at, updated_at

  heroes:
    - id (UUID, PK)
    - player_id (FK → players)
    - template_id (VARCHAR) # 영웅 종류
    - star_grade (INT, 1-6)
    - level, current_exp
    - stats (JSONB)
    - unique_effects (JSONB)

  equipment:
    - id (UUID, PK)
    - player_id (FK)
    - template_id (VARCHAR)
    - grade (VARCHAR, C/UC/R/H/L)
    - base_stat (JSONB)
    - options (JSONB)

  territories:
    - id (UUID, PK)
    - player_id (FK)
    - flags (JSONB)  # Flag 배열
    - owned_tiles (TEXT[])  # "x,y" 배열
    - connected_towns (UUID[])

  towns:
    - id (UUID, PK)
    - name, position
    - owned_by (FK → players, NULL 가능)
    - conquered_at

performance:
  indexes:
    - player_id (모든 FK)
    - template_id (조회 빈번)
    - created_at (정렬용)

  partitioning: "나중에 고려 (플레이어 수 증가 시)"
```

### Redis (Cache)

```yaml
purpose: "세션, 빠른 조회 캐싱"

data_types:
  session:
    key: "session:{player_id}"
    type: Hash
    data:
      - current_position: "x,y"
      - online_status: bool
      - last_activity: timestamp
    ttl: 1 hour (활동 시 갱신)

  template_cache:
    key: "template:monster:{id}"
    type: String (JSON)
    data: 몬스터/장비 템플릿
    ttl: 24 hours (거의 변경 없음)

  combat_state:
    key: "combat:{combat_id}"
    type: Hash
    data: 임시 전투 상태
    ttl: 10 minutes

eviction_policy: "allkeys-lru"
```

### MongoDB (Logging)

```yaml
purpose: "게임 이벤트 로그"

collections:
  game_events:
    document:
      timestamp: ISODate
      player_id: UUID
      event_type: String (combat, territory, item_drop)
      event_data: Object
      session_id: UUID

  error_logs:
    document:
      timestamp: ISODate
      level: String (error, warning, info)
      message: String
      stack_trace: String
      context: Object

indexes:
  - {player_id: 1, timestamp: -1}
  - {event_type: 1, timestamp: -1}
  - {timestamp: -1} (TTL index, 90일 후 삭제)
```

## 성능 최적화

### 클라이언트 (Unity)

```yaml
rendering:
  - Chunk System (20×20 타일 청크)
  - Frustum Culling (화면 밖 비활성화)
  - Object Pooling (몬스터, 이펙트)
  - Sprite Atlas (텍스처 배칭)
  - 안개 레이어 캐싱

memory:
  - Addressables (동적 로딩)
  - 지역별 에셋 분할
  - 사용하지 않는 에셋 언로드

network:
  - 위치 동기화: 5-10초 주기 (필요 시에만)
  - 압축 (Protocol Buffers 고려)
  - 배칭 (여러 요청 묶기)

target:
  fps: 60 (안드로이드 기준)
  memory: "<500MB"
  startup: "<3초"
```

### 서버 (ASP.NET Core)

```yaml
caching:
  redis:
    - 플레이어 세션 (1시간 TTL)
    - 몬스터/장비 템플릿 (24시간 TTL)
    - 지역 데이터 (무기한, 변경 시 무효화)

database:
  ef_core:
    - 쿼리 최적화 (Include, AsNoTracking)
    - Bulk Operations (다수 데이터 삽입)
    - Connection Pooling (기본 설정)

  postgresql:
    - 적절한 인덱스
    - Prepared Statements
    - Vacuum (주기적 정리)

async_processing:
  - 로그는 큐에 넣고 BackgroundWorker 처리
  - 게임 로직은 큐 대기 없이 즉시 응답
  - Channel<T> 사용 (고성능 큐)

target:
  response_time: "<200ms (API)"
  concurrent_users: "1000명 (단일 서버)"
  throughput: ">1000 req/sec"
```

## 보안 아키텍처

### 인증 & 인가

```yaml
authentication:
  method: "JWT (JSON Web Token)"

  token_structure:
    header: {alg: "HS256", typ: "JWT"}
    payload:
      sub: player_id
      username: String
      exp: 1 hour
      iat: issued_at
    signature: HMAC-SHA256(secret)

  flow:
    1. 클라이언트: POST /api/auth/login {username, password}
    2. 서버: 비밀번호 검증 (BCrypt)
    3. 서버: JWT 발급 (1시간 유효)
    4. 클라이언트: 저장 (PlayerPrefs)
    5. 이후 모든 요청: Authorization: Bearer {token}

refresh_token:
  - 7일 유효
  - HttpOnly Cookie (보안)
  - Refresh 시 새 Access Token 발급
```

### 데이터 검증

```yaml
client_input_validation:
  - 서버에서 모든 입력 재검증
  - 범위 체크 (이동 거리, 시간)
  - 레이트 리밋 (액션 빈도)

server_authority:
  - 모든 민감한 계산은 서버에서만
  - 클라이언트는 결과만 수신
  - 불일치 발견 시 서버 값으로 강제 동기화

anti_cheat:
  position:
    - 이동 속도 검증 (최대 속도 초과 탐지)
    - 텔레포트 탐지 (거리 제한)

  action:
    - 쿨다운 검증
    - 불가능한 액션 탐지 (예: 사망 상태에서 공격)

  resource:
    - 골드/아이템 변경 감시
    - 통계적 이상치 탐지
```

## 확장성 고려사항

### 수평 확장 (Horizontal Scaling)

```yaml
phase_2_architecture:
  load_balancer:
    - Nginx 또는 Ocelot
    - Round-robin / Least connections

  stateless_servers:
    - 세션은 Redis에 저장
    - 서버는 무상태 (어느 서버든 처리 가능)

  database:
    - PostgreSQL Read Replica (읽기 부하 분산)
    - Redis Cluster (캐시 확장)

  cdn:
    - 정적 에셋 (이미지, 사운드)
    - CDN 배포로 다운로드 속도 향상
```

### 모니터링 & 관찰성

```yaml
metrics:
  - 서버 CPU, 메모리 사용률
  - API 응답 시간 (p50, p95, p99)
  - 동시 접속자 수
  - DB 쿼리 성능

logging:
  mvp: "Serilog → MongoDB"
  phase_2: "ELK Stack (Elasticsearch, Kibana)"

alerting:
  - 에러율 > 1%
  - 응답 시간 > 500ms
  - CPU > 80%
  - 디스크 공간 < 20%
```

---

**최종 수정**: 2025-10-19
**상태**: 🟢 확정
**작성자**: Claude + SangHyeok
