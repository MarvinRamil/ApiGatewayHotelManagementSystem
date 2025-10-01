#!/bin/bash

# Hotel Management System Deployment Script
echo "Starting Hotel Management System deployment..."

# Check if .env file exists
if [ ! -f .env ]; then
    echo "Error: .env file not found!"
    echo "Please copy env.example to .env and configure your environment variables."
    exit 1
fi

# Load environment variables
export $(cat .env | grep -v '^#' | xargs)

# Stop existing containers
echo "Stopping existing containers..."
docker-compose down

# Remove old images (optional)
echo "Removing old images..."
docker-compose down --rmi all

# Build and start services
echo "Building and starting services..."
docker-compose up --build -d

# Wait for services to be ready
echo "Waiting for services to be ready..."
sleep 30

# Check if services are running
echo "Checking service status..."
docker-compose ps

# Run database migrations (if needed)
echo "Running database migrations..."
docker-compose exec hotel-api dotnet ef database update

echo "Deployment completed!"
echo "API is available at: http://localhost:8080"
echo "Swagger documentation: http://localhost:8080/swagger"
echo "Database is available at: localhost:1433"
