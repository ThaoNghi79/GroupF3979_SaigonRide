using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SaigonRide.Migrations
{
    /// <inheritdoc />
    public partial class AddAlertSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AlertDailyRevenue",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AlertStationOverload",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AlertVehicleMaintenance",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "AlertDailyRevenue", "AlertStationOverload", "AlertVehicleMaintenance", "PasswordHash" },
                values: new object[] { false, false, false, "$2a$11$KLZkhtJN5ItSLRYmU/3KBeAByLjujIl8PTomMUkAGLab3YulVVFfK" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                columns: new[] { "AlertDailyRevenue", "AlertStationOverload", "AlertVehicleMaintenance", "PasswordHash" },
                values: new object[] { false, false, false, "$2a$11$5fzPyHpf2RWJZzQNnnRWU.h6eRjsXxkkEjLUA4uClHBAPmh3/X9fm" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                columns: new[] { "AlertDailyRevenue", "AlertStationOverload", "AlertVehicleMaintenance", "PasswordHash" },
                values: new object[] { false, false, false, "$2a$11$Sv/AOROOS.MPa7TqAK8Z9Ovu1EEutOiLkQc5jcURjzoI15ebt2bDO" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlertDailyRevenue",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AlertStationOverload",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AlertVehicleMaintenance",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$HRJMHXlBTBb.PZkUunBe9OSAPt6KueY5xFWv/nA8NRZdLcL7etI9S");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$XcWehKSZ1P.Rw3uj30BnrOdinqYLbofJTYcQRywVcZxBV8PQ49VjC");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "PasswordHash",
                value: "$2a$11$TmUKLZwjUbmF9qql/.DZceBdNsPIen9mmEW3HYnatr3q8x2U3ffLO");
        }
    }
}
