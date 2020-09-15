using Microsoft.EntityFrameworkCore.Migrations;

namespace UserDataAPIApp.Migrations
{
    public partial class UpdatedSSN : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "SocialSecurityNumber",
                table: "AspNetUsers",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "SocialSecurityNumber",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                oldClrType: typeof(long));
        }
    }
}
