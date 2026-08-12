using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TcmbKurDonusturucu.Migrations
{
    /// <inheritdoc />
    public partial class AddGoogleId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SifreHash",
                table: "Kullanicilar",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "GoogleId",
                table: "Kullanicilar",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Kullanicilar_GoogleId",
                table: "Kullanicilar",
                column: "GoogleId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Kullanicilar_GoogleId",
                table: "Kullanicilar");

            migrationBuilder.DropColumn(
                name: "GoogleId",
                table: "Kullanicilar");

            migrationBuilder.AlterColumn<string>(
                name: "SifreHash",
                table: "Kullanicilar",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
