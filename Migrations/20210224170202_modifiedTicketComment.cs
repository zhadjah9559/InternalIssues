using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace InternalIssues.Migrations
{
    public partial class modifiedTicketComment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketComments_Tickets_TicketId",
                table: "TicketComments");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "TicketComments",
                newName: "AppUserId");

            migrationBuilder.AlterColumn<int>(
                name: "TicketId",
                table: "TicketComments",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CommentBody",
                table: "TicketComments",
                type: "character varying(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "TicketComments",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Updated",
                table: "TicketComments",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketComments_AppUserId",
                table: "TicketComments",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketComments_AspNetUsers_AppUserId",
                table: "TicketComments",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketComments_Tickets_TicketId",
                table: "TicketComments",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketComments_AspNetUsers_AppUserId",
                table: "TicketComments");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketComments_Tickets_TicketId",
                table: "TicketComments");

            migrationBuilder.DropIndex(
                name: "IX_TicketComments_AppUserId",
                table: "TicketComments");

            migrationBuilder.DropColumn(
                name: "CommentBody",
                table: "TicketComments");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "TicketComments");

            migrationBuilder.DropColumn(
                name: "Updated",
                table: "TicketComments");

            migrationBuilder.RenameColumn(
                name: "AppUserId",
                table: "TicketComments",
                newName: "Name");

            migrationBuilder.AlterColumn<int>(
                name: "TicketId",
                table: "TicketComments",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketComments_Tickets_TicketId",
                table: "TicketComments",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
