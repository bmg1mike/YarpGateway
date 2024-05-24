using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sterling.Gateway.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedMicroserviceAndEndpointsTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MicroServices",
                schema: "Gateway",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    MicroServiceName = table.Column<string>(type: "text", nullable: false),
                    BaseUrl = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MicroServices", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MicroServices",
                schema: "Gateway");
        }
    }
}
