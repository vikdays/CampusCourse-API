using api.Entities;
using Microsoft.EntityFrameworkCore;

public class DataContext : DbContext
{
    public DataContext()
    {
    }

    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {

    }
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<TokenEntity> BannedTokens { get; set; }
    public DbSet<CampusGroup> CampusGroups { get; set; } = null!;

}