using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using VeterinariaSaep.Data;
using VeterinariaSaep.Models;

namespace VeterinariaSaep.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TutorController : ControllerBase
    {
        private readonly AppDbContext _context;
        public TutorController(AppDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        public async Task<ActionResult> CriarTutor(Tutor tutor)
        {
            var novoTutor = new Tutor
            {
                NomeTutor = tutor.NomeTutor,
                Id = tutor.Id,
                CpfTutor = tutor.CpfTutor
            };
            _context.tutores.Add(novoTutor);
            await _context.SaveChangesAsync();
            return Ok(novoTutor);
        }
        [HttpPut]
        public async Task<ActionResult> AtualizarPet(Pet pet)
        {
            _context.pets.Update(pet);
            await _context.SaveChangesAsync();
            return Ok(pet);
        }
        [HttpGet]
        public async Task<ActionResult<List<Tutor>>> GetTutores()
        {
            var tutores = await _context.tutores
                .Select(t => new Tutor
                {
                    Id = t.Id,
                    NomeTutor = t.NomeTutor,
                    CpfTutor = CpfCms.Criptografar(t.CpfTutor)
                })
                .ToListAsync();
            return Ok(tutores);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> ExcluirTutor(int id)
        {
            var tutor = await _context.tutores.FindAsync(id);
            if (tutor == null)
            {
                return NotFound();
            }

            _context.tutores.Remove(tutor);
            await _context.SaveChangesAsync();
            return Ok(tutor);
        }
        // Criptografar o CPF do tutor antes de salvar no banco de dados
        public static class CpfCms
        {
            private static readonly X509Certificate2 Cert = CriarCertificado();

            private static X509Certificate2 CriarCertificado()
            {
                using (var rsa = RSA.Create(2048))
                {
                    var request = new CertificateRequest(
                        new X500DistinguishedName("CN=Key"),
                        rsa,
                        HashAlgorithmName.SHA256,
                        RSASignaturePadding.Pkcs1
                    );
                    return request.CreateSelfSigned(DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddYears(5));
                }
            }

            public static string Criptografar(string cpf)
            {
                if (string.IsNullOrEmpty(cpf)) return cpf;
                var cms = new EnvelopedCms(new ContentInfo(Encoding.UTF8.GetBytes(cpf)));
                cms.Encrypt(new CmsRecipient(Cert));
                return Convert.ToBase64String(cms.Encode());
            }

            public static string Decodificar(string cpfCriptografado)
            {
                if (string.IsNullOrEmpty(cpfCriptografado)) return cpfCriptografado;
                var cms = new EnvelopedCms();
                cms.Decode(Convert.FromBase64String(cpfCriptografado));
                cms.Decrypt(new X509Certificate2Collection(Cert));
                return Encoding.UTF8.GetString(cms.ContentInfo.Content);
            }
        }
    }
}