using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SaigonRide.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "StationId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Vehicles",
                keyColumn: "VehicleId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Vehicles",
                keyColumn: "VehicleId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Vehicles",
                keyColumn: "VehicleId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Vehicles",
                keyColumn: "VehicleId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Vehicles",
                keyColumn: "VehicleId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Vehicles",
                keyColumn: "VehicleId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Vehicles",
                keyColumn: "VehicleId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Vehicles",
                keyColumn: "VehicleId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Vehicles",
                keyColumn: "VehicleId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Vehicles",
                keyColumn: "VehicleId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "StationId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "StationId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "StationId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "StationId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "StationId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "StationId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "StationId",
                keyValue: 8);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Stations",
                columns: new[] { "StationId", "Capacity", "CurrentInventory", "Latitude", "Location", "Longitude", "StationName", "Status" },
                values: new object[,]
                {
                    { 1, 50, 25, null, "District 1", null, "Ben Thanh Market", "Active" },
                    { 2, 45, 11, null, "District 1", null, "Nguyen Hue Walking St", "Active" },
                    { 3, 25, 1, null, "Binh Thanh District", null, "Landmark 81", "Active" },
                    { 4, 40, 7, null, "District 7", null, "Tan Hung", "Active" },
                    { 5, 30, 2, null, "Phu Nhuan District", null, "Phu Nhuan Station", "Active" },
                    { 6, 50, 48, null, "Thu Duc City", null, "Thu Duc Station", "Active" },
                    { 7, 20, 10, null, "District 1", null, "Bui Vien Street", "Active" },
                    { 8, 35, 20, null, "District 3", null, "Notre-Dame Cathedral", "Active" }
                });

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

            migrationBuilder.InsertData(
                table: "Vehicles",
                columns: new[] { "VehicleId", "CategoryId", "StationId", "Status", "VehicleCode", "VehicleName" },
                values: new object[,]
                {
                    { 1, 1, 1, 0, "SB-V3-0017", "Bike VN-01" },
                    { 2, 2, 4, 1, "ES-F4-0042", "E-Scooter CT-01" },
                    { 3, 1, 2, 2, "ES-A1-0079", "Bike AG-01" },
                    { 4, 2, 3, 2, "SB-M1-001", "E-Scooter CM-01" },
                    { 5, 1, 4, 1, "SB-NS-0339", "Bike TN-01" },
                    { 6, 2, 5, 1, "ES-E7-8386", "E-Scooter DB-01" },
                    { 7, 1, 7, 0, "SB-T9-0099", "Bike TW-01" },
                    { 8, 2, 1, 0, "ES-D2-0011", "E-Scooter VN-01" },
                    { 9, 1, 7, 0, "SB-K1-0055", "Bike BV-01" },
                    { 10, 2, 8, 0, "ES-L3-0088", "E-Scooter ND-01" }
                });
        }
    }
}
