//Слой DAL(data access layer) создан для взаимодействия c базой данных

using app_shortlink.Domain.Entity;
using app_shortlink.Domain.Enum;
using app_shortlink.Domain.Helpers;
using Microsoft.EntityFrameworkCore;

namespace app_shortlink.DAL;

public class ApplicationDbContext : DbContext
{
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;

    public object TableUrls { get; set; }
    // public DbSet<TableUrl> TableUrls { get; set; } = null!;

    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=urlsdb;Username=mike;Password=mike");

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        modelBuilder.Entity<User>(builder =>
        {
            builder.HasData(new User
            {
                Id = 1,
                Name = "AdminM",
                Password = HashPasswordHelper.HashPassword("123456"),
                Role = Role.Admin,
            });
            
            
            
            builder.ToTable("Users").HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.Name).HasMaxLength(100).HasColumnName("Username").IsRequired();
            builder.Property(x => x.Password).HasMaxLength(100).IsRequired();
        });
        
        // modelBuilder.Entity<TableUrl>(builder =>
        // {
        //     builder.Property(x => x.Id).ValueGeneratedNever();
        //     builder.Property(x => x.FullUrl).HasMaxLength(100);
        //     builder.Property(x => x.Clicks);
        //    
        // });
    }

    

}

