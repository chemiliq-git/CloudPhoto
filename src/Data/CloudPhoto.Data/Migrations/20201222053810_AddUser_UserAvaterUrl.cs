﻿// <auto-generated />
using Microsoft.EntityFrameworkCore.Migrations;

namespace CloudPhoto.Data.Migrations
{
    public partial class AddUser_UserAvaterUrl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserSubscribes_AspNetUsers_SubscribeToUserId",
                table: "UserSubscribes");

            migrationBuilder.DropIndex(
                name: "IX_UserSubscribes_SubscribeToUserId",
                table: "UserSubscribes");

            migrationBuilder.AlterColumn<string>(
                name: "SubscribeToUserId",
                table: "UserSubscribes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "UserAvaterUrl",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserAvaterUrl",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "SubscribeToUserId",
                table: "UserSubscribes",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_UserSubscribes_SubscribeToUserId",
                table: "UserSubscribes",
                column: "SubscribeToUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserSubscribes_AspNetUsers_SubscribeToUserId",
                table: "UserSubscribes",
                column: "SubscribeToUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}