using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IndustrialMonitor.Alarm.Migrations
{
    /// <inheritdoc />
    public partial class AddNullableFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "AlarmRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<ushort>(
                name: "Port",
                table: "AlarmRecords",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SlaveId",
                table: "AlarmRecords",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IpAddress",
                table: "AlarmRecords");

            migrationBuilder.DropColumn(
                name: "Port",
                table: "AlarmRecords");

            migrationBuilder.DropColumn(
                name: "SlaveId",
                table: "AlarmRecords");
        }
    }
}
