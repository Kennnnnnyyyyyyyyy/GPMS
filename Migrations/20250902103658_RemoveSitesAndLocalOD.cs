using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gate_Pass_management.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSitesAndLocalOD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GatePasses_Sites_SiteId",
                table: "GatePasses");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Sites_SiteId",
                table: "Rooms");

            migrationBuilder.DropForeignKey(
                name: "FK_TimeBlock_Sites_SiteId",
                table: "TimeBlock");

            migrationBuilder.DropForeignKey(
                name: "FK_VisitorAppointments_Sites_SiteId",
                table: "VisitorAppointments");

            migrationBuilder.DropForeignKey(
                name: "FK_VisitorsEntries_Sites_SiteId",
                table: "VisitorsEntries");

            migrationBuilder.DropTable(
                name: "LocalODs");

            migrationBuilder.DropTable(
                name: "Sites");

            migrationBuilder.DropIndex(
                name: "IX_VisitorsEntries_SiteId",
                table: "VisitorsEntries");

            migrationBuilder.DropIndex(
                name: "IX_VisitorAppointments_SiteId",
                table: "VisitorAppointments");

            migrationBuilder.DropIndex(
                name: "IX_TimeBlock_SiteId",
                table: "TimeBlock");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_SiteId",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_GatePasses_SiteId",
                table: "GatePasses");

            migrationBuilder.DropColumn(
                name: "SiteId",
                table: "VisitorsEntries");

            migrationBuilder.DropColumn(
                name: "SiteId",
                table: "VisitorAppointments");

            migrationBuilder.DropColumn(
                name: "SiteId",
                table: "TimeBlock");

            migrationBuilder.DropColumn(
                name: "SiteId",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "SiteId",
                table: "GatePasses");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Rooms",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "Rooms");

            migrationBuilder.AddColumn<int>(
                name: "SiteId",
                table: "VisitorsEntries",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SiteId",
                table: "VisitorAppointments",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SiteId",
                table: "TimeBlock",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SiteId",
                table: "Rooms",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SiteId",
                table: "GatePasses",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LocalODs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmployeeName = table.Column<string>(type: "TEXT", nullable: true),
                    EmployeeNo = table.Column<string>(type: "TEXT", nullable: true),
                    InDateTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    OutDateTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PurposeOfVisit = table.Column<string>(type: "TEXT", nullable: true),
                    TypeOfLocalOD = table.Column<string>(type: "TEXT", nullable: true),
                    TypeOfVisit = table.Column<int>(type: "INTEGER", nullable: false),
                    VisitLocation = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalODs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Address = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    City = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Timezone = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sites", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VisitorsEntries_SiteId",
                table: "VisitorsEntries",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitorAppointments_SiteId",
                table: "VisitorAppointments",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeBlock_SiteId",
                table: "TimeBlock",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_SiteId",
                table: "Rooms",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_GatePasses_SiteId",
                table: "GatePasses",
                column: "SiteId");

            migrationBuilder.AddForeignKey(
                name: "FK_GatePasses_Sites_SiteId",
                table: "GatePasses",
                column: "SiteId",
                principalTable: "Sites",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Sites_SiteId",
                table: "Rooms",
                column: "SiteId",
                principalTable: "Sites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TimeBlock_Sites_SiteId",
                table: "TimeBlock",
                column: "SiteId",
                principalTable: "Sites",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VisitorAppointments_Sites_SiteId",
                table: "VisitorAppointments",
                column: "SiteId",
                principalTable: "Sites",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VisitorsEntries_Sites_SiteId",
                table: "VisitorsEntries",
                column: "SiteId",
                principalTable: "Sites",
                principalColumn: "Id");
        }
    }
}
