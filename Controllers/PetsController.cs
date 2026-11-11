using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VeterinariaSaep.Data;
using VeterinariaSaep.Models;

namespace VeterinariaSaep.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PetsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public PetsController(AppDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        public async Task<ActionResult> CriarPet (Pet pet)
        {
            var novoPet = new Pet
            {
                NomePet = pet.NomePet,
                Especie = pet.Especie,
                Raca = pet.Raca,
                TutorId = pet.TutorId
            };
            _context.pets.Add(novoPet);
            await _context.SaveChangesAsync();
            return Ok(novoPet);
        }
        [HttpGet]
        public async Task<ActionResult> GetPet()
        {
            var pets = await _context.pets.ToListAsync();
            return Ok(pets);
        }
        [HttpPut]
        public async Task<ActionResult> AtualizarPet(Pet pet)
        {
            _context.pets.Update(pet);
            await _context.SaveChangesAsync();
            return Ok(pet);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletarPet(int id)
        {
            var pet = await _context.pets.FindAsync(id);
            if (pet != null)
            {
                _context.pets.Remove(pet);
                await _context.SaveChangesAsync();
            }
            return Ok();
        }
    }
}