using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace harc_api.Modules.Identity.Data.Migrations
{
    /// <inheritdoc />
    public partial class DocumentsAdded2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Leaves_LeaveId",
                schema: "document",
                table: "Documents");

            migrationBuilder.RenameColumn(
                name: "LeaveId",
                schema: "document",
                table: "Documents",
                newName: "LeaveEntityId");

            migrationBuilder.RenameIndex(
                name: "IX_Documents_LeaveId",
                schema: "document",
                table: "Documents",
                newName: "IX_Documents_LeaveEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Leaves_LeaveEntityId",
                schema: "document",
                table: "Documents",
                column: "LeaveEntityId",
                principalSchema: "leave",
                principalTable: "Leaves",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Leaves_LeaveEntityId",
                schema: "document",
                table: "Documents");

            migrationBuilder.RenameColumn(
                name: "LeaveEntityId",
                schema: "document",
                table: "Documents",
                newName: "LeaveId");

            migrationBuilder.RenameIndex(
                name: "IX_Documents_LeaveEntityId",
                schema: "document",
                table: "Documents",
                newName: "IX_Documents_LeaveId");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Leaves_LeaveId",
                schema: "document",
                table: "Documents",
                column: "LeaveId",
                principalSchema: "leave",
                principalTable: "Leaves",
                principalColumn: "Id");
        }
    }
}
