using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubsidySystem.Migrations
{
    /// <inheritdoc />
    public partial class AddApprovedFieldsToPaymentRegistry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "approved_by",
                table: "payment_registries",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "approved_date",
                table: "payment_registries",
                type: "date",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "approved_by",
                table: "payment_registries");

            migrationBuilder.DropColumn(
                name: "approved_date",
                table: "payment_registries");
        }
    }
}
