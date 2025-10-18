# Deployment Guide

## Overview

Castle Lord Tycoon 배포 및 운영 가이드입니다.

**Infrastructure**:
- Server: ASP.NET Core 8.0
- Database: PostgreSQL 17
- Cache: Redis 7
- Reverse Proxy: Nginx (기존 서버 사용)
- Containerization: Docker + Docker Compose
- SSL: Let's Encrypt (기존 서버 Certbot 사용)
- CI/CD: GitHub Actions

---

## 1. Architecture Overview

```
┌─────────────────────────────────────────────────────────┐
│                     Internet                            │
└────────────────────┬────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────┐
│  기존 Nginx (Reverse Proxy + SSL Termination)          │
│  - castle.yourdomain.com → Game Server                 │
└────────────────────┬────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────┐
│  Game Server Container (Docker)                         │
│  - REST API: /api/*                                     │
│  - SignalR WebSocket: /hub/*                            │
│  - External Port: 10010                                 │
│  - Internal Port: 8080                                  │
└────────┬────────────────────────┬───────────────────────┘
         │                        │
         ▼                        ▼
┌────────────────────┐   ┌──────────────────────┐
│  PostgreSQL 17     │   │  Redis 7             │
│  External: 10001   │   │  External: 10002     │
│  Internal: 5432    │   │  Internal: 6379      │
└────────────────────┘   └──────────────────────┘
```

---

## 2. Docker Compose Configuration

### 2.1 docker-compose.yml

```yaml
version: '3.8'

services:
  # PostgreSQL Database
  postgres:
    image: postgres:17-alpine
    container_name: castle-db
    restart: unless-stopped
    environment:
      POSTGRES_DB: ${DB_NAME:-castlelord}
      POSTGRES_USER: ${DB_USER:-admin}
      POSTGRES_PASSWORD: ${DB_PASSWORD}
      PGDATA: /var/lib/postgresql/data/pgdata
    volumes:
      - postgres-data:/var/lib/postgresql/data
      - ./scripts/init-db.sql:/docker-entrypoint-initdb.d/init.sql
    ports:
      - "${POSTGRES_PORT:-10001}:5432"
    networks:
      - castle-network
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U ${DB_USER:-admin}"]
      interval: 10s
      timeout: 5s
      retries: 5

  # Redis Cache
  redis:
    image: redis:7-alpine
    container_name: castle-redis
    restart: unless-stopped
    ports:
      - "${REDIS_PORT:-10002}:6379"
    networks:
      - castle-network
    volumes:
      - redis-data:/data
    command: redis-server --appendonly yes

  # Backend API Server
  api:
    build:
      context: ./server
      dockerfile: Dockerfile
    container_name: castle-api
    restart: unless-stopped
    environment:
      - ASPNETCORE_ENVIRONMENT=${ENVIRONMENT:-Production}
      - ASPNETCORE_URLS=http://+:8080
      - ConnectionStrings__DefaultConnection=Host=postgres;Port=5432;Database=${DB_NAME:-castlelord};Username=${DB_USER:-admin};Password=${DB_PASSWORD}
      - JWT__Secret=${JWT_SECRET}
      - JWT__Issuer=${JWT_ISSUER:-CastleLordTycoon}
      - JWT__Audience=${JWT_AUDIENCE:-CastleLordTycoonClient}
      - JWT__ExpirationMinutes=${JWT_EXPIRATION:-60}
      - Redis__ConnectionString=redis:6379
    ports:
      - "${API_PORT:-10010}:8080"
    depends_on:
      postgres:
        condition: service_healthy
      redis:
        condition: service_started
    networks:
      - castle-network
    volumes:
      - ./logs:/app/logs
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:8080/health"]
      interval: 30s
      timeout: 10s
      retries: 3

volumes:
  postgres-data:
  redis-data:

networks:
  castle-network:
    driver: bridge
```

---

### 2.2 Backend Dockerfile

