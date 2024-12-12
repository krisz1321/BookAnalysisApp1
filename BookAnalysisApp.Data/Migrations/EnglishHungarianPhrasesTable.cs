using BookAnalysisApp.Entities;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookAnalysisApp.Data
{
    public partial class AddEnglishHungarianPhrasesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EnglishHungarianPhrases",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    EnglishPhrase = table.Column<string>(maxLength: 255, nullable: false),
                    HungarianMeanings = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnglishHungarianPhrases", x => x.Id);
                });
        }



        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "EnglishHungarianPhrases");
        }
    }
}