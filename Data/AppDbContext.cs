using Microsoft.EntityFrameworkCore;
using VeterinariaSaep.Models;
using VeterinariaSaep.Models.DTOs;

namespace VeterinariaSaep.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Login> logins { get; set; }
        public DbSet<Tutor> tutores { get; set; }
        public DbSet<Pet> pets { get; set; }
        public DbSet<Agendamentos> agendamentos { get; set; }

    }
}
