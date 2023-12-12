//Слой DAL(data access layer) создан для взаимодействия c базой данных

using app_shortlink.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace app_shortlink.DAL;

public class ApplicationDbContext : DbContext
{

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<table_urls> table_urls { get; set; }
    
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Host=localhost;Database=urlsdb;Username=postgres;Password=Sutulov12misha");

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<table_urls>().HasData(
            new table_urls {urls_id = 1, full_url = "aa", short_url = "aa", click = 1, DateCreate = DateTime.Today});
        
    }
}

