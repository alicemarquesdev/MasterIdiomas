using MasterIdiomas.Enums;
using System.ComponentModel.DataAnnotations;

namespace MasterIdiomas.Models
{
    public class AlunoModel
    {
        [Key]
        public int AlunoId { get; set; }

        [Required(ErrorMessage = "Digite o seu nome.")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Por favor, selecione o gênero.")]
        public GeneroEnum Genero { get; set; } = GeneroEnum.Outro;

        [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
        public DateTime DataNascimento { get; set; } = default;

        public DateTime DataCadastro { get; set; } = DateTime.Now;

        public StatusEnum Status { get; set; } = StatusEnum.Ativo;

        public virtual IList<AlunoCursoModel> AlunoCurso { get; set; } = new List<AlunoCursoModel>();
    }
}