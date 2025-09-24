using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankingApi.Models
{
    public enum TipoTransacao
    {
        Transferencia,
        Deposito,
        Saque,
        Pagamento,
        TED,
        PIX,
        DOC
    }
    
    public enum StatusTransacao
    {
        Pendente,
        Processando,
        Concluida,
        Cancelada,
        Rejeitada
    }
    
    public class TransacaoBancaria
    {
        [Key]
        public int Id { get; set; }
        
        // ISO20022 Message Identification
        [Required]
        [MaxLength(35)]
        public string MessageId { get; set; } = string.Empty;
        
        // ISO20022 End-to-End Identification
        [Required]
        [MaxLength(35)]
        public string EndToEndId { get; set; } = string.Empty;
        
        // ISO20022 Transaction Identification
        [Required]
        [MaxLength(35)]
        public string TransactionId { get; set; } = string.Empty;
        
        [Required]
        public TipoTransacao TipoTransacao { get; set; }
        
        [Required]
        public StatusTransacao Status { get; set; } = StatusTransacao.Pendente;
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Valor { get; set; }
        
        [Required]
        [MaxLength(3)]
        public string Moeda { get; set; } = "BRL";
        
        // Conta de Origem
        public int? ContaOrigemId { get; set; }
        
        [ForeignKey("ContaOrigemId")]
        public virtual ContaBancaria? ContaOrigem { get; set; }
        
        // Conta de Destino
        public int? ContaDestinoId { get; set; }
        
        [ForeignKey("ContaDestinoId")]
        public virtual ContaBancaria? ContaDestino { get; set; }
        
        // ISO20022 Remittance Information
        [MaxLength(140)]
        public string? InformacaoRemessa { get; set; }
        
        // ISO20022 Purpose Code
        [MaxLength(4)]
        public string? CodigoFinalidade { get; set; }
        
        // ISO20022 Category Purpose Code
        [MaxLength(4)]
        public string? CodigoCategoriaFinalidade { get; set; }
        
        [MaxLength(500)]
        public string? Descricao { get; set; }
        
        // ISO20022 Debtor Information
        [MaxLength(140)]
        public string? NomeDevedor { get; set; }
        
        [MaxLength(34)]
        public string? ContaDevedor { get; set; }
        
        // ISO20022 Creditor Information
        [MaxLength(140)]
        public string? NomeCredor { get; set; }
        
        [MaxLength(34)]
        public string? ContaCredor { get; set; }
        
        // ISO20022 Agent Information
        [MaxLength(11)]
        public string? BicAgenteDevedor { get; set; }
        
        [MaxLength(11)]
        public string? BicAgenteCredor { get; set; }
        
        public DateTime DataSolicitacao { get; set; } = DateTime.UtcNow;
        public DateTime? DataProcessamento { get; set; }
        public DateTime? DataLiquidacao { get; set; }
        
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        public DateTime DataAtualizacao { get; set; } = DateTime.UtcNow;
        
        [MaxLength(500)]
        public string? MotivoRejeicao { get; set; }
    }
}