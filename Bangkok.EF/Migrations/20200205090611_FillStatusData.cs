using Microsoft.EntityFrameworkCore.Migrations;

namespace Bangkok.EF.Migrations
{
    public partial class FillStatusData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"INSERT INTO [enum.Status] (ID, StatusInitial) 
VALUES (1, 'A')
, (2, 'R')
, (3, 'D')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DELETE FROM [enum.Status] WHERE ID IN (1, 2, 3)");
        }
    }
}
