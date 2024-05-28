using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sterling.Gateway.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedEndpointsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                schema: "Gateway",
                table: "RouteConfigs",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateModified",
                schema: "Gateway",
                table: "RouteConfigs",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "Gateway",
                table: "RouteConfigs",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MicroServiceId",
                schema: "Gateway",
                table: "RouteConfigs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Endpoints",
                schema: "Gateway",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ApiType = table.Column<int>(type: "integer", nullable: false),
                    SubUrl = table.Column<string>(type: "text", nullable: false),
                    ClusterConfigEntityId = table.Column<string>(type: "text", nullable: false),
                    RequestPayload = table.Column<string>(type: "text", nullable: true),
                    ResponsePayload = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Endpoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Endpoints_ClusterConfigs_ClusterConfigEntityId",
                        column: x => x.ClusterConfigEntityId,
                        principalSchema: "Gateway",
                        principalTable: "ClusterConfigs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Endpoints_ClusterConfigEntityId",
                schema: "Gateway",
                table: "Endpoints",
                column: "ClusterConfigEntityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Endpoints",
                schema: "Gateway");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                schema: "Gateway",
                table: "RouteConfigs");

            migrationBuilder.DropColumn(
                name: "DateModified",
                schema: "Gateway",
                table: "RouteConfigs");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "Gateway",
                table: "RouteConfigs");

            migrationBuilder.DropColumn(
                name: "MicroServiceId",
                schema: "Gateway",
                table: "RouteConfigs");
        }
    }
}
