using Microsoft.EntityFrameworkCore.Migrations;

namespace Bangkok.EF.Migrations
{
    public partial class AddedLogDataTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Log.Data",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ErrorType = table.Column<string>(type: "varchar(255)", nullable: true),
                    Description = table.Column<string>(type: "varchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Log.Data", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Log.Data");
        }
    }
}
