# Castle Lord Tycoon

중세시대 쿼터뷰 오픈월드 RPG + 자동 전투 모바일 게임

---

## 🚀 빠른 시작

### **1. Docker 컨테이너 시작** (PostgreSQL + Redis)

```bash
docker-compose --env-file .env.dev -f docker-compose.dev.yml up -d
```

### **2. 서버 실행**

```bash
cd CastleLordTycoon.Server
dotnet run
```

서버가 자동으로 `../.env.dev` 파일을 읽어 환경 변수를 로드합니다.

### **3. 서버 종료 시 Docker 정리**

```bash
docker-compose -f docker-compose.dev.yml down
```

---

## 📁 프로젝트 구조

```
Castle-Lord-Tycoon/
├── .env.dev                    # 개발 환경 변수 (Git 제외)
├── .env.prod                   # 프로덕션 환경 변수 (CI/CD용)
├── .env.example                # 템플릿
├── docker-compose.dev.yml      # 로컬 개발용
├── docker-compose.prod.yml     # 배포용
├── .gitignore                  # Git 제외 파일
├── CastleLordTycoon/           # Unity 클라이언트
│   ├── Assets/
│   │   ├── Scripts/            # C# 스크립트
│   │   │   ├── Core/           # 핵심 시스템
│   │   │   ├── Network/        # 서버 통신
│   │   │   ├── UI/             # UI 컨트롤러
│   │   │   ├── Combat/         # 전투 시스템
│   │   │   ├── Character/      # 영웅/캐릭터
│   │   │   ├── World/          # 월드 탐험
│   │   │   └── Data/           # 데이터 모델
│   │   ├── Sprites/            # 스프라이트
│   │   ├── Prefabs/            # 프리팹
│   │   └── Scenes/             # 씬
│   └── SETUP.md                # Unity 프로젝트 설정 가이드
└── CastleLordTycoon.Server/    # ASP.NET Core 서버
    ├── Program.cs              # .env.dev 자동 로드
    ├── appsettings.json        # 기본 설정
    └── appsettings.Development.json
```

---

## 🛠️ 기술 스택

- **Client**: Unity 2022.3.62f2 LTS + C# 9.0 (Android)
- **Server**: ASP.NET Core 9.0 + C# 12.0
- **Database**: PostgreSQL 17 (포트 10001)
- **Cache**: Redis 7 (포트 10002)
- **Deployment**: Docker + Docker Compose

---

## 🔐 환경 변수 관리

### **로컬 개발** (`.env.dev`)
- PostgreSQL: `localhost:10001`
- Redis: `localhost:10002`
- 환경: `Development`

### **프로덕션 배포** (`.env.prod`)
- PostgreSQL: `postgres:5432` (Docker 내부)
- Redis: `redis:6379` (Docker 내부)
- 환경: `Production`
- GitHub Actions Secrets로 관리

---

## 📊 데이터베이스 접속

### **pgAdmin 4 연결**
```
Host: localhost
Port: 10001
Database: castle_lord_tycoon
Username: sang
Password: (보안상 .env.dev 참조)
```

---

## 🎯 개발 워크플로우

### 서버 개발
1. Docker 컨테이너 시작
2. 서버 디렉토리에서 `dotnet run`
3. API 테스트 (`http://localhost:10010/swagger`)

### 클라이언트 개발
1. Unity 프로젝트 열기 (`CastleLordTycoon/`)
2. Unity 설정 가이드 참조 (`CastleLordTycoon/SETUP.md`)
3. Play 모드로 테스트

---

## 📝 문서

자세한 기획 및 기술 문서는 `docs/` 폴더를 참조하세요.

### 기획 문서
- **게임 개요**: `docs/00-overview/game-concept.md`
- **시스템 기획**: `docs/01-systems/`
- **데이터 밸런스**: `docs/02-data/`

### 기술 문서
- **아키텍처**: `docs/04-technical/architecture.md`
- **API 명세**: `docs/04-technical/client-server-contract.md`
- **데이터 구조**: `docs/04-technical/data-structures.md`
- **Google Sign-In 통합**: `docs/04-technical/google-signin-integration.md`
- **배포 가이드**: `docs/04-technical/deployment.md`

### Unity 클라이언트
- **프로젝트 설정**: `CastleLordTycoon/SETUP.md`
- **프로젝트 개요**: `.claude/project-context.md`

---

## 🔧 트러블슈팅

### **PostgreSQL 비밀번호 인증 실패**
```bash
# Docker 볼륨 초기화
docker-compose -f docker-compose.dev.yml down -v
docker-compose --env-file .env.dev -f docker-compose.dev.yml up -d
```

### **.env.dev 파일이 없음**
```bash
# .env.example을 복사하여 .env.dev 생성
cp .env.example .env.dev
# 실제 비밀번호로 수정
```
