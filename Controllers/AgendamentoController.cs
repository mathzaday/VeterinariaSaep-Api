using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VeterinariaSaep.Data;
using VeterinariaSaep.Models;
using VeterinariaSaep.Models.DTOs;

namespace VeterinariaSaep.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgendamentoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AgendamentoController(AppDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        public async Task<ActionResult> Agendar(Agendamentos agt)
        {
            var petExiste = await _context.pets.AnyAsync(p => p.Id == agt.PetId);
            if (!petExiste)
                return BadRequest($"Pet com o ID {agt.PetId} não foi encontrado.");

            var tutorExiste = await _context.tutores.AnyAsync(p => p.Id == agt.TutorId);
            if (!tutorExiste)
                return BadRequest($"Tutor com o ID {agt.TutorId} não foi encontrado.");

            var novoAgendamento = new Agendamentos
            {
                Data = agt.Data,
                PetId = agt.PetId,
                TutorId = agt.TutorId,
                Servico = agt.Servico
            };
            _context.agendamentos.Add(novoAgendamento);
            await _context.SaveChangesAsync();
            return Ok(novoAgendamento);
        }
        [HttpGet]
        public async Task<ActionResult> GetAgendamento()
        {
            var agendamentos = await _context.agendamentos
                .Select(a => new
                {
                    a.Id,
                    a.Data,
                    a.Servico,
                    a.PetId,
                    NomePet = _context.pets
                        .Where(p => p.Id == a.PetId)
                        .Select(p => p.NomePet)
                        .FirstOrDefault(),
                    a.TutorId,
                    NomeTutor = _context.tutores
                        .Where(t => t.Id == a.TutorId)
                        .Select(t => t.NomeTutor)
                        .FirstOrDefault()
                }).ToListAsync();
            return Ok(agendamentos);
        }
        [HttpPut]
        public async Task<ActionResult> Atualizar(Agendamentos agt)
        {
            var agendamentoExistente = await _context.agendamentos.FindAsync(agt.Id);
            if (agendamentoExistente == null) return NotFound("Agendamento não encontrado.");

            var petExiste = await _context.pets.AnyAsync(p => p.Id == agt.PetId);
            if (!petExiste) return BadRequest($"Pet com ID {agt.PetId} não foi encontrado.");

            var tutorExiste = await _context.tutores.AnyAsync(t => t.Id == agt.TutorId);
            if (!tutorExiste) return BadRequest($"Tutor com ID {agt.TutorId} não foi encontrado.");

            agendamentoExistente.Data = agt.Data;
            agendamentoExistente.Servico = agt.Servico;
            agendamentoExistente.PetId = agt.PetId;
            agendamentoExistente.TutorId = agt.TutorId;

            await _context.SaveChangesAsync();
            return Ok(agendamentoExistente);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Deletar(int id)
        {
            var agt = await _context.agendamentos.FindAsync(id);
            if (agt != null)
            {
                _context.agendamentos.Remove(agt);
                await _context.SaveChangesAsync();
            }
            return Ok();
        }
    }   
}