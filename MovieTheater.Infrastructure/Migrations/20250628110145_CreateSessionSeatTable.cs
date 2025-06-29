using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MovieTheater.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateSessionSeatTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_HallSeats_SeatId",
                table: "Bookings");

            migrationBuilder.RenameColumn(
                name: "SeatId",
                table: "Bookings",
                newName: "SessionSeatId");

            migrationBuilder.RenameIndex(
                name: "IX_Bookings_SeatId",
                table: "Bookings",
                newName: "IX_Bookings_SessionSeatId");

            migrationBuilder.CreateTable(
                name: "SessionSeats",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SessionId = table.Column<long>(type: "bigint", nullable: false),
                    HallSeatId = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionSeats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SessionSeats_HallSeats_HallSeatId",
                        column: x => x.HallSeatId,
                        principalTable: "HallSeats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SessionSeats_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SessionSeats_HallSeatId",
                table: "SessionSeats",
                column: "HallSeatId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionSeats_SessionId",
                table: "SessionSeats",
                column: "SessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_SessionSeats_SessionSeatId",
                table: "Bookings",
                column: "SessionSeatId",
                principalTable: "SessionSeats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_SessionSeats_SessionSeatId",
                table: "Bookings");

            migrationBuilder.DropTable(
                name: "SessionSeats");

            migrationBuilder.RenameColumn(
                name: "SessionSeatId",
                table: "Bookings",
                newName: "SeatId");

            migrationBuilder.RenameIndex(
                name: "IX_Bookings_SessionSeatId",
                table: "Bookings",
                newName: "IX_Bookings_SeatId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_HallSeats_SeatId",
                table: "Bookings",
                column: "SeatId",
                principalTable: "HallSeats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
