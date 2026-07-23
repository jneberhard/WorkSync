using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WorkSync.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiTenancy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Workorders",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Members",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "FollowUpItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Assignments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TenantRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CompanyName = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    AdminEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    RequestedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ApprovedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AssignedTenantId = table.Column<int>(type: "integer", nullable: true),
                    IsApproved = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    AdminEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    IsApproved = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ApprovedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Tenants",
                columns: new[] { "Id", "AdminEmail", "ApprovedAtUtc", "CreatedAtUtc", "IsActive", "IsApproved", "Name" },
                values: new object[] { 3735930, "admin@worksync.com", new DateTime(2026, 7, 23, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 7, 23, 0, 0, 0, 0, DateTimeKind.Utc), true, true, "Brainstormers" });

            // Preserve every pre-existing record by assigning it to the legacy
            // Brainstormers tenant before tenant foreign keys are created.
            migrationBuilder.Sql("""
                UPDATE "AspNetUsers" SET "TenantId" = 3735930;
                UPDATE "Workorders" SET "TenantId" = 3735930;
                UPDATE "Assignments" SET "TenantId" = 3735930;
                UPDATE "FollowUpItems" SET "TenantId" = 3735930;
                UPDATE "Members" SET "TenantId" = 3735930;
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Workorders_TenantId",
                table: "Workorders",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Members_TenantId",
                table: "Members",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_FollowUpItems_TenantId",
                table: "FollowUpItems",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_TenantId",
                table: "Assignments",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_TenantId",
                table: "AspNetUsers",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Tenants_TenantId",
                table: "AspNetUsers",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Tenants_TenantId",
                table: "Assignments",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FollowUpItems_Tenants_TenantId",
                table: "FollowUpItems",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Members_Tenants_TenantId",
                table: "Members",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Workorders_Tenants_TenantId",
                table: "Workorders",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Tenants_TenantId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Tenants_TenantId",
                table: "Assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_FollowUpItems_Tenants_TenantId",
                table: "FollowUpItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Members_Tenants_TenantId",
                table: "Members");

            migrationBuilder.DropForeignKey(
                name: "FK_Workorders_Tenants_TenantId",
                table: "Workorders");

            migrationBuilder.DropTable(
                name: "TenantRequests");

            migrationBuilder.DropTable(
                name: "Tenants");

            migrationBuilder.DropIndex(
                name: "IX_Workorders_TenantId",
                table: "Workorders");

            migrationBuilder.DropIndex(
                name: "IX_Members_TenantId",
                table: "Members");

            migrationBuilder.DropIndex(
                name: "IX_FollowUpItems_TenantId",
                table: "FollowUpItems");

            migrationBuilder.DropIndex(
                name: "IX_Assignments_TenantId",
                table: "Assignments");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_TenantId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Workorders");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "FollowUpItems");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AspNetUsers");
        }
    }
}
