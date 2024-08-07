using ACB_Prueba_Tecnica.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ACB_Prueba_Tecnica.DAL.Context
{
    public class ACBContext : DbContext
    {
        //Tablas
        public DbSet<MatchEvent> MatchEvents { get; set; }

        public ACBContext(DbContextOptions<ACBContext> options) : base(options)
        {
        }


    }
}
