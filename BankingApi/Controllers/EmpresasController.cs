using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BankingApi.Models;

namespace BankingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpresasController : ControllerBase
    {
        private readonly BankingContext _context;

        public EmpresasController(BankingContext context)
        {
            _context = context;
        }

        // GET: api/Empresas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Empresa>>> GetEmpresas()
        {
            return await _context.Empresas
                .Include(e => e.Endereco)
                .Include(e => e.ContasBancarias)
                .ToListAsync();
        }

        // GET: api/Empresas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Empresa>> GetEmpresa(int id)
        {
            var empresa = await _context.Empresas
                .Include(e => e.Endereco)
                .Include(e => e.ContasBancarias)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (empresa == null)
            {
                return NotFound();
            }

            return empresa;
        }

        // PUT: api/Empresas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmpresa(int id, Empresa empresa)
        {
            if (id != empresa.Id)
            {
                return BadRequest();
            }

            empresa.DataAtualizacao = DateTime.UtcNow;
            _context.Entry(empresa).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmpresaExists(id))
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

        // POST: api/Empresas
        [HttpPost]
        public async Task<ActionResult<Empresa>> PostEmpresa(Empresa empresa)
        {
            empresa.DataCriacao = DateTime.UtcNow;
            empresa.DataAtualizacao = DateTime.UtcNow;
            
            _context.Empresas.Add(empresa);
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (EmpresaCnpjExists(empresa.Cnpj))
                {
                    return Conflict("CNPJ já cadastrado no sistema.");
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetEmpresa", new { id = empresa.Id }, empresa);
        }

        // DELETE: api/Empresas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmpresa(int id)
        {
            var empresa = await _context.Empresas.FindAsync(id);
            if (empresa == null)
            {
                return NotFound();
            }

            _context.Empresas.Remove(empresa);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmpresaExists(int id)
        {
            return _context.Empresas.Any(e => e.Id == id);
        }
        
        private bool EmpresaCnpjExists(string cnpj)
        {
            return _context.Empresas.Any(e => e.Cnpj == cnpj);
        }
    }
}