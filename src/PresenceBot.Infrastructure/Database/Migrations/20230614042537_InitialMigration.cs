using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PresenceBot.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Identity = table.Column<string>(type: "TEXT", nullable: false),
                    LastPresentedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    IdentityComponents = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Identity);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Clients");
        }
    }
}
