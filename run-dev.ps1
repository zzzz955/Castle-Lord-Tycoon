# ===== Castle Lord Tycoon - Development Environment Runner (Windows PowerShell) =====

Write-Host "🚀 Starting Castle Lord Tycoon - Development Mode" -ForegroundColor Green
Write-Host ""

# 1. .env.dev 파일 존재 확인
if (-Not (Test-Path ".env.dev")) {
    Write-Host "❌ Error: .env.dev file not found!" -ForegroundColor Red
    Write-Host "Please create .env.dev from .env.example"
    exit 1
}

# 2. 환경 변수 로드
Write-Host "📦 Loading environment variables from .env.dev..." -ForegroundColor Cyan
Get-Content .env.dev | ForEach-Object {
    if ($_ -match '^([^=]+)=(.*)$') {
        $name = $matches[1]
        $value = $matches[2]
        [Environment]::SetEnvironmentVariable($name, $value, "Process")
    }
}

# 3. Docker 컨테이너 시작 (PostgreSQL + Redis)
Write-Host "🐳 Starting Docker containers (PostgreSQL + Redis)..." -ForegroundColor Cyan
docker-compose --env-file .env.dev -f docker-compose.dev.yml up -d

# 4. 컨테이너 시작 대기
Write-Host "⏳ Waiting for containers to be ready..." -ForegroundColor Yellow
Start-Sleep -Seconds 3

# 5. ASP.NET Core 서버 실행
Write-Host "🔧 Starting ASP.NET Core server..." -ForegroundColor Cyan
Set-Location CastleLordTycoon.Server
dotnet run --environment Development

# 6. 종료 시 안내
Write-Host ""
Write-Host "✅ Server stopped. Docker containers are still running." -ForegroundColor Green
Write-Host "To stop Docker: docker-compose -f docker-compose.dev.yml down"
