using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeriesFilmesAPI.Data;
using SeriesFilmesAPI.Models;

namespace SeriesFilmesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CadastroesController : ControllerBase
    {
        private readonly SeriesFilmesAPIContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public CadastroesController(SeriesFilmesAPIContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            this._hostEnvironment = hostEnvironment;
        }

        // GET: api/Cadastroes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cadastro>>> GetCadastro()
        {
            return await _context.Cadastro
                .Select(x => new Cadastro(){ 
                    CadastroId = x.CadastroId,
                    Name = x.Name,
                    Categoria = x.Categoria,
                    ImageName = x.ImageName,
                    ImageSrc = String.Format("{0}://{1}{2}/Images/{3}", Request.Scheme, Request.Host, Request.PathBase, x.ImageName)
                })
                .ToListAsync();
        }

        // GET: api/Cadastroes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Cadastro>> GetCadastro(int id)
        {
            var cadastro = await _context.Cadastro.FindAsync(id);

            if (cadastro == null)
            {
                return NotFound();
            }

            return cadastro;
        }

        // PUT: api/Cadastroes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCadastro(int id,[FromForm] Cadastro cadastro)
        {
            if (id != cadastro.CadastroId)
            {
                return BadRequest();
            }

            if (cadastro.ImageFile != null)
            {
                DeleteImage(cadastro.ImageName);
                cadastro.ImageName = await SaveImage(cadastro.ImageFile);
            }

            _context.Entry(cadastro).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CadastroExists(id))
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

        // POST: api/Cadastroes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Cadastro>> PostCadastro([FromForm]Cadastro cadastro)
        {

            cadastro.ImageName = await SaveImage(cadastro.ImageFile);
            _context.Cadastro.Add(cadastro);
            await _context.SaveChangesAsync();

            return StatusCode(201);

            
        }

        // DELETE: api/Cadastroes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCadastro(int id)
        {
            var cadastro = await _context.Cadastro.FindAsync(id);
            if (cadastro == null)
            {
                return NotFound();
            }

            DeleteImage(cadastro.ImageName);
            _context.Cadastro.Remove(cadastro);
            await _context.SaveChangesAsync();

            return NoContent();
        }
       
        private bool CadastroExists(int id)
        {
            return _context.Cadastro.Any(e => e.CadastroId == id);
        }

        [NonAction]
        public async Task<string> SaveImage(IFormFile imageFile)
        {
            string imageName = new String(Path.GetFileNameWithoutExtension(imageFile.FileName).Take(10).ToArray()).Replace(' ', '-');
            imageName = imageName + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(imageFile.FileName);
            var imagePath = Path.Combine(_hostEnvironment.ContentRootPath, "Images", imageName);
            using (var fileStream = new FileStream(imagePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }
            return imageName;
        }

        [NonAction]
        public void DeleteImage(string imageName)
        {
            var imagePath = Path.Combine(_hostEnvironment.ContentRootPath, "Images", imageName);
            if (System.IO.File.Exists(imagePath))
                System.IO.File.Delete(imagePath);
        }
    }
}
