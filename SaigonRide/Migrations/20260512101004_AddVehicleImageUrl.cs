using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SaigonRide.Migrations
{
    /// <inheritdoc />
    public partial class AddVehicleImageUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Vehicles",
                type: "nvarchar(max)",
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Vehicles");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$6/hXc/GUyF65J9S3/XvHqeL8B3yv/jLh9oyJyOA3a1YKwA..QHl4W");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$LGgTIC/b2aHMPdvuiH3YOupihTg9sQKKPpAfML62KsRaCYLaHEMw2");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "PasswordHash",
                value: "$2a$11$Drdd.Yb7xZS4iQqqKchUh.psgKANOzG1CW1Z2MduyhKJmPSyrhDL6");
        }
    }
}
