using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BankingApi.Models;

namespace BankingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnderecosController : ControllerBase
    {
        private readonly BankingContext _context;

        public EnderecosController(BankingContext context)
        {
            _context = context;
        }

        // GET: api/Enderecos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Endereco>>> GetEnderecos()
        {
            return await _context.Enderecos.ToListAsync();
        }

        // GET: api/Enderecos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Endereco>> GetEndereco(int id)
        {
            var endereco = await _context.Enderecos.FindAsync(id);

            if (endereco == null)
            {
                return NotFound();
            }

            return endereco;
        }

        // PUT: api/Enderecos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEndereco(int id, Endereco endereco)
        {
            if (id != endereco.Id)
            {
                return BadRequest();
            }

            endereco.DataAtualizacao = DateTime.UtcNow;
            _context.Entry(endereco).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EnderecoExists(id))
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

        // POST: api/Enderecos
        [HttpPost]
        public async Task<ActionResult<Endereco>> PostEndereco(Endereco endereco)
        {
            endereco.DataCriacao = DateTime.UtcNow;
            endereco.DataAtualizacao = DateTime.UtcNow;
            
            _context.Enderecos.Add(endereco);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEndereco", new { id = endereco.Id }, endereco);
        }

        // DELETE: api/Enderecos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEndereco(int id)
        {
            var endereco = await _context.Enderecos.FindAsync(id);
            if (endereco == null)
            {
                return NotFound();
            }

            _context.Enderecos.Remove(endereco);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EnderecoExists(int id)
        {
            return _context.Enderecos.Any(e => e.Id == id);
        }
    }
}