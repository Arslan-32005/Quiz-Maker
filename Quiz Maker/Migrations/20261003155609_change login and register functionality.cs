using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quiz_Maker.Migrations
{
    /// <inheritdoc />
    public partial class changeloginandregisterfunctionality : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attempts_AspNetUsers_UserId1",
                table: "Attempts");

            migrationBuilder.DropForeignKey(
                name: "FK_Quizzes_AspNetUsers_UserId1",
                table: "Quizzes");

            migrationBuilder.DropIndex(
                name: "IX_Quizzes_UserId1",
                table: "Quizzes");

            migrationBuilder.DropIndex(
                name: "IX_Attempts_UserId1",
                table: "Attempts");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Attempts");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Quizzes",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "AttemptedAt",
                table: "Attempts",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Quizzes",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "Quizzes",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "AttemptedAt",
                table: "Attempts",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "Attempts",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_UserId1",
                table: "Quizzes",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_Attempts_UserId1",
                table: "Attempts",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Attempts_AspNetUsers_UserId1",
                table: "Attempts",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Quizzes_AspNetUsers_UserId1",
                table: "Quizzes",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
