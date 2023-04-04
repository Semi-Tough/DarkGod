using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlayerData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Account = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Exp = table.Column<int>(type: "int", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    Coin = table.Column<int>(type: "int", nullable: false),
                    Diamond = table.Column<int>(type: "int", nullable: false),
                    Crystal = table.Column<int>(type: "int", nullable: false),
                    Hp = table.Column<int>(type: "int", nullable: false),
                    Ad = table.Column<int>(type: "int", nullable: false),
                    AdDef = table.Column<int>(type: "int", nullable: false),
                    Ap = table.Column<int>(type: "int", nullable: false),
                    ApDef = table.Column<int>(type: "int", nullable: false),
                    Critical = table.Column<int>(type: "int", nullable: false),
                    Pierce = table.Column<int>(type: "int", nullable: false),
                    Dodge = table.Column<int>(type: "int", nullable: false),
                    GuideId = table.Column<int>(type: "int", nullable: false),
                    DungeonId = table.Column<int>(type: "int", nullable: false),
                    Power = table.Column<int>(type: "int", nullable: false),
                    LeaveTime = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StrengthenData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StrengthenType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    PlayerDataId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StrengthenData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StrengthenData_PlayerData_PlayerDataId",
                        column: x => x.PlayerDataId,
                        principalTable: "PlayerData",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TaskData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Progress = table.Column<int>(type: "int", nullable: false),
                    MaxCount = table.Column<int>(type: "int", nullable: false),
                    Finished = table.Column<bool>(type: "bit", nullable: false),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    PlayerDataId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskData_PlayerData_PlayerDataId",
                        column: x => x.PlayerDataId,
                        principalTable: "PlayerData",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_StrengthenData_PlayerDataId",
                table: "StrengthenData",
                column: "PlayerDataId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskData_PlayerDataId",
                table: "TaskData",
                column: "PlayerDataId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StrengthenData");

            migrationBuilder.DropTable(
                name: "TaskData");

            migrationBuilder.DropTable(
                name: "PlayerData");
        }
    }
}