```dockerfile
# server/Dockerfile

# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["CastleLordTycoon.API/CastleLordTycoon.API.csproj", "CastleLordTycoon.API/"]
COPY ["CastleLordTycoon.Core/CastleLordTycoon.Core.csproj", "CastleLordTycoon.Core/"]
COPY ["CastleLordTycoon.Infrastructure/CastleLordTycoon.Infrastructure.csproj", "CastleLordTycoon.Infrastructure/"]

RUN dotnet restore "CastleLordTycoon.API/CastleLordTycoon.API.csproj"

# Copy source code
COPY . .

# Build application
WORKDIR "/src/CastleLordTycoon.API"
RUN dotnet build "CastleLordTycoon.API.csproj" -c Release -o /app/build

# Publish Stage
FROM build AS publish
RUN dotnet publish "CastleLordTycoon.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install curl for healthcheck
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published application
COPY --from=publish /app/publish .

# Create logs directory
RUN mkdir -p /app/logs

# Expose port
EXPOSE 8080

# Run application
ENTRYPOINT ["dotnet", "CastleLordTycoon.API.dll"]
```

---

## 3. 기존 Nginx 설정 업데이트

### 3.1 서브도메인 설정 파일 생성

기존 Nginx 서버에 새 게임용 설정 추가:

**파일**: `/etc/nginx/sites-available/castle-lord-tycoon`

```nginx
server {
    listen 443 ssl http2;
    server_name castle.yourdomain.com;

    # SSL 인증서 (Let's Encrypt)
    ssl_certificate /etc/letsencrypt/live/castle.yourdomain.com/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/castle.yourdomain.com/privkey.pem;

    # SSL 설정
    ssl_protocols TLSv1.2 TLSv1.3;
    ssl_ciphers HIGH:!aNULL:!MD5;
    ssl_prefer_server_ciphers on;

    # Security Headers
    add_header Strict-Transport-Security "max-age=31536000; includeSubDomains" always;
    add_header X-Frame-Options "DENY" always;
    add_header X-Content-Type-Options "nosniff" always;
    add_header X-XSS-Protection "1; mode=block" always;

    # REST API
    location /api/ {
        proxy_pass http://localhost:10010/api/;
        proxy_http_version 1.1;

        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;

        proxy_connect_timeout 60s;
        proxy_send_timeout 60s;
        proxy_read_timeout 60s;
    }

    # SignalR WebSocket
    location /hub/ {
        proxy_pass http://localhost:10010/hub/;
        proxy_http_version 1.1;

        # WebSocket 필수 헤더
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection "upgrade";
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;

        # WebSocket 타임아웃 (장시간 유지)
        proxy_connect_timeout 7d;
        proxy_send_timeout 7d;
        proxy_read_timeout 7d;
    }

    # Swagger (개발 환경)
    location /swagger/ {
        proxy_pass http://localhost:10010/swagger/;
        proxy_http_version 1.1;
        proxy_set_header Host $host;
    }

    # Health Check
    location /health {
        proxy_pass http://localhost:10010/health;
        access_log off;
    }
}

# HTTP → HTTPS 리다이렉트
server {
    listen 80;
    server_name castle.yourdomain.com;
    return 301 https://$server_name$request_uri;
}
```

### 3.2 설정 활성화

```bash
# 심볼릭 링크 생성
sudo ln -s /etc/nginx/sites-available/castle-lord-tycoon /etc/nginx/sites-enabled/

# 설정 테스트
sudo nginx -t

# Nginx 재시작
sudo systemctl reload nginx
```

### 3.3 Let's Encrypt SSL 인증서 발급

```bash
# 새 서브도메인 인증서 발급
sudo certbot --nginx -d castle.yourdomain.com

# 자동 갱신 확인
sudo certbot renew --dry-run
```

**참고**: 기존 게임의 인증서와 독립적으로 관리됩니다.

---

## 4. Environment Variables

### 4.1 .env File (Production)

```bash
# .env (서버에 직접 배치, Git에는 커밋하지 않음)

# Environment
ENVIRONMENT=Production

# 포트 설정 (10000번대 사용)
POSTGRES_PORT=10001
REDIS_PORT=10002
API_PORT=10010

# Database
DB_NAME=castlelord_prod
DB_USER=admin
DB_PASSWORD=<강력한_비밀번호>

# JWT
JWT_SECRET=<256비트_이상_랜덤_문자열>
JWT_ISSUER=CastleLordTycoon
JWT_AUDIENCE=CastleLordTycoonClient
JWT_EXPIRATION=60

# Admin
ADMIN_EMAIL=admin@castlelordtycoon.com
```

