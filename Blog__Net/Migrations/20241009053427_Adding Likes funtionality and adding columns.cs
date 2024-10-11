using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blog__Net.Migrations
{
    /// <inheritdoc />
    public partial class AddingLikesfuntionalityandaddingcolumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "likesCount",
                table: "Posts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "likesCount",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "LikeDate",
                table: "PostLike");
        }
    }
}
