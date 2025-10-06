# Hotel Management System

A comprehensive hotel management system built with .NET 8, featuring API Gateway architecture, JWT authentication, and full booking management capabilities.

## 🏨 Overview

This system provides a complete solution for hotel operations including:
- **Guest Management**: Registration, authentication, and profile management
- **Room Management**: Room types, availability, pricing, and maintenance scheduling
- **Booking System**: Complete booking lifecycle from reservation to checkout
- **Staff Management**: Role-based access control for hotel staff
- **Email Notifications**: Automated email system for booking confirmations and updates
- **Dashboard Analytics**: Real-time insights and reporting

## 🏗️ Architecture

The system follows a **Clean Architecture** pattern with the following layers:

```
┌─────────────────────────────────────┐
│        Booking Service API          │
│      (BookingServiceApi)            │
└─────────────────────────────────────┘
                    │
┌─────────────────────────────────────┐
│     Application Layer               │
│ (HotelManagementApplication)        │
└─────────────────────────────────────┘
                    │
┌─────────────────────────────────────┐
│     Infrastructure Layer            │
│ (HotelManagementInfrastructure)     │
└─────────────────────────────────────┘
                    │
┌─────────────────────────────────────┐
│        Domain Layer                 │
│   (HootelManagementDomain)          │
└─────────────────────────────────────┘
```

## 🚀 Features

### Authentication & Authorization
- JWT-based authentication with refresh tokens
- Role-based access control (Admin, Manager, Receptionist, etc.)
- Secure password hashing
- Email verification system

### Booking Management
- Complete booking lifecycle management
- Real-time room availability checking
- Booking status tracking (Pending, Confirmed, Checked-in, Checked-out, Cancelled)
- OTP verification for bookings
- Automatic booking number generation

### Room Management
- Room types and pricing management
- Room status tracking (Available, Occupied, Maintenance, Out of Order)
- Room amenities and capacity management
- Image upload and management
- Maintenance scheduling

### Guest Management
- Guest registration and profile management
- Booking history tracking
- Email-based guest communication

### Staff Management
- Staff role management (Manager, Receptionist, Housekeeping, etc.)
- Staff authentication and authorization
- Dashboard access control

### Email System
- Automated booking confirmations
- Check-in/check-out notifications
- OTP delivery via email
- Customizable email templates

## 🛠️ Technology Stack

- **.NET 8** - Backend framework
- **ASP.NET Core Web API** - REST API development
- **Entity Framework Core** - ORM for database operations
- **SQL Server** - Database
- **JWT** - Authentication and authorization
- **Docker** - Containerization
- **Swagger/OpenAPI** - API documentation

## 📋 Prerequisites

- .NET 8 SDK
- SQL Server (Local or Docker)
- Docker (optional, for containerized deployment)
- Visual Studio 2022 or VS Code

## 🚀 Getting Started

### 1. Clone the Repository

```bash
git clone <repository-url>
cd ApiGatewayHotelManagementSystem
```

### 2. Database Setup

#### Option A: Using Docker
```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Passw0rd" \
   -p 1433:1433 --name sqlserver \
   -d mcr.microsoft.com/mssql/server:2022-latest
```

#### Option B: Local SQL Server
- Install SQL Server 2022
- Create a database named `HotelManagement`

### 3. Environment Configuration

Copy the environment example file and configure your settings:

```bash
cp env.example .env
```

Update the `.env` file with your configuration:

```env
# Database Configuration
DB_SERVER=localhost
DB_PORT=1433
DB_NAME=HotelManagement
DB_USERNAME=sa
DB_PASSWORD=YourStrong@Passw0rd

# JWT Configuration
JWT_KEY=your-super-secret-jwt-key-here
JWT_ISSUER=BookManagementSystem
JWT_AUDIENCE=BookManagementSystemUsers

# Email Configuration
SMTP_HOST=your-smtp-host
SMTP_PORT=587
SMTP_USERNAME=your-email@domain.com
SMTP_PASSWORD=your-email-password
SMTP_USE_SSL=false
EMAIL_FROM=noreply@hotel.com
EMAIL_FROM_NAME=Hotel Management System
EMAIL_ADMIN=admin@hotel.com

# Hotel Information
HOTEL_NAME=Your Hotel Name
HOTEL_ADDRESS=Your Hotel Address
HOTEL_PHONE=+1-000-000-0000
```

### 4. Database Migration

Run the following commands to set up the database:

```bash
# Navigate to the infrastructure project
cd HotelManagementInfratructure

# Add migration
dotnet ef migrations add InitialCreate

# Update database
dotnet ef database update
```

