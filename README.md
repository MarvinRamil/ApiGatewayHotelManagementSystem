# Hotel Management System

A comprehensive hotel management system built with .NET 9.0, featuring booking management, room management, guest management, and dashboard analytics.

## Features

- **Authentication & Authorization**: JWT-based authentication with refresh tokens
- **Booking Management**: Complete booking lifecycle with OTP verification
- **Room Management**: Room status tracking, image uploads, maintenance scheduling
- **Guest Management**: Guest information and booking history
- **Dashboard Analytics**: Real-time statistics, revenue tracking, and reporting
- **Email Notifications**: Automated email notifications for bookings and confirmations
- **Image Management**: Room image uploads with WebP conversion

## Prerequisites

- Docker and Docker Compose
- .NET 9.0 SDK (for local development)
- SQL Server (included in Docker setup)

## Quick Start

### 1. Clone the Repository
```bash
git clone <repository-url>
cd ApiGatewayHotelManagementSystem
```

### 2. Environment Configuration
```bash
# Copy the example environment file
cp env.example .env

# Edit .env file with your configuration
nano .env
```

### 3. Deploy with Docker
```bash
# Linux/Mac
chmod +x deploy.sh
./deploy.sh

# Windows
deploy.bat
```

### 4. Access the Application
- **API**: http://localhost:8080
- **Swagger Documentation**: http://localhost:8080/swagger
- **Database**: localhost:1433

## Environment Variables

Create a `.env` file with the following variables:

```env
# Database Configuration
DB_PASSWORD=YourStrong@Passw0rd

# JWT Configuration
JWT_KEY=your-super-secret-jwt-key
JWT_ISSUER=BookManagementSystem
JWT_AUDIENCE=BookManagementSystemUsers

# Email Configuration
SMTP_HOST=your-smtp-host
SMTP_PORT=587
SMTP_USERNAME=your-smtp-username
SMTP_PASSWORD=your-smtp-password
SMTP_USE_SSL=false
EMAIL_FROM=noreply@yourhotel.com
EMAIL_FROM_NAME=Hotel Management System
EMAIL_ADMIN=admin@yourhotel.com

# Hotel Information
HOTEL_NAME=Your Hotel Name
HOTEL_ADDRESS=Your Hotel Address
HOTEL_PHONE=+1-000-000-0000
```

## API Endpoints

### Authentication
- `POST /api/auth/register` - User registration
- `POST /api/auth/login` - User login
- `POST /api/auth/refresh` - Refresh token

### Bookings
- `GET /api/booking` - Get all bookings
- `POST /api/booking` - Create new booking
- `GET /api/booking/{id}` - Get booking by ID
- `PUT /api/booking/{id}` - Update booking
- `DELETE /api/booking/{id}` - Delete booking

### Rooms
- `GET /api/room` - Get all rooms
- `POST /api/room` - Create new room
- `GET /api/room/{id}` - Get room by ID
- `PUT /api/room/{id}` - Update room
- `DELETE /api/room/{id}` - Delete room

### Dashboard
- `GET /api/dashboard` - Get complete dashboard data
- `GET /api/dashboard/stats` - Get hotel statistics
- `GET /api/dashboard/revenue` - Get revenue data
- `GET /api/dashboard/room-status` - Get room status overview

## Development

### Local Development Setup
```bash
# Restore packages
dotnet restore

# Update database
dotnet ef database update

# Run the application
dotnet run --project BookingServiceApi
```

### Database Migrations
```bash
# Add new migration
dotnet ef migrations add MigrationName --project HotelManagementInfratructure

# Update database
dotnet ef database update --project HotelManagementInfratructure
```

## Docker Commands

```bash
# Build and start services
docker-compose up --build -d

# View logs
docker-compose logs -f

# Stop services
docker-compose down

# Remove volumes (WARNING: This will delete all data)
docker-compose down -v
```

## Project Structure

```
├── BookingServiceApi/          # Main API project
├── HotelManagementApplication/ # Application layer (DTOs, Interfaces)
├── HotelManagementInfratructure/ # Infrastructure layer (Services, Repositories)
├── HootelManagementDomain/     # Domain layer (Entities, Enums)
├── Dockerfile                  # Docker configuration
├── docker-compose.yml         # Docker Compose configuration
├── .env                       # Environment variables (not in git)
└── env.example               # Environment variables template
```

## Security

- JWT-based authentication with refresh tokens
- Environment variables for sensitive configuration
- Input validation and sanitization
- CORS configuration
- Rate limiting (if using reverse proxy)

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## License

This project is licensed under the MIT License.
