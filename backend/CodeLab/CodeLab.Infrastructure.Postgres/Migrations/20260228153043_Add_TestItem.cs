using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeLab.Infrastructure.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class Add_TestItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "test_results",
                table: "submissions",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "test_results",
                table: "submissions");
        }
    }
}
