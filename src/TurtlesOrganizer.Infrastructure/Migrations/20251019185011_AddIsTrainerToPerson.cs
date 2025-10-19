using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TurtlesOrganizer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsTrainerToPerson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsTrainer",
                table: "Persons",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql("UPDATE \"Persons\" SET \"IsTrainer\" = true;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsTrainer",
                table: "Persons");
        }
    }
}
