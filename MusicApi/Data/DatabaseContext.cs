using Microsoft.EntityFrameworkCore;
using MusicApi.Model;

namespace MusicApi.Data;

public class DatabaseContext : DbContext
{
    public DbSet<Artist> Artists { get; init; } = null!;
    public DbSet<Album> Albums { get; init; } = null!;
    public DbSet<Song> Songs { get; init; } = null!;

    private readonly IConfiguration _configuration;
    
    public DatabaseContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(_configuration.GetValue<string>("DbConnection"));
    }
}