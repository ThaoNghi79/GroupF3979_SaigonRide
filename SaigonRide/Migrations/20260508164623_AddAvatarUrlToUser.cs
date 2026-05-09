using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SaigonRide.Migrations
{
    /// <inheritdoc />
    public partial class AddAvatarUrlToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AvatarUrl",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "AvatarUrl", "PasswordHash" },
                values: new object[] { null, "$2a$11$HRJMHXlBTBb.PZkUunBe9OSAPt6KueY5xFWv/nA8NRZdLcL7etI9S" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                columns: new[] { "AvatarUrl", "PasswordHash" },
                values: new object[] { null, "$2a$11$XcWehKSZ1P.Rw3uj30BnrOdinqYLbofJTYcQRywVcZxBV8PQ49VjC" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                columns: new[] { "AvatarUrl", "PasswordHash" },
                values: new object[] { null, "$2a$11$TmUKLZwjUbmF9qql/.DZceBdNsPIen9mmEW3HYnatr3q8x2U3ffLO" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarUrl",
                table: "Users");

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
    }
}