### 5. Run the Application

#### Option A: Using Docker Compose
```bash
docker-compose up -d
```

#### Option B: Manual Setup
```bash
# Run the Booking Service
cd BookingServiceApi
dotnet run
```

The application will be available at:
- **Booking Service**: `http://localhost:5000`
- **Swagger UI**: `http://localhost:5000/swagger`

## 📚 API Documentation

### Authentication Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/register` | Register a new user |
| POST | `/api/auth/login` | User login |
| POST | `/api/auth/refresh` | Refresh access token |

### Booking Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/booking` | Get all bookings |
| GET | `/api/booking/{id}` | Get booking by ID |
| POST | `/api/booking` | Create new booking |
| PUT | `/api/booking/{id}` | Update booking |
| DELETE | `/api/booking/{id}` | Delete booking |
| PATCH | `/api/booking/{id}/status` | Update booking status |
| PATCH | `/api/booking/{id}/check-in` | Check-in booking |
| PATCH | `/api/booking/{id}/check-out` | Check-out booking |
| GET | `/api/booking/check-availability` | Check room availability |

### Room Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/room` | Get all rooms |
| GET | `/api/room/{id}` | Get room by ID |
| POST | `/api/room` | Create new room |
| PUT | `/api/room/{id}` | Update room |
| DELETE | `/api/room/{id}` | Delete room |

### Guest Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/guest` | Get all guests |
| GET | `/api/guest/{id}` | Get guest by ID |
| POST | `/api/guest` | Create new guest |
| PUT | `/api/guest/{id}` | Update guest |
| DELETE | `/api/guest/{id}` | Delete guest |

## 🔐 Authentication

The system uses JWT tokens for authentication. Include the token in the Authorization header:

```
Authorization: Bearer <your-jwt-token>
```

### User Roles

- **Admin**: Full system access
- **Manager**: Hotel management operations
- **Receptionist**: Check-in/check-out operations
- **Housekeeping**: Room maintenance
- **Maintenance**: Technical maintenance
- **Security**: Security operations
- **Chef**: Kitchen operations
- **Waiter**: Service operations

## 📊 Database Schema

### Core Entities

- **Booking**: Manages hotel reservations
- **Room**: Hotel room information and status
- **Guest**: Guest information and profiles
- **HotelStaff**: Staff member information
- **MaintenanceDate**: Room maintenance scheduling
- **BookingOtp**: OTP verification for bookings

### Enums

- **BookingStatus**: Pending, Confirmed, Cancelled, CheckedIn, CheckedOut
- **RoomStatus**: Available, Occupied, Maintenance, OutOfOrder
- **StaffRoles**: Manager, Receptionist, Housekeeping, etc.
- **MaintenanceStatus**: Scheduled, InProgress, Completed
- **OtpStatus**: Pending, Verified, Expired

## 🐳 Docker Deployment

### Using Docker Compose

```bash
# Build and start all services
docker-compose up -d

# View logs
docker-compose logs -f

# Stop services
docker-compose down
```

### Environment Variables

The Docker setup uses environment variables from the `.env` file. Make sure to configure all required variables before deployment.

## 🧪 Testing

### API Testing

Use the provided HTTP files for testing:

- `ApiGatewayHotelManagementSystem.http`
- `BookingServiceApi.http`

### Manual Testing

1. Register a new user
2. Login to get JWT token
3. Use the token to access protected endpoints
4. Test booking creation and management

## 📝 Development

### Project Structure

```
ApiGatewayHotelManagementSystem/
├── BookingServiceApi/                   # Main booking service
├── HotelManagementApplication/          # Application layer
├── HotelManagementInfratructure/        # Infrastructure layer
├── HootelManagementDomain/              # Domain layer
├── docker-compose.yml                   # Docker configuration
├── Dockerfile                           # Docker configuration
├── deploy.bat                           # Deployment script
├── deploy.sh                            # Deployment script
├── env.example                          # Environment template
└── README.md                            # Project documentation
```

### Adding New Features

1. Define entities in the Domain layer
2. Create DTOs in the Application layer
3. Implement services in the Application layer
4. Add repositories in the Infrastructure layer
5. Create controllers in the API layer
6. Update database with migrations

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License.

## 🆘 Support

For support and questions:
- Create an issue in the repository
- Contact the development team
- Check the API documentation

## 🔄 Version History

- **v1.0.0** - Initial release with basic booking management
- **v1.1.0** - Added email notifications and OTP verification
- **v1.2.0** - Enhanced authentication with refresh tokens
- **v1.3.0** - Added API Gateway and improved architecture

---

**Built with ❤️ for modern hotel management**