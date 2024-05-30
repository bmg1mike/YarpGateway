using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sterling.Gateway.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedDateTimeToCluster : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MicroServices",
                schema: "Gateway");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                schema: "Gateway",
                table: "ClusterConfigs",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateModified",
                schema: "Gateway",
                table: "ClusterConfigs",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "Gateway",
                table: "ClusterConfigs",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateCreated",
                schema: "Gateway",
                table: "ClusterConfigs");

            migrationBuilder.DropColumn(
                name: "DateModified",
                schema: "Gateway",
                table: "ClusterConfigs");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "Gateway",
                table: "ClusterConfigs");

            migrationBuilder.CreateTable(
                name: "MicroServices",
                schema: "Gateway",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    BaseUrl = table.Column<string>(type: "text", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    MicroServiceName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MicroServices", x => x.Id);
                });
        }
    }
}
