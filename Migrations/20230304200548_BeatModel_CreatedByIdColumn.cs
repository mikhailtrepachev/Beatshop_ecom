using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beatshop.Migrations
{
    /// <inheritdoc />
    public partial class BeatModel_CreatedByIdColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "Beats",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Beats");
        }
    }
}
