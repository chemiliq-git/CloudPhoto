﻿// <auto-generated />
using Microsoft.EntityFrameworkCore.Migrations;

namespace CloudPhoto.Data.Migrations
{
    public partial class AddUser_UserAvaterUrl_FixName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserAvaterUrl",
                table: "AspNetUsers",
                newName: "UserAvatarUrl");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserAvatarUrl",
                table: "AspNetUsers",
                newName: "UserAvaterUrl");
        }
    }
}
