@echo off
echo Starting Hotel Management System deployment...

REM Check if .env file exists
if not exist .env (
    echo Error: .env file not found!
    echo Please copy env.example to .env and configure your environment variables.
    pause
    exit /b 1
)

REM Stop existing containers
echo Stopping existing containers...
docker-compose down

REM Remove old images (optional)
echo Removing old images...
docker-compose down --rmi all

REM Build and start services
echo Building and starting services...
docker-compose up --build -d

REM Wait for services to be ready
echo Waiting for services to be ready...
timeout /t 30 /nobreak > nul

REM Check if services are running
echo Checking service status...
docker-compose ps

REM Run database migrations (if needed)
echo Running database migrations...
docker-compose exec hotel-api dotnet ef database update

echo Deployment completed!
echo API is available at: http://localhost:8080
echo Swagger documentation: http://localhost:8080/swagger
echo Database is available at: localhost:1433
pause
