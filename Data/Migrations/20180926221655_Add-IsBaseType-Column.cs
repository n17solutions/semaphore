using Microsoft.EntityFrameworkCore.Migrations;

namespace N17Solutions.Semaphore.Data.Migrations
{
    public partial class AddIsBaseTypeColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBaseType",
                table: "Signal",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBaseType",
                table: "Signal");
        }
    }
}
