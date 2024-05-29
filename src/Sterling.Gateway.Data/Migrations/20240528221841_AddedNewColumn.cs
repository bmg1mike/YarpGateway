using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sterling.Gateway.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedNewColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Endpoints_ClusterConfigs_ClusterConfigEntityId",
                schema: "Gateway",
                table: "Endpoints");

            migrationBuilder.RenameColumn(
                name: "ClusterConfigEntityId",
                schema: "Gateway",
                table: "Endpoints",
                newName: "MicroServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_Endpoints_ClusterConfigEntityId",
                schema: "Gateway",
                table: "Endpoints",
                newName: "IX_Endpoints_MicroServiceId");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationName",
                schema: "Gateway",
                table: "AspNetUsers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddForeignKey(
                name: "FK_Endpoints_ClusterConfigs_MicroServiceId",
                schema: "Gateway",
                table: "Endpoints",
                column: "MicroServiceId",
                principalSchema: "Gateway",
                principalTable: "ClusterConfigs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Endpoints_ClusterConfigs_MicroServiceId",
                schema: "Gateway",
                table: "Endpoints");

            migrationBuilder.RenameColumn(
                name: "MicroServiceId",
                schema: "Gateway",
                table: "Endpoints",
                newName: "ClusterConfigEntityId");

            migrationBuilder.RenameIndex(
                name: "IX_Endpoints_MicroServiceId",
                schema: "Gateway",
                table: "Endpoints",
                newName: "IX_Endpoints_ClusterConfigEntityId");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationName",
                schema: "Gateway",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Endpoints_ClusterConfigs_ClusterConfigEntityId",
                schema: "Gateway",
                table: "Endpoints",
                column: "ClusterConfigEntityId",
                principalSchema: "Gateway",
                principalTable: "ClusterConfigs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
