using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace auth_server.Migrations
{
    /// <inheritdoc />
    public partial class OtpModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Otps",
                columns: table => new
                {
                    peopleId = table.Column<string>(type: "text", nullable: false),
                    otp = table.Column<int>(type: "integer", nullable: false),
                    expiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Otps", x => x.peopleId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Otps");
        }
    }
}