**비밀번호 생성 예시**:
```bash
# 랜덤 비밀번호 생성
openssl rand -base64 32
```

---

### 4.2 .env.example (Git 커밋용)

```bash
# .env.example (템플릿)

ENVIRONMENT=Production

# 포트 설정 (10000번대 사용)
POSTGRES_PORT=10001
REDIS_PORT=10002
API_PORT=10010

# Database
DB_NAME=castlelord_prod
DB_USER=admin
DB_PASSWORD=<CHANGE_ME>

# JWT
JWT_SECRET=<CHANGE_ME>
JWT_ISSUER=CastleLordTycoon
JWT_AUDIENCE=CastleLordTycoonClient
JWT_EXPIRATION=60

# Admin
ADMIN_EMAIL=admin@example.com
```

---

## 5. GitHub Actions CI/CD

### 5.1 .github/workflows/deploy.yml

```yaml
name: Deploy to Production

on:
  push:
    branches:
      - main
  workflow_dispatch:

jobs:
  test:
    name: Run Tests
    runs-on: ubuntu-latest

    steps:
      - name: Checkout code
        uses: actions/checkout@v3

      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'

      - name: Restore dependencies
        run: dotnet restore server/CastleLordTycoon.sln

      - name: Build
        run: dotnet build server/CastleLordTycoon.sln --configuration Release --no-restore

      - name: Run unit tests
        run: dotnet test server/CastleLordTycoon.Tests/CastleLordTycoon.Tests.csproj --configuration Release --no-build --verbosity normal

  build-and-push:
    name: Build and Push Docker Image
    runs-on: ubuntu-latest
    needs: test

    steps:
      - name: Checkout code
        uses: actions/checkout@v3

      - name: Set up Docker Buildx
        uses: docker/setup-buildx-action@v2

      - name: Login to Docker Hub
        uses: docker/login-action@v2
        with:
          username: ${{ secrets.DOCKER_USERNAME }}
          password: ${{ secrets.DOCKER_PASSWORD }}

      - name: Build and push
        uses: docker/build-push-action@v4
        with:
          context: ./server
          file: ./server/Dockerfile
          push: true
          tags: |
            ${{ secrets.DOCKER_USERNAME }}/castle-lord-api:latest
            ${{ secrets.DOCKER_USERNAME }}/castle-lord-api:${{ github.sha }}
          cache-from: type=registry,ref=${{ secrets.DOCKER_USERNAME }}/castle-lord-api:buildcache
          cache-to: type=registry,ref=${{ secrets.DOCKER_USERNAME }}/castle-lord-api:buildcache,mode=max

  deploy:
    name: Deploy to Server
    runs-on: ubuntu-latest
    needs: build-and-push

    steps:
      - name: Deploy via SSH
        uses: appleboy/ssh-action@master
        with:
          host: ${{ secrets.SERVER_HOST }}
          username: ${{ secrets.SERVER_USER }}
          key: ${{ secrets.SSH_PRIVATE_KEY }}
          script: |
            cd /opt/castle-lord-tycoon
            docker-compose pull api
            docker-compose up -d api
            docker image prune -f
```

---

### 5.2 Required GitHub Secrets

Settings → Secrets and variables → Actions에서 설정:

```yaml
DOCKER_USERNAME: Docker Hub 사용자명
DOCKER_PASSWORD: Docker Hub 액세스 토큰
SERVER_HOST: 서버 IP 주소
SERVER_USER: SSH 사용자명 (예: ubuntu)
SSH_PRIVATE_KEY: SSH 개인키 (id_rsa 내용)
```

---

## 6. Server Setup Guide

### 6.1 Initial Server Configuration

