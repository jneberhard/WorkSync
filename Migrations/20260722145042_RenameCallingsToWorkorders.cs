using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkSync.Migrations
{
    /// <inheritdoc />
    public partial class RenameCallingsToWorkorders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Callings",
                newName: "Workorders");

            migrationBuilder.Sql(
                """ALTER TABLE "Workorders" RENAME CONSTRAINT "PK_Callings" TO "PK_Workorders";""");

            migrationBuilder.RenameColumn(
                name: "CurrentCalling",
                table: "Members",
                newName: "CurrentWorkorder");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Workorders",
                newName: "Callings");

            migrationBuilder.Sql(
                """ALTER TABLE "Callings" RENAME CONSTRAINT "PK_Workorders" TO "PK_Callings";""");

            migrationBuilder.RenameColumn(
                name: "CurrentWorkorder",
                table: "Members",
                newName: "CurrentCalling");
        }
    }
}
