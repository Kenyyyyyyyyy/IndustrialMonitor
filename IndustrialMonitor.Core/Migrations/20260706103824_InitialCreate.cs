using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IndustrialMonitor.Core.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeviceDataModels",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    value00 = table.Column<int>(type: "int", nullable: false),
                    value01 = table.Column<int>(type: "int", nullable: false),
                    value02 = table.Column<int>(type: "int", nullable: false),
                    value03 = table.Column<int>(type: "int", nullable: false),
                    value04 = table.Column<int>(type: "int", nullable: false),
                    value05 = table.Column<int>(type: "int", nullable: false),
                    value06 = table.Column<int>(type: "int", nullable: false),
                    value07 = table.Column<int>(type: "int", nullable: false),
                    value08 = table.Column<int>(type: "int", nullable: false),
                    value09 = table.Column<int>(type: "int", nullable: false),
                    value10 = table.Column<int>(type: "int", nullable: false),
                    value11 = table.Column<int>(type: "int", nullable: false),
                    value12 = table.Column<int>(type: "int", nullable: false),
                    value13 = table.Column<int>(type: "int", nullable: false),
                    value14 = table.Column<int>(type: "int", nullable: false),
                    value15 = table.Column<int>(type: "int", nullable: false),
                    value16 = table.Column<int>(type: "int", nullable: false),
                    value17 = table.Column<int>(type: "int", nullable: false),
                    value18 = table.Column<int>(type: "int", nullable: false),
                    value19 = table.Column<int>(type: "int", nullable: false),
                    value20 = table.Column<int>(type: "int", nullable: false),
                    value21 = table.Column<int>(type: "int", nullable: false),
                    value22 = table.Column<int>(type: "int", nullable: false),
                    value23 = table.Column<int>(type: "int", nullable: false),
                    value24 = table.Column<int>(type: "int", nullable: false),
                    value25 = table.Column<int>(type: "int", nullable: false),
                    value26 = table.Column<int>(type: "int", nullable: false),
                    value27 = table.Column<int>(type: "int", nullable: false),
                    value28 = table.Column<int>(type: "int", nullable: false),
                    value29 = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceDataModels", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceDataModels");
        }
    }
}
