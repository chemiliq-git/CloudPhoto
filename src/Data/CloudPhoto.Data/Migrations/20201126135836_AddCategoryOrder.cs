namespace CloudPhoto.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class AddCategoryOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "Categories",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "Categories");
        }
    }
}
