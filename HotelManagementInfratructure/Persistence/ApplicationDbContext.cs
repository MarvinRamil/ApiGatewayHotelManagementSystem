using HootelManagementDomain.Entities;
using HotelManagementInfratructure.Identity;
using HotelManagementInfratructure.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace HotelManagementInfratructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<User>, IDBContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Booking> Bookings => Set<Booking>();
        public DbSet<Guest> Guests => Set<Guest>();
        public DbSet<HotelStaff> HotelStaffs => Set<HotelStaff>();
        public DbSet<Room> Rooms => Set<Room>();
        public DbSet<MaintenanceDate> MaintenanceDates => Set<MaintenanceDate>();
        public DbSet<BookingOtp> BookingOtps => Set<BookingOtp>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Entity Primary Keys and Base Properties
            ConfigureBaseEntities(modelBuilder);

            // Apply global query filter for soft delete
            foreach (var entityType in modelBuilder.Model.GetEntityTypes()
                .Where(e => typeof(BaseEntity).IsAssignableFrom(e.ClrType) && !e.IsAbstract()))
            {
                var method = typeof(ApplicationDbContext).GetMethod(
                    nameof(SetGlobalQuery),
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
                    .MakeGenericMethod(entityType.ClrType);

                method.Invoke(null, new object[] { modelBuilder });
            }

            // Seed Identity Data
            SeedData(modelBuilder);
        }

        private static void SetGlobalQuery<T>(ModelBuilder builder) where T : BaseEntity
        {
            builder.Entity<T>().HasQueryFilter(e => !e.IsDeleted);
        }

        private void ConfigureBaseEntities(ModelBuilder modelBuilder)
        {
            // Configure common properties for all entities inheriting from BaseEntity
            foreach (var entityType in modelBuilder.Model.GetEntityTypes()
                .Where(e => e.ClrType.IsSubclassOf(typeof(BaseEntity))))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property("Id")
                    .HasMaxLength(36);
            }
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // --- Seed Role ---
            var adminRoleId = "8d69da53-ca1b-4c83-85d8-99bd7fa9836c";
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = adminRoleId,
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                }
            );

            // --- Seed Admin User ---
            var adminUserId = "f2a4e4f5-9b4e-4f7d-9c15-3c9a8f52a2bb"; // fixed GUID

            var hasher = new PasswordHasher<User>();
            var adminUser = new User
            {
                Id = adminUserId,
                UserName = "Admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@localhost.com",
                NormalizedEmail = "ADMIN@LOCALHOST.COM",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D"),
                EmployeeId = null
            };

            adminUser.PasswordHash = hasher.HashPassword(adminUser, "Admin.12345");

            modelBuilder.Entity<User>().HasData(adminUser);

            // --- Assign Role to User ---
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    RoleId = adminRoleId,
                    UserId = adminUserId
                }
            );
        }
    }
}
