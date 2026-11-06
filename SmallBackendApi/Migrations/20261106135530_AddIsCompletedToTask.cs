using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmallBackendApi.Migrations
{
    /// <inheritdoc />
    public partial class AddIsCompletedToTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "EmployeeTasks",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "EmployeeTasks");
        }
    }
}
