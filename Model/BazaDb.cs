using RunEnova.Extension;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Text;

namespace RunEnova.Model
{
    public class BazaDb : DbContext
    {
        public BazaDb() : base("name=BazyEnova")
        {
            base.Database.CreateIfNotExists();
        }
        //public BazaDb(DbContextOptions<BazaDb> options) : base(options)
        //{
        //    if (DbSetting.GetConnectionString("BazyEnova") != null)
        //        base.Database.Migrate();
        //    //base.Database.EnsureCreated(); -odkomentować dla ukończonej aplikacji(zamiast base.Database.Migrate())

        //}
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    //if (string.IsNullOrEmpty(DbSetting.GetConnectionString("BazyEnova")))
        //    //    return;

        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        optionsBuilder.UseSqlServer(DbSetting.GetConnectionString("BazyEnova"));
        //    }
        //}

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //if (string.IsNullOrEmpty(DbSetting.GetConnectionString("BazyEnova")))
            //    return;

            //modelBuilder.Entity<Baza>()
            //{
            //    entity.Property(e => e.NazwaBazy)
            //        .HasMaxLength(50)
            //        .IsUnicode(false);
            //});
        }
        public DbSet<Baza> Baza { get; set; }
    }
}
