# Use the official .NET 9.0 runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the official .NET 9.0 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY ["BookingServiceApi/BookingServiceApi.csproj", "BookingServiceApi/"]
COPY ["HotelManagementApplication/HotelManagementApplication.csproj", "HotelManagementApplication/"]
COPY ["HotelManagementInfratructure/HotelManagementInfratructure.csproj", "HotelManagementInfratructure/"]
COPY ["HootelManagementDomain/HootelManagementDomain.csproj", "HootelManagementDomain/"]

# Restore dependencies
RUN dotnet restore "BookingServiceApi/BookingServiceApi.csproj"

# Copy the rest of the source code
COPY . .

# Build the application
WORKDIR "/src/BookingServiceApi"
RUN dotnet build "BookingServiceApi.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "BookingServiceApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage/image
FROM base AS final
WORKDIR /app

# Copy the published application
COPY --from=publish /app/publish .

# Create directory for uploaded images
RUN mkdir -p /app/wwwroot/images/rooms

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
  CMD curl -f http://localhost/health || exit 1

ENTRYPOINT ["dotnet", "BookingServiceApi.dll"]
