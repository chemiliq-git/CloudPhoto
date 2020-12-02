﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CloudPhoto.Data.Migrations
{
    public partial class FixManyToMany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ImageTags",
                table: "ImageTags");

            migrationBuilder.DropIndex(
                name: "IX_ImageTags_ImageId",
                table: "ImageTags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ImageCategories",
                table: "ImageCategories");

            migrationBuilder.DropIndex(
                name: "IX_ImageCategories_ImageId",
                table: "ImageCategories");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ImageTags");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ImageCategories");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ImageTags",
                table: "ImageTags",
                columns: new[] { "ImageId", "TagId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ImageCategories",
                table: "ImageCategories",
                columns: new[] { "ImageId", "CategoryId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ImageTags_Tags_ImageId",
                table: "ImageTags",
                column: "ImageId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImageTags_Tags_ImageId",
                table: "ImageTags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ImageTags",
                table: "ImageTags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ImageCategories",
                table: "ImageCategories");

            migrationBuilder.AddColumn<string>(
                name: "ImageId",
                table: "Tags",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "ImageTags",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "ImageTags",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "ImageTags",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "ImageCategories",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "ImageCategories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "ImageCategories",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageId",
                table: "Categories",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ImageTags",
                table: "ImageTags",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ImageCategories",
                table: "ImageCategories",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_ImageId",
                table: "Tags",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_ImageTags_ImageId",
                table: "ImageTags",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_ImageTags_TagId",
                table: "ImageTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_ImageCategories_ImageId",
                table: "ImageCategories",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ImageId",
                table: "Categories",
                column: "ImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Images_ImageId",
                table: "Categories",
                column: "ImageId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ImageTags_Tags_TagId",
                table: "ImageTags",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Images_ImageId",
                table: "Tags",
                column: "ImageId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}