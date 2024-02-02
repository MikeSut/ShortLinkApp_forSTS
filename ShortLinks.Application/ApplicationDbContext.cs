using ShortLinks.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace ShortLinks.Application;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; init; } = null!;
    public DbSet<Url> Urls { get; init; } = null!;
    public DbSet<IpClient> IpClients { get; init; } = null!;

    public DbSet<TgChatId> TgChatIdUsers { get; init; } = null!;

}

