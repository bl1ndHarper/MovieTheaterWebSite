using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieTheater.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMovieDirectorFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DirectorDetailsUrl",
                table: "Movies",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DirectorName",
                table: "Movies",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DirectorDetailsUrl",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "DirectorName",
                table: "Movies");
        }
    }
}