```bash
# Ubuntu 22.04 LTS 기준

# 시스템 업데이트
sudo apt update && sudo apt upgrade -y

# Docker 설치
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh
sudo usermod -aG docker $USER

# Docker Compose 설치
sudo apt install docker-compose-plugin -y

# 프로젝트 디렉토리 생성
sudo mkdir -p /opt/castle-lord-tycoon
sudo chown $USER:$USER /opt/castle-lord-tycoon
cd /opt/castle-lord-tycoon

# Git 클론
git clone https://github.com/yourusername/castle-lord-tycoon.git .

# 환경 변수 설정
cp .env.example .env
nano .env  # 비밀번호 설정

# 방화벽 설정 (10000번대 포트 추가)
sudo ufw allow 22/tcp    # SSH
sudo ufw allow 80/tcp    # HTTP
sudo ufw allow 443/tcp   # HTTPS
sudo ufw allow 10001/tcp # PostgreSQL (필요시)
sudo ufw allow 10002/tcp # Redis (필요시)
sudo ufw allow 10010/tcp # Game Server (필요시)
sudo ufw enable
```

---

### 6.2 Database Initialization

```sql
-- scripts/init-db.sql

-- Enable UUID extension
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Create tables (실제로는 EF Core Migration 사용)
-- 이 스크립트는 초기 권한 설정용

-- Create read-only user for analytics
CREATE USER analytics_user WITH PASSWORD '<analytics_password>';
GRANT CONNECT ON DATABASE castlelord_prod TO analytics_user;
GRANT SELECT ON ALL TABLES IN SCHEMA public TO analytics_user;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT SELECT ON TABLES TO analytics_user;
```

---

## 7. Local Development Setup

### 7.1 docker-compose.dev.yml

```yaml
version: '3.8'

services:
  postgres:
    image: postgres:17-alpine
    container_name: castle-db-dev
    environment:
      POSTGRES_DB: castlelord_dev
      POSTGRES_USER: dev
      POSTGRES_PASSWORD: dev123
    ports:
      - "5432:5432"
    volumes:
      - postgres-dev-data:/var/lib/postgresql/data

  redis:
    image: redis:7-alpine
    container_name: castle-redis-dev
    ports:
      - "6379:6379"

volumes:
  postgres-dev-data:
```

**실행**:
```bash
docker-compose -f docker-compose.dev.yml up -d

# 백엔드 실행 (Visual Studio / Rider)
cd server/CastleLordTycoon.API
dotnet run

# Unity 클라이언트
# Unity Editor에서 Play 버튼 클릭
```

---

### 7.2 appsettings.Development.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=castlelord_dev;Username=dev;Password=dev123"
  },
  "JWT": {
    "Secret": "dev-secret-key-minimum-32-characters-long-12345678",
    "Issuer": "CastleLordTycoon",
    "Audience": "CastleLordTycoonClient",
    "ExpirationMinutes": 1440
  },
  "AllowedHosts": "*",
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:3000",
      "http://127.0.0.1:3000"
    ]
  }
}
```

---

## 8. Deployment Checklist

### 8.1 Pre-Deployment

- [ ] 모든 테스트 통과 확인
- [ ] 환경 변수 설정 확인 (.env 파일)
- [ ] 데이터베이스 백업
- [ ] SSL 인증서 유효 확인
- [ ] 기존 Nginx 설정 검증: `sudo nginx -t`
- [ ] Docker 이미지 빌드 성공 확인
- [ ] 보안 취약점 스캔
- [ ] 포트 충돌 확인 (10001, 10002, 10010)

---

### 8.2 Deployment Steps

```bash
# 1. 서버 접속
ssh user@your-server-ip

# 2. 프로젝트 디렉토리 이동
cd /opt/castle-lord-tycoon

# 3. 최신 코드 풀
git pull origin main

# 4. 환경 변수 확인
cat .env

# 5. 컨테이너 중지
docker-compose down

# 6. 데이터베이스 마이그레이션 (필요시)
docker-compose run --rm api dotnet ef database update

# 7. 이미지 빌드 및 컨테이너 시작
docker-compose up -d --build

# 8. 로그 확인
docker-compose logs -f api

