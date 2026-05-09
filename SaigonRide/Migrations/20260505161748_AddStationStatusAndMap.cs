using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SaigonRide.Migrations
{
    /// <inheritdoc />
    public partial class AddStationStatusAndMap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Stations",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Stations",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Stations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Stations",
                keyColumn: "StationId",
                keyValue: 1,
                columns: new[] { "Latitude", "Longitude", "Status" },
                values: new object[] { null, null, "Active" });

            migrationBuilder.UpdateData(
                table: "Stations",
                keyColumn: "StationId",
                keyValue: 2,
                columns: new[] { "Latitude", "Longitude", "Status" },
                values: new object[] { null, null, "Active" });

            migrationBuilder.UpdateData(
                table: "Stations",
                keyColumn: "StationId",
                keyValue: 3,
                columns: new[] { "Latitude", "Longitude", "Status" },
                values: new object[] { null, null, "Active" });

            migrationBuilder.UpdateData(
                table: "Stations",
                keyColumn: "StationId",
                keyValue: 4,
                columns: new[] { "Latitude", "Longitude", "Status" },
                values: new object[] { null, null, "Active" });

            migrationBuilder.UpdateData(
                table: "Stations",
                keyColumn: "StationId",
                keyValue: 5,
                columns: new[] { "Latitude", "Longitude", "Status" },
                values: new object[] { null, null, "Active" });

            migrationBuilder.UpdateData(
                table: "Stations",
                keyColumn: "StationId",
                keyValue: 6,
                columns: new[] { "Latitude", "Longitude", "Status" },
                values: new object[] { null, null, "Active" });

            migrationBuilder.UpdateData(
                table: "Stations",
                keyColumn: "StationId",
                keyValue: 7,
                columns: new[] { "Latitude", "Longitude", "Status" },
                values: new object[] { null, null, "Active" });

            migrationBuilder.UpdateData(
                table: "Stations",
                keyColumn: "StationId",
                keyValue: 8,
                columns: new[] { "Latitude", "Longitude", "Status" },
                values: new object[] { null, null, "Active" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$vA8oUo.hP36376FYrWBcSe2Pe6/Lh/KLXLUPEDIOgtpvS7ErgfQgC");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$kcXB79TmzsSLsyPaSoKteeLAax2w1OdcOHsmvpXo5plVrKR9W4ycG");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "PasswordHash",
                value: "$2a$11$gBZWcMkRztGQm...KPZohu/Nw4BvubfHEp2GdQS.7upepKxZfwHM6");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Stations");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Stations");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Stations");

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
