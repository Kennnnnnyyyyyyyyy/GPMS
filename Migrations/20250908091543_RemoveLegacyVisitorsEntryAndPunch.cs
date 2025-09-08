using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gate_Pass_management.Migrations
{
    /// <inheritdoc />
    public partial class RemoveLegacyVisitorsEntryAndPunch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Temporarily disable FK enforcement for SQLite while we rebuild/drop tables
            migrationBuilder.Sql("PRAGMA foreign_keys = OFF;", suppressTransaction: true);

            migrationBuilder.DropForeignKey(
                name: "FK_GatePasses_VisitorsEntries_VisitorEntryId",
                table: "GatePasses");

            migrationBuilder.DropTable(
                name: "Punches");

            migrationBuilder.DropTable(
                name: "VisitorsEntries");

            migrationBuilder.DropIndex(
                name: "IX_GatePasses_VisitorEntryId",
                table: "GatePasses");

            migrationBuilder.DropColumn(
                name: "VisitorEntryId",
                table: "GatePasses");

            // Re-enable FK enforcement
            migrationBuilder.Sql("PRAGMA foreign_keys = ON;", suppressTransaction: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VisitorEntryId",
                table: "GatePasses",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "VisitorsEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CompanyName = table.Column<string>(type: "TEXT", nullable: true),
                    EmployeeName = table.Column<string>(type: "TEXT", nullable: true),
                    EntryDateTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    GatePassId = table.Column<int>(type: "INTEGER", nullable: true),
                    PurposeOfVisit = table.Column<string>(type: "TEXT", nullable: true),
                    VehicleNo = table.Column<string>(type: "TEXT", nullable: true),
                    VehicleType = table.Column<string>(type: "TEXT", nullable: true),
                    VisitDateTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    VisitEndDateTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    VisitorMobileNo = table.Column<string>(type: "TEXT", nullable: true),
                    VisitorName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitorsEntries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Punches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VisitorEntryId = table.Column<int>(type: "INTEGER", nullable: false),
                    PunchType = table.Column<int>(type: "INTEGER", nullable: false),
                    TimestampUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Punches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Punches_VisitorsEntries_VisitorEntryId",
                        column: x => x.VisitorEntryId,
                        principalTable: "VisitorsEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GatePasses_VisitorEntryId",
                table: "GatePasses",
                column: "VisitorEntryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Punches_VisitorEntryId",
                table: "Punches",
                column: "VisitorEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitorsEntries_VisitDateTime",
                table: "VisitorsEntries",
                column: "VisitDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_VisitorsEntries_VisitEndDateTime",
                table: "VisitorsEntries",
                column: "VisitEndDateTime");

            migrationBuilder.AddForeignKey(
                name: "FK_GatePasses_VisitorsEntries_VisitorEntryId",
                table: "GatePasses",
                column: "VisitorEntryId",
                principalTable: "VisitorsEntries",
                principalColumn: "Id");
        }
    }
}