# 9. Health Check
curl https://castle.yourdomain.com/health
```

---

### 8.3 Post-Deployment

- [ ] Health Check 엔드포인트 정상 응답 확인
- [ ] SignalR 연결 테스트
- [ ] 로그 에러 확인: `docker-compose logs api | grep ERROR`
- [ ] 데이터베이스 연결 확인
- [ ] SSL 인증서 유효성 확인
- [ ] 모니터링 대시보드 확인

---

## 9. Monitoring and Logging

### 9.1 Application Logs

```bash
# 실시간 로그 확인
docker-compose logs -f api

# 최근 100줄
docker-compose logs --tail 100 api

# 에러만 필터링
docker-compose logs api | grep ERROR

# 파일로 저장
docker-compose logs api > logs/api-$(date +%Y%m%d).log
```

---

### 9.2 Nginx Access Logs

```bash
# 접속 로그 (기존 서버 Nginx)
sudo tail -f /var/log/nginx/access.log

# 에러 로그
sudo tail -f /var/log/nginx/error.log

# 특정 IP 필터링
sudo grep "192.168.1.100" /var/log/nginx/access.log
```

---

### 9.3 Database Performance

```sql
-- PostgreSQL 쿼리 성능 모니터링
SELECT pid, now() - pg_stat_activity.query_start AS duration, query
FROM pg_stat_activity
WHERE state = 'active'
ORDER BY duration DESC;

-- 느린 쿼리 로그 활성화 (postgresql.conf)
log_min_duration_statement = 1000  # 1초 이상 쿼리 로깅
```

---

## 10. Backup and Recovery

### 10.1 Database Backup

```bash
#!/bin/bash
# scripts/backup-db.sh

BACKUP_DIR="/opt/backups/postgres"
TIMESTAMP=$(date +%Y%m%d_%H%M%S)
BACKUP_FILE="$BACKUP_DIR/castlelord_backup_$TIMESTAMP.sql.gz"

mkdir -p $BACKUP_DIR

# PostgreSQL 백업
docker-compose exec -T postgres pg_dump -U admin castlelord_prod | gzip > $BACKUP_FILE

# 7일 이상 된 백업 삭제
find $BACKUP_DIR -type f -name "*.sql.gz" -mtime +7 -delete

echo "Backup completed: $BACKUP_FILE"
```

**Cron 설정 (매일 새벽 3시)**:
```bash
crontab -e

# 추가
0 3 * * * /opt/castle-lord-tycoon/scripts/backup-db.sh >> /var/log/backup.log 2>&1
```

---

### 10.2 Database Restore

```bash
# 백업 파일 복원
gunzip < /opt/backups/postgres/castlelord_backup_20250115_030000.sql.gz | \
  docker-compose exec -T postgres psql -U admin castlelord_prod
```

---

## 11. Scaling Strategies

### 11.1 Horizontal Scaling (향후)

```yaml
# docker-compose.scale.yml
services:
  api:
    deploy:
      replicas: 3
    # ... (기존 설정)

# Nginx upstream 수정 (기존 서버 /etc/nginx/sites-available/castle-lord-tycoon)
upstream api_backend {
    least_conn;
    server 127.0.0.1:10010;
    server 127.0.0.1:10011;
    server 127.0.0.1:10012;
}
```

---

### 11.2 Database Replication (향후)

```yaml
# Master-Slave PostgreSQL
postgres-master:
  # ... (기존 마스터 설정)

postgres-slave:
  image: postgres:17-alpine
  environment:
    POSTGRES_MASTER_SERVICE_HOST: postgres-master
    POSTGRES_REPLICATION_MODE: slave
  # ... (복제 설정)
```

---

## 12. Troubleshooting

### 12.1 Common Issues

**문제**: 컨테이너가 시작되지 않음
```bash
# 로그 확인
docker-compose logs api

# 환경 변수 확인
docker-compose config

# 포트 충돌 확인
sudo netstat -tulpn | grep :10010
```

---

**문제**: Database 연결 실패
```bash
# PostgreSQL 상태 확인
docker-compose exec postgres pg_isready -U admin

