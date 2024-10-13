using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TimetableService.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "timetables",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    hospital_id = table.Column<int>(type: "integer", nullable: false),
                    doctor_id = table.Column<string>(type: "text", nullable: false),
                    from = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    to = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    room = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_timetables", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "appointment",
                columns: table => new
                {
                    timetable_id = table.Column<int>(type: "integer", nullable: false),
                    time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    user_id = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_appointment", x => new { x.timetable_id, x.time });
                    table.ForeignKey(
                        name: "fk_appointment_timetables_timetable_id",
                        column: x => x.timetable_id,
                        principalTable: "timetables",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "appointment");

            migrationBuilder.DropTable(
                name: "timetables");
        }
    }
}
