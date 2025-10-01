using BookManagementSystem.Services;
using BookManagementSystem.services;
using Duende.IdentityServer.Services;
using HotelManagementApplication.Interface.Repositories;
using HotelManagementApplication.Interface.Services;
using HotelManagementInfrastructure.Services;
using HotelManagementInfratructure.Identity;
using HotelManagementInfratructure.Persistence;
using HotelManagementInfratructure.Repository;
using HotelManagementInfratructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementInfratructure.Injection
{
    public static class ClassInjection
    {
        public static IServiceCollection AddInfrastructureClass(this IServiceCollection services, IConfiguration configuration)
        {
            // Services
            services.AddScoped<TokenServices>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IRoomService, RoomService>();
            services.AddScoped<IRoomImageService, RoomImageService>();
            services.AddScoped<IGuestService, GuestService>();
            services.AddScoped<IHotelStaffService, HotelStaffService>();
            services.AddScoped<IMaintenanceDateService, MaintenanceDateService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IBookingOtpService, BookingOtpService>();
            services.AddScoped<IDashboardService, DashboardService>();

            // Repositories
            services.AddScoped<IBooking, BookingRepository>();
            services.AddScoped<IRoom, RoomRepository>();
            services.AddScoped<IGuest, GuestRepository>();
            services.AddScoped<IHotelStaff, HotelStaffRepository>();
            services.AddScoped<IMaintenanceDate, MaintenanceDateRepository>();
            services.AddScoped<IBookingOtp, BookingOtpRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
