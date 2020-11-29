namespace CloudPhoto.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class AddVoteTable_Fix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Votes_Images_ImageId1",
                table: "Votes");

            migrationBuilder.DropIndex(
                name: "IX_Votes_ImageId1",
                table: "Votes");

            migrationBuilder.DropColumn(
                name: "ImageId1",
                table: "Votes");

            migrationBuilder.AlterColumn<string>(
                name: "ImageId",
                table: "Votes",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_ImageId",
                table: "Votes",
                column: "ImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_Images_ImageId",
                table: "Votes",
                column: "ImageId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Votes_Images_ImageId",
                table: "Votes");

            migrationBuilder.DropIndex(
                name: "IX_Votes_ImageId",
                table: "Votes");

            migrationBuilder.AlterColumn<int>(
                name: "ImageId",
                table: "Votes",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "ImageId1",
                table: "Votes",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Votes_ImageId1",
                table: "Votes",
                column: "ImageId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_Images_ImageId1",
                table: "Votes",
                column: "ImageId1",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
