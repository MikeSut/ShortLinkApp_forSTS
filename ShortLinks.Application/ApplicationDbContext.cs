using ShortLinks.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace ShortLinks.Application;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }

    public DbSet<Url> Urls { get; set; }
    
    public DbSet<IpClient> IpClients { get; set; }
}

