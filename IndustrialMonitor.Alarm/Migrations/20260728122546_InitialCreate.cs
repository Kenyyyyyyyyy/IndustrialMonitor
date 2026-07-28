using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IndustrialMonitor.Alarm.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AlarmRecords",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DeviceId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AlarmLevel = table.Column<int>(type: "INTEGER", nullable: false),
                    AlarmCode = table.Column<int>(type: "INTEGER", nullable: false),
                    AlarmItem = table.Column<int>(type: "INTEGER", nullable: false),
                    Message = table.Column<string>(type: "TEXT", nullable: false),
                    DateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsRecovered = table.Column<bool>(type: "INTEGER", nullable: false),
                    RecoveredAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlarmRecords", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlarmRecords");
        }
    }
}
