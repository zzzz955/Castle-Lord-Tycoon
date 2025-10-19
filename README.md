# Castle Lord Tycoon

중세시대 쿼터뷰 오픈월드 RPG + 자동 전투 모바일 게임

---

## 🚀 빠른 시작

### 1. 환경 변수 설정

프로젝트 루트에 `.env.dev` 또는 `.env.prod` 파일이 있는지 확인하세요.

```bash
# .env.example을 복사하여 시작 (이미 생성되어 있음)
# .env.dev  - 개발 환경용
# .env.prod - 프로덕션 환경용
```

### 2. 개발 환경 실행

```bash
# Docker 컨테이너 시작 (PostgreSQL + Redis)
docker-compose --env-file .env.dev -f docker-compose.dev.yml up -d

# ASP.NET 서버 실행 (Native)
cd CastleLordTycoon.Server
dotnet watch run

# Unity Editor에서 클라이언트 실행
```

### 3. 프로덕션 환경 실행 (로컬 테스트)

```bash
# 전체 스택 Docker로 실행
docker-compose --env-file .env.prod -f docker-compose.prod.yml up -d --build
```

### 4. 컨테이너 종료

```bash
# 개발 환경
docker-compose -f docker-compose.dev.yml down

# 프로덕션 환경
docker-compose -f docker-compose.prod.yml down
```

---

## 📁 프로젝트 구조

```
Castle-Lord-Tycoon/
├── .env.dev                    # 개발 환경 변수
├── .env.prod                   # 프로덕션 환경 변수
├── .env.example                # 환경 변수 템플릿
├── docker-compose.dev.yml      # 개발용 Docker 설정
├── docker-compose.prod.yml     # 프로덕션용 Docker 설정
├── CastleLordTycoon.Server/    # ASP.NET Core 서버
│   ├── appsettings.json
│   └── appsettings.Development.json
└── docs/                       # 기획 문서
    ├── 00-overview/
    ├── 01-systems/
    ├── 02-data/
    ├── 03-ui/
    └── 04-technical/
```

---

## 🛠️ 기술 스택

- **Client**: Unity 2022.3.62f2 LTS + C# 9.0 (Android)
- **Server**: ASP.NET Core 8.0 + C# 12.0
- **Database**: PostgreSQL 17 + Redis 7
- **Deployment**: Docker + Docker Compose + Nginx

---

## 🔐 환경 변수 관리

### 개발 환경 (.env.dev)
- `POSTGRES_PORT=10001` (로컬 접근)
- `REDIS_PORT=10002` (로컬 접근)
- 모든 컨테이너와 서버가 **localhost** 사용

### 프로덕션 환경 (.env.prod)
- `POSTGRES_HOST=postgres` (Docker 내부 통신)
- `REDIS_HOST=redis` (Docker 내부 통신)
- 모든 서비스가 **Docker 네트워크** 사용

---

## 📊 데이터베이스 접속

### pgAdmin 4 연결 설정

**개발 환경**:
```
Host: localhost
Port: 10001
Database: castle_lord_tycoon
Username: sang
Password: fostj137sw!@
```

**프로덕션 환경** (Docker 내부):
```
Host: postgres
Port: 5432
Database: castle_lord_tycoon
Username: sang
Password: (프로덕션 비밀번호)
```

---

## 🎯 다음 단계

1. ASP.NET Core 프로젝트 생성
2. Unity 프로젝트 생성
3. Entity Framework Core 설정
4. API 개발 시작

---

## 📝 문서

자세한 기획 및 기술 문서는 `docs/` 폴더를 참조하세요.

- **게임 개요**: `docs/00-overview/game-concept.md`
- **시스템 기획**: `docs/01-systems/`
- **데이터 밸런스**: `docs/02-data/`
- **기술 문서**: `docs/04-technical/`
