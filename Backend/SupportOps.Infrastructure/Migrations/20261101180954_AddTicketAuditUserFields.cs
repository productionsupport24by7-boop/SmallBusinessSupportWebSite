using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupportOps.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketAuditUserFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResolvedByUserId",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResolvedByUserId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Tickets");
        }
    }
}
