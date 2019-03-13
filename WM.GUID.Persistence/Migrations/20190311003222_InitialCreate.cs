using Microsoft.EntityFrameworkCore.Migrations;

namespace WM.GUID.Persistence.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GUIDs",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Expire = table.Column<long>(nullable: true),
                    User = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GUIDs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GUIDs");
        }
    }
}
