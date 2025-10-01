using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelManagementInfratructure.Migrations
{
    /// <inheritdoc />
    public partial class otp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BookingOtps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", maxLength: 36, nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    OtpCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    UsedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingOtps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingOtps_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f2a4e4f5-9b4e-4f7d-9c15-3c9a8f52a2bb",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "29ffb399-ff8d-425b-848b-b7700f0c2090", "AQAAAAIAAYagAAAAEPyToaAiHSa4FhPkHHgqXVqLPM4FdXRHN75HMQQaPOmWTYn9Oxldm6uXfjkPywGImA==", "1a7bfb35-ddd9-4e03-91f3-6a5ef54a6204" });

            migrationBuilder.CreateIndex(
                name: "IX_BookingOtps_BookingId",
                table: "BookingOtps",
                column: "BookingId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingOtps");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f2a4e4f5-9b4e-4f7d-9c15-3c9a8f52a2bb",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "12eb7f4c-a84c-4abe-8307-ba4df4dde251", "AQAAAAIAAYagAAAAECRDSdCtOBtDlIlDW2Q2neu9/weKDck7OxjPI613gFL5UtsZ9NecOCrZA3UiNiPXvA==", "f8974293-3eae-4528-aea7-58d48a0eee97" });
        }
    }
}
