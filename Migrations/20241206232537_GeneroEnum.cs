using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MasterIdiomas.Migrations
{
    /// <inheritdoc />
    public partial class GeneroEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE Professores SET Genero = 0 WHERE Genero = 'Masculino'");
            migrationBuilder.Sql("UPDATE Professores SET Genero = 1 WHERE Genero = 'Feminino'");
            migrationBuilder.Sql("UPDATE Professores SET Genero = 2 WHERE Genero = 'Outro'");

            migrationBuilder.Sql("UPDATE Alunos SET Genero = 0 WHERE Genero = 'Masculino'");
            migrationBuilder.Sql("UPDATE Alunos SET Genero = 1 WHERE Genero = 'Feminino'");
            migrationBuilder.Sql("UPDATE Alunos SET Genero = 2 WHERE Genero = 'Outro'");

            migrationBuilder.AlterColumn<int>(
                name: "Genero",
                table: "Professores",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Genero",
                table: "Alunos",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Genero",
                table: "Professores",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Genero",
                table: "Alunos",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}