using System.Configuration;
using System.Data.Entity;

namespace RunEnova.Model
{
    public class BazaDb : DbContext
    {
        public BazaDb() : base("name=DefaultConnection")
        {
            base.Database.CreateIfNotExists();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Baza>().ToTable("Baza");
        }
        public DbSet<Baza> Baza { get; set; }
    }
}
