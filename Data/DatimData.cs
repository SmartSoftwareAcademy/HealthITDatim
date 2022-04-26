using HealthITDatim.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HealthITDatim.Data
{
    public class DatimData: IdentityDbContext
    {
        public DatimData(DbContextOptions<DatimData> options) : base(options) { }

        public DbSet<Patient> Patient { get; set; }
    }
}
