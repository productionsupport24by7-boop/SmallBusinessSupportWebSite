using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupportOps.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedByUserIdToTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Tickets");
        }
    }
}
