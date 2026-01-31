using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LaundrySystem.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceMessageFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRead",
                table: "ServiceMessages");

            migrationBuilder.RenameColumn(
                name: "Message",
                table: "ServiceMessages",
                newName: "Body");

            migrationBuilder.AddColumn<DateTime>(
                name: "ActiveFrom",
                table: "ServiceMessages",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ActiveTo",
                table: "ServiceMessages",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Severity",
                table: "ServiceMessages",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "ServiceMessages",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActiveFrom",
                table: "ServiceMessages");

            migrationBuilder.DropColumn(
                name: "ActiveTo",
                table: "ServiceMessages");

            migrationBuilder.DropColumn(
                name: "Severity",
                table: "ServiceMessages");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "ServiceMessages");

            migrationBuilder.RenameColumn(
                name: "Body",
                table: "ServiceMessages",
                newName: "Message");

            migrationBuilder.AddColumn<bool>(
                name: "IsRead",
                table: "ServiceMessages",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
