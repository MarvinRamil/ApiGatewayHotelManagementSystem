using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelManagementInfratructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Guests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "BookingNumber",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "EmployeeId", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "f2a4e4f5-9b4e-4f7d-9c15-3c9a8f52a2bb", 0, "1bea2462-b1ac-4ad1-b140-43b356e5f6d3", "admin@localhost.com", true, null, false, null, "ADMIN@LOCALHOST.COM", "ADMIN", "AQAAAAIAAYagAAAAEPC0+9AY02Jyiu59xOgdE68DlTfMjlmoXje/4PT/AHv1wXD2thu4+mKTqMjwJakbNQ==", null, false, "3cd0d080-65cf-4970-b0f5-e5b8852c9faa", false, "Admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "8d69da53-ca1b-4c83-85d8-99bd7fa9836c", "f2a4e4f5-9b4e-4f7d-9c15-3c9a8f52a2bb" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "8d69da53-ca1b-4c83-85d8-99bd7fa9836c", "f2a4e4f5-9b4e-4f7d-9c15-3c9a8f52a2bb" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f2a4e4f5-9b4e-4f7d-9c15-3c9a8f52a2bb");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Guests");

            migrationBuilder.DropColumn(
                name: "BookingNumber",
                table: "Bookings");
        }
    }
}
