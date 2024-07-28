using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HSBGHelper.Server.Migrations
{
    /// <inheritdoc />
    public partial class initialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HeroPowers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HtmlGuide = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroPowers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Buddies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tier = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HtmlGuide = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompositionId = table.Column<int>(type: "int", nullable: true),
                    CompositionId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Buddies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Heroes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Armor = table.Column<int>(type: "int", nullable: false),
                    buddyId = table.Column<int>(type: "int", nullable: false),
                    heroPowerId = table.Column<int>(type: "int", nullable: false),
                    TierHSReplay = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TierJeef = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HtmlGuide = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HeroId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Heroes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Heroes_Buddies_buddyId",
                        column: x => x.buddyId,
                        principalTable: "Buddies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Heroes_HeroPowers_heroPowerId",
                        column: x => x.heroPowerId,
                        principalTable: "HeroPowers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Heroes_Heroes_HeroId",
                        column: x => x.HeroId,
                        principalTable: "Heroes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Spells",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tier = table.Column<int>(type: "int", nullable: false),
                    HtmlGuide = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BuddyId = table.Column<int>(type: "int", nullable: true),
                    HeroPowerId = table.Column<int>(type: "int", nullable: true),
                    SpellId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spells", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Spells_Buddies_BuddyId",
                        column: x => x.BuddyId,
                        principalTable: "Buddies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Spells_HeroPowers_HeroPowerId",
                        column: x => x.HeroPowerId,
                        principalTable: "HeroPowers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Spells_Spells_SpellId",
                        column: x => x.SpellId,
                        principalTable: "Spells",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HeroSpell",
                columns: table => new
                {
                    heroSynergiesId = table.Column<int>(type: "int", nullable: false),
                    spellSynergiesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroSpell", x => new { x.heroSynergiesId, x.spellSynergiesId });
                    table.ForeignKey(
                        name: "FK_HeroSpell_Heroes_heroSynergiesId",
                        column: x => x.heroSynergiesId,
                        principalTable: "Heroes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HeroSpell_Spells_spellSynergiesId",
                        column: x => x.spellSynergiesId,
                        principalTable: "Spells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Compositions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tier = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Difficulty = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Link = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    mainMinionId = table.Column<int>(type: "int", nullable: false),
                    HtmlGuide = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Compositions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Minions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tier = table.Column<int>(type: "int", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HtmlGuide = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BuddyId = table.Column<int>(type: "int", nullable: true),
                    CompositionId = table.Column<int>(type: "int", nullable: true),
                    CompositionId1 = table.Column<int>(type: "int", nullable: true),
                    HeroPowerId = table.Column<int>(type: "int", nullable: true),
                    MinionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Minions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Minions_Buddies_BuddyId",
                        column: x => x.BuddyId,
                        principalTable: "Buddies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Minions_Compositions_CompositionId",
                        column: x => x.CompositionId,
                        principalTable: "Compositions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Minions_Compositions_CompositionId1",
                        column: x => x.CompositionId1,
                        principalTable: "Compositions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Minions_HeroPowers_HeroPowerId",
                        column: x => x.HeroPowerId,
                        principalTable: "HeroPowers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Minions_Minions_MinionId",
                        column: x => x.MinionId,
                        principalTable: "Minions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HeroMinion",
                columns: table => new
                {
                    heroSynergiesId = table.Column<int>(type: "int", nullable: false),
                    minionSynergiesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroMinion", x => new { x.heroSynergiesId, x.minionSynergiesId });
                    table.ForeignKey(
                        name: "FK_HeroMinion_Heroes_heroSynergiesId",
                        column: x => x.heroSynergiesId,
                        principalTable: "Heroes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HeroMinion_Minions_minionSynergiesId",
                        column: x => x.minionSynergiesId,
                        principalTable: "Minions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MinionSpell",
                columns: table => new
                {
                    minionSynergiesId = table.Column<int>(type: "int", nullable: false),
                    spellSynergiesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MinionSpell", x => new { x.minionSynergiesId, x.spellSynergiesId });
                    table.ForeignKey(
                        name: "FK_MinionSpell_Minions_minionSynergiesId",
                        column: x => x.minionSynergiesId,
                        principalTable: "Minions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MinionSpell_Spells_spellSynergiesId",
                        column: x => x.spellSynergiesId,
                        principalTable: "Spells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Buddies_CompositionId",
                table: "Buddies",
                column: "CompositionId");

            migrationBuilder.CreateIndex(
                name: "IX_Buddies_CompositionId1",
                table: "Buddies",
                column: "CompositionId1");

            migrationBuilder.CreateIndex(
                name: "IX_Compositions_mainMinionId",
                table: "Compositions",
                column: "mainMinionId");

            migrationBuilder.CreateIndex(
                name: "IX_Heroes_buddyId",
                table: "Heroes",
                column: "buddyId");

            migrationBuilder.CreateIndex(
                name: "IX_Heroes_HeroId",
                table: "Heroes",
                column: "HeroId");

            migrationBuilder.CreateIndex(
                name: "IX_Heroes_heroPowerId",
                table: "Heroes",
                column: "heroPowerId");

            migrationBuilder.CreateIndex(
                name: "IX_HeroMinion_minionSynergiesId",
                table: "HeroMinion",
                column: "minionSynergiesId");

            migrationBuilder.CreateIndex(
                name: "IX_HeroSpell_spellSynergiesId",
                table: "HeroSpell",
                column: "spellSynergiesId");

            migrationBuilder.CreateIndex(
                name: "IX_Minions_BuddyId",
                table: "Minions",
                column: "BuddyId");

            migrationBuilder.CreateIndex(
                name: "IX_Minions_CompositionId",
                table: "Minions",
                column: "CompositionId");

            migrationBuilder.CreateIndex(
                name: "IX_Minions_CompositionId1",
                table: "Minions",
                column: "CompositionId1");

            migrationBuilder.CreateIndex(
                name: "IX_Minions_HeroPowerId",
                table: "Minions",
                column: "HeroPowerId");

            migrationBuilder.CreateIndex(
                name: "IX_Minions_MinionId",
                table: "Minions",
                column: "MinionId");

            migrationBuilder.CreateIndex(
                name: "IX_MinionSpell_spellSynergiesId",
                table: "MinionSpell",
                column: "spellSynergiesId");

            migrationBuilder.CreateIndex(
                name: "IX_Spells_BuddyId",
                table: "Spells",
                column: "BuddyId");

            migrationBuilder.CreateIndex(
                name: "IX_Spells_HeroPowerId",
                table: "Spells",
                column: "HeroPowerId");

            migrationBuilder.CreateIndex(
                name: "IX_Spells_SpellId",
                table: "Spells",
                column: "SpellId");

            migrationBuilder.AddForeignKey(
                name: "FK_Buddies_Compositions_CompositionId",
                table: "Buddies",
                column: "CompositionId",
                principalTable: "Compositions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Buddies_Compositions_CompositionId1",
                table: "Buddies",
                column: "CompositionId1",
                principalTable: "Compositions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Compositions_Minions_mainMinionId",
                table: "Compositions",
                column: "mainMinionId",
                principalTable: "Minions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Buddies_Compositions_CompositionId",
                table: "Buddies");

            migrationBuilder.DropForeignKey(
                name: "FK_Buddies_Compositions_CompositionId1",
                table: "Buddies");

            migrationBuilder.DropForeignKey(
                name: "FK_Minions_Compositions_CompositionId",
                table: "Minions");

            migrationBuilder.DropForeignKey(
                name: "FK_Minions_Compositions_CompositionId1",
                table: "Minions");

            migrationBuilder.DropTable(
                name: "HeroMinion");

            migrationBuilder.DropTable(
                name: "HeroSpell");

            migrationBuilder.DropTable(
                name: "MinionSpell");

            migrationBuilder.DropTable(
                name: "Heroes");

            migrationBuilder.DropTable(
                name: "Spells");

            migrationBuilder.DropTable(
                name: "Compositions");

            migrationBuilder.DropTable(
                name: "Minions");

            migrationBuilder.DropTable(
                name: "Buddies");

            migrationBuilder.DropTable(
                name: "HeroPowers");
        }
    }
}
