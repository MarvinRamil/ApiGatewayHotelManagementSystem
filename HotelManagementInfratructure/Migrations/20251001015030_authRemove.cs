using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelManagementInfratructure.Migrations
{
    /// <inheritdoc />
    public partial class authRemove : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f2a4e4f5-9b4e-4f7d-9c15-3c9a8f52a2bb",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "981d010a-c3bc-4eb5-9d56-9f82bbc5e60a", "AQAAAAIAAYagAAAAEAXpciD7fo32Z+G6l+JrrjcU3o1CRPdIgB1FCKFgdv2C3l0M1vGFOtR/DmCUgAjF9g==", "4819cddd-44ba-4c90-a758-9c8f6e3c4ea2" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false),
                    ReasonRevoked = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReplacedByToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RevokedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f2a4e4f5-9b4e-4f7d-9c15-3c9a8f52a2bb",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b45e5d55-3815-4d94-8283-06d9722140fc", "AQAAAAIAAYagAAAAEGq0HCP+99iLBqkZY99TtCsP3doMlEGP4XSMGYqPuChoptC0MDJDfwzXbTVVF8IqQg==", "8d4b7219-2690-420f-9a78-57693ff7da51" });
        }
    }
}
