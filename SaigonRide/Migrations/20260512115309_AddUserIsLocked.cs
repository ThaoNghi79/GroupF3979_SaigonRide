using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SaigonRide.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIsLocked : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLocked",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "IsLocked", "PasswordHash" },
                values: new object[] { false, "$2a$11$vLe9Wi0Y1ym2JPHaq4qSEeWXxZVpyvyOjCtMkTNXqkfKXzertYd/." });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                columns: new[] { "IsLocked", "PasswordHash" },
                values: new object[] { false, "$2a$11$1K1sJ42.BWDH5dHCarK4dOCkX5Crc9UVGuQaT8gx17TDC5FcZlGL." });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                columns: new[] { "IsLocked", "PasswordHash" },
                values: new object[] { false, "$2a$11$KF.kKUqRbxDY0ZRV7X68FuD8MDwC6bm0V9upe9QHFumeymF6Sv9ZC" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLocked",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$RvGqYUdZdemAHVoXNE5jzeJFY4p0o/MZgoW9A3mKBYgdu65JPm/Xy");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$rzmYvWipIbU9NXXBJ65MQemnEeX7wvzuXbUcIsL8teS2dCTeniRkm");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "PasswordHash",
                value: "$2a$11$RNkhu8/ZlPLWZXsnAFnua.GHpuMXs/nNjtaGoPmqtAb/doFW4M2di");
        }
    }
}
