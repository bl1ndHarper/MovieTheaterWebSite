using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieTheater.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHallSeatStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "HallSeats",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "HallSeats");
        }
    }
}
