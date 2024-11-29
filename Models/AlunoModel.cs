using MasterIdiomas.Enums;
using System.ComponentModel.DataAnnotations;

namespace MasterIdiomas.Models
{
    public class AlunoModel
    {
        [Key]
        public int AlunoId { get; set; }

        [Required(ErrorMessage = "Digite o seu nome.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Por favor, selecione o gênero.")]
        public string Genero { get; set; } = string.Empty;

        [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
        public DateTime DataNascimento { get; set; } = default;

        public DateTime DataCadastro { get; set; } = DateTime.Now;

        public StatusEnum Status { get; set; } = StatusEnum.Ativo;

        public virtual IList<AlunoCursoModel> AlunoCurso { get; set; } = new List<AlunoCursoModel>();
    }
}