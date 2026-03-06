using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeLab.Infrastructure.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class Add_AssignedDate_And_Indexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "assigned_date",
                table: "exercises",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.CreateIndex(
                name: "ix_exercises_assigned_date",
                table: "exercises",
                column: "assigned_date",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_exercises_slug",
                table: "exercises",
                column: "slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_exercises_assigned_date",
                table: "exercises");

            migrationBuilder.DropIndex(
                name: "ix_exercises_slug",
                table: "exercises");

            migrationBuilder.DropColumn(
                name: "assigned_date",
                table: "exercises");
        }
    }
}
