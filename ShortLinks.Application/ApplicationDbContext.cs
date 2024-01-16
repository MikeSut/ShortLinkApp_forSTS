using ShortLinks.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace ShortLinks.Application;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Url> Urls { get; set; } = null!;
    public DbSet<IpClient> IpClients { get; set; } = null!;
    public DbSet<PhoneNumber> PhoneNumbers { get; set; }

}

