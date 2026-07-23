using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkSync.Migrations
{
    /// <inheritdoc />
    public partial class LinkTenantRequestAdminAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdminUserId",
                table: "TenantRequests",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminUserId",
                table: "TenantRequests");
        }
    }
}
