using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sterling.Gateway.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedRouteAndClusterTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClusterConfigs",
                schema: "Gateway",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ClusterId = table.Column<string>(type: "text", nullable: false),
                    DestinationAddress = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClusterConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RouteConfigs",
                schema: "Gateway",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    RouteId = table.Column<string>(type: "text", nullable: false),
                    ClusterId = table.Column<string>(type: "text", nullable: false),
                    Path = table.Column<string>(type: "text", nullable: false),
                    AuthorizationPolicy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteConfigs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClusterConfigs",
                schema: "Gateway");

            migrationBuilder.DropTable(
                name: "RouteConfigs",
                schema: "Gateway");
        }
    }
}
