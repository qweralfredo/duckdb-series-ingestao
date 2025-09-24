using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BankingApi.Models;

namespace BankingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContasBancariasController : ControllerBase
    {
        private readonly BankingContext _context;

        public ContasBancariasController(BankingContext context)
        {
            _context = context;
        }

        // GET: api/ContasBancarias
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContaBancaria>>> GetContasBancarias()
        {
            return await _context.ContasBancarias
                .Include(c => c.Pessoa)
                .Include(c => c.Empresa)
                .ToListAsync();
        }

        // GET: api/ContasBancarias/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ContaBancaria>> GetContaBancaria(int id)
        {
            var contaBancaria = await _context.ContasBancarias
                .Include(c => c.Pessoa)
                .Include(c => c.Empresa)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (contaBancaria == null)
            {
                return NotFound();
            }

            return contaBancaria;
        }

        // PUT: api/ContasBancarias/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContaBancaria(int id, ContaBancaria contaBancaria)
        {
            if (id != contaBancaria.Id)
            {
                return BadRequest();
            }

            // Validação: uma conta deve pertencer a uma pessoa OU empresa, não ambos
            if (contaBancaria.PessoaId.HasValue && contaBancaria.EmpresaId.HasValue)
            {
                return BadRequest("Uma conta bancária deve pertencer a uma pessoa OU empresa, não ambos.");
            }

            if (!contaBancaria.PessoaId.HasValue && !contaBancaria.EmpresaId.HasValue)
            {
                return BadRequest("Uma conta bancária deve pertencer a uma pessoa ou empresa.");
            }

            contaBancaria.DataAtualizacao = DateTime.UtcNow;
            _context.Entry(contaBancaria).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContaBancariaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ContasBancarias
        [HttpPost]
        public async Task<ActionResult<ContaBancaria>> PostContaBancaria(ContaBancaria contaBancaria)
        {
            // Validação: uma conta deve pertencer a uma pessoa OU empresa, não ambos
            if (contaBancaria.PessoaId.HasValue && contaBancaria.EmpresaId.HasValue)
            {
                return BadRequest("Uma conta bancária deve pertencer a uma pessoa OU empresa, não ambos.");
            }

            if (!contaBancaria.PessoaId.HasValue && !contaBancaria.EmpresaId.HasValue)
            {
                return BadRequest("Uma conta bancária deve pertencer a uma pessoa ou empresa.");
            }

            contaBancaria.DataCriacao = DateTime.UtcNow;
            contaBancaria.DataAtualizacao = DateTime.UtcNow;
            
            _context.ContasBancarias.Add(contaBancaria);
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ContaExiste(contaBancaria.Banco, contaBancaria.Agencia, contaBancaria.NumeroConta))
                {
                    return Conflict("Conta bancária já cadastrada no sistema.");
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetContaBancaria", new { id = contaBancaria.Id }, contaBancaria);
        }

        // DELETE: api/ContasBancarias/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContaBancaria(int id)
        {
            var contaBancaria = await _context.ContasBancarias.FindAsync(id);
            if (contaBancaria == null)
            {
                return NotFound();
            }

            _context.ContasBancarias.Remove(contaBancaria);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ContaBancariaExists(int id)
        {
            return _context.ContasBancarias.Any(e => e.Id == id);
        }
        
        private bool ContaExiste(string banco, string agencia, string numeroConta)
        {
            return _context.ContasBancarias.Any(c => 
                c.Banco == banco && 
                c.Agencia == agencia && 
                c.NumeroConta == numeroConta);
        }
    }
}