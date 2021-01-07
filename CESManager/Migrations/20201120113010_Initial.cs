using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CESManager.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordHash = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    PasswordSalt = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sessions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "PasswordHash", "PasswordSalt", "Username" },
                values: new object[] { 1, new byte[] { 169, 60, 148, 90, 194, 63, 110, 252, 103, 36, 102, 154, 27, 200, 48, 37, 188, 59, 26, 34, 63, 160, 238, 218, 149, 165, 61, 113, 27, 102, 49, 210, 13, 99, 93, 208, 16, 165, 245, 111, 95, 187, 114, 107, 229, 145, 145, 251, 4, 128, 99, 175, 112, 242, 144, 144, 253, 184, 237, 9, 184, 41, 25, 21 }, new byte[] { 168, 214, 215, 75, 87, 84, 178, 153, 18, 54, 180, 124, 72, 245, 107, 249, 247, 94, 159, 166, 176, 0, 18, 80, 116, 180, 189, 84, 129, 136, 214, 242, 178, 5, 31, 12, 246, 150, 24, 69, 37, 239, 153, 194, 187, 49, 104, 183, 189, 14, 81, 25, 113, 153, 1, 9, 186, 57, 15, 181, 159, 109, 248, 238, 173, 169, 153, 177, 145, 183, 127, 61, 116, 40, 220, 126, 10, 114, 11, 1, 4, 103, 22, 255, 232, 192, 112, 208, 168, 56, 117, 87, 0, 127, 46, 218, 156, 54, 54, 8, 54, 45, 135, 161, 252, 16, 83, 171, 63, 201, 37, 122, 232, 44, 46, 151, 6, 69, 22, 31, 6, 73, 50, 218, 125, 180, 172, 12 }, "User1" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "PasswordHash", "PasswordSalt", "Username" },
                values: new object[] { 2, new byte[] { 169, 60, 148, 90, 194, 63, 110, 252, 103, 36, 102, 154, 27, 200, 48, 37, 188, 59, 26, 34, 63, 160, 238, 218, 149, 165, 61, 113, 27, 102, 49, 210, 13, 99, 93, 208, 16, 165, 245, 111, 95, 187, 114, 107, 229, 145, 145, 251, 4, 128, 99, 175, 112, 242, 144, 144, 253, 184, 237, 9, 184, 41, 25, 21 }, new byte[] { 168, 214, 215, 75, 87, 84, 178, 153, 18, 54, 180, 124, 72, 245, 107, 249, 247, 94, 159, 166, 176, 0, 18, 80, 116, 180, 189, 84, 129, 136, 214, 242, 178, 5, 31, 12, 246, 150, 24, 69, 37, 239, 153, 194, 187, 49, 104, 183, 189, 14, 81, 25, 113, 153, 1, 9, 186, 57, 15, 181, 159, 109, 248, 238, 173, 169, 153, 177, 145, 183, 127, 61, 116, 40, 220, 126, 10, 114, 11, 1, 4, 103, 22, 255, 232, 192, 112, 208, 168, 56, 117, 87, 0, 127, 46, 218, 156, 54, 54, 8, 54, 45, 135, 161, 252, 16, 83, 171, 63, 201, 37, 122, 232, 44, 46, 151, 6, 69, 22, 31, 6, 73, 50, 218, 125, 180, 172, 12 }, "User2" });

            migrationBuilder.InsertData(
                table: "Sessions",
                columns: new[] { "Id", "EndDateTime", "StartDateTime", "UserId" },
                values: new object[] { 1, new DateTime(2020, 11, 19, 14, 30, 0, 0, DateTimeKind.Unspecified), new DateTime(2020, 11, 19, 14, 0, 0, 0, DateTimeKind.Unspecified), 1 });

            migrationBuilder.InsertData(
                table: "Sessions",
                columns: new[] { "Id", "EndDateTime", "StartDateTime", "UserId" },
                values: new object[] { 2, new DateTime(2020, 11, 19, 4, 30, 0, 0, DateTimeKind.Unspecified), new DateTime(2020, 11, 19, 4, 0, 0, 0, DateTimeKind.Unspecified), 2 });

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_UserId",
                table: "Sessions",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
