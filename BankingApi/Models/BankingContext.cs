using Microsoft.EntityFrameworkCore;

namespace BankingApi.Models
{
    public class BankingContext : DbContext
    {
        public BankingContext(DbContextOptions<BankingContext> options) : base(options)
        {
        }
        
        public DbSet<Endereco> Enderecos { get; set; }
        public DbSet<Pessoa> Pessoas { get; set; }
        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<ContaBancaria> ContasBancarias { get; set; }
        public DbSet<TransacaoBancaria> TransacoesBancarias { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configuração das relações para TransacaoBancaria
            modelBuilder.Entity<TransacaoBancaria>()
                .HasOne(t => t.ContaOrigem)
                .WithMany(c => c.TransacoesOrigem)
                .HasForeignKey(t => t.ContaOrigemId)
                .OnDelete(DeleteBehavior.Restrict);
                
            modelBuilder.Entity<TransacaoBancaria>()
                .HasOne(t => t.ContaDestino)
                .WithMany(c => c.TransacoesDestino)
                .HasForeignKey(t => t.ContaDestinoId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Configuração para garantir que uma conta bancária pertence a uma pessoa OU empresa
            modelBuilder.Entity<ContaBancaria>()
                .HasIndex(c => new { c.Banco, c.Agencia, c.NumeroConta })
                .IsUnique();
                
            // Índices únicos
            modelBuilder.Entity<Pessoa>()
                .HasIndex(p => p.Cpf)
                .IsUnique();
                
            modelBuilder.Entity<Empresa>()
                .HasIndex(e => e.Cnpj)
                .IsUnique();
        }
    }
}