using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VeterinariaSaep.Models.DTOs
{
    public class Agendamentos
    {
        [Key]
        public int Id { get; set; }
        public DateTime Data { get; set; }

        public string Servico { get; set; }

        public int TutorId { get; set; }
        [JsonIgnore]
        public Tutor? Tutor { get; set; }
        public int PetId { get; set; }
        [JsonIgnore]
        public Pet? Pet { get; set; }
    }
}