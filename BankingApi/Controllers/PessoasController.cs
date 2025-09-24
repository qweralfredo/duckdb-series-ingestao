using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BankingApi.Models;

namespace BankingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PessoasController : ControllerBase
    {
        private readonly BankingContext _context;

        public PessoasController(BankingContext context)
        {
            _context = context;
        }

        // GET: api/Pessoas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pessoa>>> GetPessoas()
        {
            return await _context.Pessoas
                .Include(p => p.Endereco)
                .Include(p => p.ContasBancarias)
                .ToListAsync();
        }

        // GET: api/Pessoas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Pessoa>> GetPessoa(int id)
        {
            var pessoa = await _context.Pessoas
                .Include(p => p.Endereco)
                .Include(p => p.ContasBancarias)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pessoa == null)
            {
                return NotFound();
            }

            return pessoa;
        }

        // PUT: api/Pessoas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPessoa(int id, Pessoa pessoa)
        {
            if (id != pessoa.Id)
            {
                return BadRequest();
            }

            pessoa.DataAtualizacao = DateTime.UtcNow;
            _context.Entry(pessoa).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PessoaExists(id))
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

        // POST: api/Pessoas
        [HttpPost]
        public async Task<ActionResult<Pessoa>> PostPessoa(Pessoa pessoa)
        {
            pessoa.DataCriacao = DateTime.UtcNow;
            pessoa.DataAtualizacao = DateTime.UtcNow;
            
            _context.Pessoas.Add(pessoa);
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PessoaCpfExists(pessoa.Cpf))
                {
                    return Conflict("CPF já cadastrado no sistema.");
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetPessoa", new { id = pessoa.Id }, pessoa);
        }

        // DELETE: api/Pessoas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePessoa(int id)
        {
            var pessoa = await _context.Pessoas.FindAsync(id);
            if (pessoa == null)
            {
                return NotFound();
            }

            _context.Pessoas.Remove(pessoa);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PessoaExists(int id)
        {
            return _context.Pessoas.Any(e => e.Id == id);
        }
        
        private bool PessoaCpfExists(string cpf)
        {
            return _context.Pessoas.Any(e => e.Cpf == cpf);
        }
    }
}