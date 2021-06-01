using Microsoft.EntityFrameworkCore.Migrations;

namespace InternalIssues.Migrations
{
    public partial class fixedTicketTypeIssues : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_TicketTypes_TicketTypeId1",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_TicketTypeId1",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "TicketId",
                table: "TicketTypes");

            migrationBuilder.DropColumn(
                name: "TicketTypeId1",
                table: "Tickets");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_TicketTypeId",
                table: "Tickets",
                column: "TicketTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_TicketTypes_TicketTypeId",
                table: "Tickets",
                column: "TicketTypeId",
                principalTable: "TicketTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_TicketTypes_TicketTypeId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_TicketTypeId",
                table: "Tickets");

            migrationBuilder.AddColumn<int>(
                name: "TicketId",
                table: "TicketTypes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TicketTypeId1",
                table: "Tickets",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_TicketTypeId1",
                table: "Tickets",
                column: "TicketTypeId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_TicketTypes_TicketTypeId1",
                table: "Tickets",
                column: "TicketTypeId1",
                principalTable: "TicketTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
