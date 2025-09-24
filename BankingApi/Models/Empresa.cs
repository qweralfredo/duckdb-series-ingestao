using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankingApi.Models
{
    public class Empresa
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string RazaoSocial { get; set; } = string.Empty;
        
        [MaxLength(200)]
        public string? NomeFantasia { get; set; }
        
        [Required]
        [MaxLength(18)]
        public string Cnpj { get; set; } = string.Empty;
        
        [MaxLength(20)]
        public string? InscricaoEstadual { get; set; }
        
        [MaxLength(20)]
        public string? InscricaoMunicipal { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;
        
        [MaxLength(20)]
        public string? Telefone { get; set; }
        
        [MaxLength(200)]
        public string? Website { get; set; }
        
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