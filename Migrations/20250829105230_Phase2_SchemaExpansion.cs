using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gate_Pass_management.Migrations
{
    /// <inheritdoc />
    public partial class Phase2_SchemaExpansion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "VisitEndDateTime",
                table: "VisitorsEntries",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AddColumn<int>(
                name: "GatePassId",
                table: "VisitorsEntries",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SiteId",
                table: "VisitorsEntries",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "InDateTime",
                table: "LocalODs",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ActorUserId = table.Column<string>(type: "TEXT", nullable: true),
                    Action = table.Column<string>(type: "TEXT", nullable: false),
                    EntityName = table.Column<string>(type: "TEXT", nullable: false),
                    EntityId = table.Column<string>(type: "TEXT", nullable: true),
                    DataJson = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
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

            migrationBuilder.CreateTable(
                name: "Sites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    City = table.Column<string>(type: "TEXT", nullable: false),
                    Address = table.Column<string>(type: "TEXT", nullable: false),
                    Timezone = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sites", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SmsMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    To = table.Column<string>(type: "TEXT", nullable: false),
                    Body = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderMessageId = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SentUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Error = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmsMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GatePasses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PassNumber = table.Column<string>(type: "TEXT", nullable: false),
                    VisitorName = table.Column<string>(type: "TEXT", nullable: false),
                    Mobile = table.Column<string>(type: "TEXT", nullable: false),
                    ValidFromUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ValidToUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    QRCodeValue = table.Column<string>(type: "TEXT", nullable: true),
                    SiteId = table.Column<int>(type: "INTEGER", nullable: true),
                    VisitorEntryId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GatePasses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GatePasses_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GatePasses_VisitorsEntries_VisitorEntryId",
                        column: x => x.VisitorEntryId,
                        principalTable: "VisitorsEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SiteId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Capacity = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    Resources = table.Column<string>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rooms_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Meetings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoomId = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    OrganizerName = table.Column<string>(type: "TEXT", nullable: true),
                    StartUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsBlocked = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meetings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Meetings_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TimeBlocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SiteId = table.Column<int>(type: "INTEGER", nullable: true),
                    RoomId = table.Column<int>(type: "INTEGER", nullable: true),
                    Reason = table.Column<string>(type: "TEXT", nullable: true),
                    StartUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeBlocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimeBlocks_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TimeBlocks_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VisitorsEntries_GatePassId",
                table: "VisitorsEntries",
                column: "GatePassId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitorsEntries_SiteId",
                table: "VisitorsEntries",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitorsEntries_VisitDateTime",
                table: "VisitorsEntries",
                column: "VisitDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_VisitorsEntries_VisitEndDateTime",
                table: "VisitorsEntries",
                column: "VisitEndDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_CreatedUtc",
                table: "AuditLogs",
                column: "CreatedUtc");

            migrationBuilder.CreateIndex(
                name: "IX_GatePasses_PassNumber",
                table: "GatePasses",
                column: "PassNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GatePasses_SiteId",
                table: "GatePasses",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_GatePasses_VisitorEntryId",
                table: "GatePasses",
                column: "VisitorEntryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Meetings_RoomId_StartUtc",
                table: "Meetings",
                columns: new[] { "RoomId", "StartUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_Punches_VisitorEntryId_TimestampUtc",
                table: "Punches",
                columns: new[] { "VisitorEntryId", "TimestampUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_SiteId",
                table: "Rooms",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeBlocks_RoomId",
                table: "TimeBlocks",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeBlocks_SiteId",
                table: "TimeBlocks",
                column: "SiteId");

            migrationBuilder.AddForeignKey(
                name: "FK_VisitorsEntries_GatePasses_GatePassId",
                table: "VisitorsEntries",
                column: "GatePassId",
                principalTable: "GatePasses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VisitorsEntries_Sites_SiteId",
                table: "VisitorsEntries",
                column: "SiteId",
                principalTable: "Sites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VisitorsEntries_GatePasses_GatePassId",
                table: "VisitorsEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_VisitorsEntries_Sites_SiteId",
                table: "VisitorsEntries");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "GatePasses");

            migrationBuilder.DropTable(
                name: "Meetings");

            migrationBuilder.DropTable(
                name: "Punches");

            migrationBuilder.DropTable(
                name: "SmsMessages");

            migrationBuilder.DropTable(
                name: "TimeBlocks");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "Sites");

            migrationBuilder.DropIndex(
                name: "IX_VisitorsEntries_GatePassId",
                table: "VisitorsEntries");

            migrationBuilder.DropIndex(
                name: "IX_VisitorsEntries_SiteId",
                table: "VisitorsEntries");

            migrationBuilder.DropIndex(
                name: "IX_VisitorsEntries_VisitDateTime",
                table: "VisitorsEntries");

            migrationBuilder.DropIndex(
                name: "IX_VisitorsEntries_VisitEndDateTime",
                table: "VisitorsEntries");

            migrationBuilder.DropColumn(
                name: "GatePassId",
                table: "VisitorsEntries");

            migrationBuilder.DropColumn(
                name: "SiteId",
                table: "VisitorsEntries");

            migrationBuilder.AlterColumn<DateTime>(
                name: "VisitEndDateTime",
                table: "VisitorsEntries",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "InDateTime",
                table: "LocalODs",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);
        }
    }
}
