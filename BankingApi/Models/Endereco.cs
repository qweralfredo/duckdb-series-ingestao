using System.ComponentModel.DataAnnotations;

namespace BankingApi.Models
{
    public class Endereco
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string Logradouro { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(10)]
        public string Numero { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string? Complemento { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Bairro { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string Cidade { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(2)]
        public string Estado { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(10)]
        public string Cep { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string? Pais { get; set; } = "Brasil";
        
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        public DateTime DataAtualizacao { get; set; } = DateTime.UtcNow;
    }
}