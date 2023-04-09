using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataTransferAPILab.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Audits",
                columns: table => new
                {
                    AuditId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TransferDataId = table.Column<int>(type: "int", nullable: false),
                    TransferName = table.Column<string>(type: "varchar(50)", nullable: false),
                    Timestamp = table.Column<string>(type: "nvarchar(19)", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    Bytes = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Audits", x => x.AuditId);
                });

            migrationBuilder.CreateTable(
                name: "Transfers",
                columns: table => new
                {
                    TransferDataId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TransferName = table.Column<string>(type: "varchar(50)", nullable: false),
                    TransferData = table.Column<string>(type: "varchar(500000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transfers", x => x.TransferDataId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Audits");

            migrationBuilder.DropTable(
                name: "Transfers");
        }
    }
}
