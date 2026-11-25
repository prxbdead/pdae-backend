using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pdae.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddQuestionAndAnswerOption : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LessonId = table.Column<Guid>(type: "uuid", nullable: false),
                    Prompt = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Explanation = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    ExerciseType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    ExerciseData = table.Column<string>(type: "jsonb", nullable: false),
                    AnswerKey = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                    table.CheckConstraint("CK_Questions_ExerciseType", "\"ExerciseType\" IN (\n    'multiple_choice',\n    'true_false',\n    'fill_blank',\n    'match_pairs',\n    'order_steps',\n    'read_chart',\n    'spot_scam',\n    'risk_calculator',\n    'dca_timeline',\n    'story_card'\n)");
                    table.ForeignKey(
                        name: "FK_Questions_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AnswerOptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Text = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswerOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnswerOptions_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnswerOptions_QuestionId_Order",
                table: "AnswerOptions",
                columns: new[] { "QuestionId", "Order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Questions_LessonId_Order",
                table: "Questions",
                columns: new[] { "LessonId", "Order" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnswerOptions");

            migrationBuilder.DropTable(
                name: "Questions");
        }
    }
}