# 연결 테스트
docker-compose exec postgres psql -U admin -d castlelord_prod -c "SELECT 1;"
```

---

**문제**: SSL 인증서 오류
```bash
# 인증서 유효기간 확인
sudo openssl x509 -in /etc/letsencrypt/live/castle.yourdomain.com/fullchain.pem -noout -dates

# 인증서 갱신
sudo certbot renew --force-renewal
sudo systemctl reload nginx
```

---

**문제**: Nginx 설정 오류
```bash
# 설정 테스트
sudo nginx -t

# 에러 로그 확인
sudo tail -f /var/log/nginx/error.log

# Nginx 재시작
sudo systemctl restart nginx
```

---

## 13. Security Hardening

### 13.1 Server Hardening

```bash
# SSH 키 기반 인증만 허용
sudo nano /etc/ssh/sshd_config
# PasswordAuthentication no
sudo systemctl restart sshd

# Fail2Ban 설치 (무차별 대입 공격 방지)
sudo apt install fail2ban -y
sudo systemctl enable fail2ban

# 자동 보안 업데이트
sudo apt install unattended-upgrades -y
sudo dpkg-reconfigure --priority=low unattended-upgrades
```

---

### 13.2 Docker Security

```bash
# Docker 컨테이너 취약점 스캔
docker scan castle-lord-api:latest

# 불필요한 이미지 정리
docker system prune -a --volumes
```

---

## 14. Performance Optimization

### 14.1 Docker Image Optimization

```dockerfile
# Multi-stage build로 이미지 크기 최소화 (이미 적용됨)
# Alpine 기반 이미지 사용
# Layer 캐싱 활용
```

---

### 14.2 Nginx Caching

```nginx
# /etc/nginx/sites-available/castle-lord-tycoon에 추가
proxy_cache_path /var/cache/nginx/castle levels=1:2 keys_zone=castle_cache:10m max_size=100m inactive=60m;

location /api/static/ {
    proxy_cache castle_cache;
    proxy_cache_valid 200 60m;
    proxy_cache_use_stale error timeout updating http_500 http_502 http_503 http_504;
    add_header X-Cache-Status $upstream_cache_status;

    proxy_pass http://localhost:10010/api/static/;
}
```

---

## 15. Maintenance Windows

### 15.1 Planned Maintenance

```bash
#!/bin/bash
# scripts/maintenance-mode.sh

MODE=$1  # "on" or "off"

if [ "$MODE" = "on" ]; then
    # 503 페이지 활성화
    sudo mv /etc/nginx/sites-enabled/castle-lord-tycoon /etc/nginx/sites-available/castle-lord-tycoon.backup
    sudo cp /etc/nginx/sites-available/maintenance.conf /etc/nginx/sites-enabled/castle-lord-tycoon
    sudo systemctl reload nginx
    echo "Maintenance mode: ON"
else
    # 정상 서비스 복구
    sudo rm /etc/nginx/sites-enabled/castle-lord-tycoon
    sudo mv /etc/nginx/sites-available/castle-lord-tycoon.backup /etc/nginx/sites-enabled/castle-lord-tycoon
    sudo systemctl reload nginx
    echo "Maintenance mode: OFF"
fi
```

---

## 16. Documentation

- [Client-Server Contract](./client-server-contract.md): API 명세
- [Security Guidelines](./security-guidelines.md): 보안 정책
- [Architecture](./architecture.md): 시스템 아키텍처

---

## 17. Support

**문제 발생 시**:
1. 로그 확인: `docker-compose logs -f`
2. GitHub Issues 등록
3. 긴급 상황: admin@castlelordtycoon.com

---

## 18. Port Summary

기존 게임과 충돌 방지를 위해 10000번대 포트 사용:

| 서비스 | 외부 포트 | 내부 포트 | 설명 |
|--------|----------|----------|------|
| PostgreSQL | 10001 | 5432 | 데이터베이스 |
| Redis | 10002 | 6379 | 캐시 서버 |
| Game Server | 10010 | 8080 | REST API + SignalR |

**중요**:
- Nginx는 기존 서버의 443/80 포트 사용
- 컨테이너 간 통신은 내부 포트 사용 (예: postgres:5432)
- 외부 직접 접속시에만 외부 포트 사용 (예: localhost:10001)
