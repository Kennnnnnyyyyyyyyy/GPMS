using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gate_Pass_management.Migrations
{
    /// <inheritdoc />
    public partial class Scheduler_Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Office",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    TimeZoneId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    BusinessStart = table.Column<TimeOnly>(type: "TEXT", nullable: false),
                    BusinessEnd = table.Column<TimeOnly>(type: "TEXT", nullable: false),
                    WorkDays = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Office", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SchedulerRooms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OfficeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Capacity = table.Column<int>(type: "INTEGER", nullable: false),
                    EquipmentJson = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchedulerRooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchedulerRooms_Office_OfficeId",
                        column: x => x.OfficeId,
                        principalTable: "Office",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SchedulerMeetings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Subject = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    OrganizerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OfficeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RoomId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Start = table.Column<DateTime>(type: "TEXT", nullable: false),
                    End = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchedulerMeetings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchedulerMeetings_Employees_OrganizerId",
                        column: x => x.OrganizerId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SchedulerMeetings_Office_OfficeId",
                        column: x => x.OfficeId,
                        principalTable: "Office",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SchedulerMeetings_SchedulerRooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "SchedulerRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "MeetingAttendees",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    MeetingId = table.Column<Guid>(type: "TEXT", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ResponseStatus = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeetingAttendees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MeetingAttendees_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MeetingAttendees_SchedulerMeetings_MeetingId",
                        column: x => x.MeetingId,
                        principalTable: "SchedulerMeetings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MeetingAttendees_EmployeeId",
                table: "MeetingAttendees",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingAttendees_MeetingId_EmployeeId",
                table: "MeetingAttendees",
                columns: new[] { "MeetingId", "EmployeeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Office_Name",
                table: "Office",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SchedulerMeetings_OfficeId_RoomId_Start",
                table: "SchedulerMeetings",
                columns: new[] { "OfficeId", "RoomId", "Start" });

            migrationBuilder.CreateIndex(
                name: "IX_SchedulerMeetings_OrganizerId",
                table: "SchedulerMeetings",
                column: "OrganizerId");

            migrationBuilder.CreateIndex(
                name: "IX_SchedulerMeetings_RoomId",
                table: "SchedulerMeetings",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_SchedulerMeetings_Start",
                table: "SchedulerMeetings",
                column: "Start");

            migrationBuilder.CreateIndex(
                name: "IX_SchedulerRooms_OfficeId",
                table: "SchedulerRooms",
                column: "OfficeId");

            migrationBuilder.CreateIndex(
                name: "IX_SchedulerRooms_OfficeId_Name",
                table: "SchedulerRooms",
                columns: new[] { "OfficeId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MeetingAttendees");

            migrationBuilder.DropTable(
                name: "SchedulerMeetings");

            migrationBuilder.DropTable(
                name: "SchedulerRooms");

            migrationBuilder.DropTable(
                name: "Office");
        }
    }
}
