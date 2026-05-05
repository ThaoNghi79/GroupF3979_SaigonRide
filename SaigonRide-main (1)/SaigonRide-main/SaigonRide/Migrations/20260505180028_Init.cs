using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SaigonRide.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$G0qxpgmafO7dr4VwzSTRnucE/A9hl6BHOJo.QJ5C.IsMuuQ6yBuUq");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$zMKPnhvx87A0iI9TetHkZOoUO0hP6W.lJG0ijEpmqrytPxI0oqEke");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "PasswordHash",
                value: "$2a$11$Q3ApMuJyKP2QIS05S10cOe3Z75TwD3uUiny.1F5NMrjhnZIPE8Obq");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$byhMstsucbqn4uHhCWkkgOBfWPzYNBG9wpcDbDLsMQ3E6RMg8mQuK");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$zqcBA6S2CKLxgk9izzeh1uRTlbC36ed9OKuJiJKqlv3/7Cmy6Xggi");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "PasswordHash",
                value: "$2a$11$gZ5fBrgt4VLU6ZHPgk3wVuqvr.3raRzuxHrOKMFFe0/M/3c.pMKUO");
        }
    }
}
