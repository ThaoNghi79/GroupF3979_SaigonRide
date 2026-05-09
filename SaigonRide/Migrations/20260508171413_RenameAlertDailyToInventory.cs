using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SaigonRide.Migrations
{
    /// <inheritdoc />
    public partial class RenameAlertDailyToInventory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AlertDailyRevenue",
                table: "Users",
                newName: "AlertStationInventory");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$.39DfCvM2Wc42.OkNKiHxu9Z2Mb/cDtXUnLKUbJ9D8945Ejtl0djW");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$ku1H6/aecpK6MKiCmI9ZI.p3I5xg3w3ZTskl5HQw3KanzDQu04k2m");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "PasswordHash",
                value: "$2a$11$wTIBukldxOYQy4rR.gjMiuZAJJj17WPiu67WrLY.t5BbaQAyGe9fq");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AlertStationInventory",
                table: "Users",
                newName: "AlertDailyRevenue");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$KLZkhtJN5ItSLRYmU/3KBeAByLjujIl8PTomMUkAGLab3YulVVFfK");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$5fzPyHpf2RWJZzQNnnRWU.h6eRjsXxkkEjLUA4uClHBAPmh3/X9fm");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "PasswordHash",
                value: "$2a$11$Sv/AOROOS.MPa7TqAK8Z9Ovu1EEutOiLkQc5jcURjzoI15ebt2bDO");
        }
    }
}
