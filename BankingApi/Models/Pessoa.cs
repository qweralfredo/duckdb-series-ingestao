using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankingApi.Models
{
    public class Pessoa
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string Nome { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(14)]
        public string Cpf { get; set; } = string.Empty;
        
        [MaxLength(20)]
        public string? Rg { get; set; }
        
        public DateTime DataNascimento { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;
        
        [MaxLength(20)]
        public string? Telefone { get; set; }
        
        [MaxLength(20)]
        public string? Celular { get; set; }
        
        // Relacionamento com Endereco
        public int? EnderecoId { get; set; }
        
        [ForeignKey("EnderecoId")]
        public virtual Endereco? Endereco { get; set; }
        
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        public DateTime DataAtualizacao { get; set; } = DateTime.UtcNow;
        
        // Relacionamento com Contas Bancárias
        public virtual ICollection<ContaBancaria> ContasBancarias { get; set; } = new List<ContaBancaria>();
    }
}