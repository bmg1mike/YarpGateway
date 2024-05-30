using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sterling.Gateway.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveMicroserviceTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PermissionRole",
                schema: "Gateway",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PermissionRole",
                schema: "Gateway",
                table: "AspNetUsers");
        }
    }
}
