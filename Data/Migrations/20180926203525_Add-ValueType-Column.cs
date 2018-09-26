using Microsoft.EntityFrameworkCore.Migrations;

namespace N17Solutions.Semaphore.Data.Migrations
{
    public partial class AddValueTypeColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ValueType",
                table: "Signal",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValueType",
                table: "Signal");
        }
    }
}
