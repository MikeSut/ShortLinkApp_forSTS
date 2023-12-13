//Слой DAL(data access layer) создан для взаимодействия c базой данных

using app_shortlink.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace app_shortlink.DAL;

public class ApplicationDbContext : DbContext
{
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        // Database.EnsureCreated();
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<table_url> Table_urls { get; set; } = null!;

    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=urlsdb;Username=mike;Password=mike");

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().Property(x => x.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<User>().Property(x => x.Username).HasMaxLength(100);
        modelBuilder.Entity<table_url>().Property(x => x.urls_id).ValueGeneratedOnAdd();
    }

    

}

