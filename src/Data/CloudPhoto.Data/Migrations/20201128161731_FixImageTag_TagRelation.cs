using Microsoft.EntityFrameworkCore.Migrations;

namespace CloudPhoto.Data.Migrations
{
    public partial class FixImageTag_TagRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImageTags_Tags_ImageId",
                table: "ImageTags");

            migrationBuilder.CreateIndex(
                name: "IX_ImageTags_TagId",
                table: "ImageTags",
                column: "TagId");

            migrationBuilder.AddForeignKey(
                name: "FK_ImageTags_Tags_TagId",
                table: "ImageTags",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImageTags_Tags_TagId",
                table: "ImageTags");

            migrationBuilder.DropIndex(
                name: "IX_ImageTags_TagId",
                table: "ImageTags");

            migrationBuilder.AddForeignKey(
                name: "FK_ImageTags_Tags_ImageId",
                table: "ImageTags",
                column: "ImageId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
