//Слой DAL(data access layer) создан для взаимодействия c базой данных

using app_shortlink.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace app_shortlink.DAL
{
    public sealed class ApplicationDbContext : DbContext //ApplicationDbContext содержит основные компоненты для взаимодействия с БД
    {
        public ApplicationDbContext()
        {
            Database.EnsureCreated(); //метод для создания БД
        }
        
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) //Метод, отвечающий за настройку кофигурации ApplicationDbContext
        {
            // optionsBuilder.LogTo(Console.WriteLine);
            optionsBuilder.UseSqlServer("Server=localhost;Database=shortlinkdb;Trusted_Connection=True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(builder =>
            {
                builder.Property(x => x.Id).ValueGeneratedOnAdd();
                builder.Property(x => x.Username).HasMaxLength(100);
                builder.Property(x => x.Password).HasMaxLength(15);
            });

        }
    }
}

