using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankingApi.Models
{
    public enum TipoConta
    {
        ContaCorrente,
        ContaPoupanca,
        ContaSalario,
        ContaInvestimento
    }
    
    public class ContaBancaria
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(10)]
        public string Banco { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(10)]
        public string Agencia { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(20)]
        public string NumeroConta { get; set; } = string.Empty;
        
        [MaxLength(5)]
        public string? DigitoVerificador { get; set; }
        
        [Required]
        public TipoConta TipoConta { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Saldo { get; set; } = 0;
        
        public bool Ativa { get; set; } = true;
        
        // Relacionamento com Pessoa (opcional)
        public int? PessoaId { get; set; }
        
        [ForeignKey("PessoaId")]
        public virtual Pessoa? Pessoa { get; set; }
        
        // Relacionamento com Empresa (opcional)
        public int? EmpresaId { get; set; }
        
        [ForeignKey("EmpresaId")]
        public virtual Empresa? Empresa { get; set; }
        
        public DateTime DataAbertura { get; set; } = DateTime.UtcNow;
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        public DateTime DataAtualizacao { get; set; } = DateTime.UtcNow;
        
        // Relacionamento com Transações
        public virtual ICollection<TransacaoBancaria> TransacoesOrigem { get; set; } = new List<TransacaoBancaria>();
        public virtual ICollection<TransacaoBancaria> TransacoesDestino { get; set; } = new List<TransacaoBancaria>();
    }
}