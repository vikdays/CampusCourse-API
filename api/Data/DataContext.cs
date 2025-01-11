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
    public DbSet<TokenEntity> BannedTokens { get; set; } = null!;
    public DbSet<CampusGroup> CampusGroups { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<CampusCourse> Courses { get; set; } = null!;
    public DbSet<CampusCourseStudent> Students { get; set; } = null!;
    public DbSet<CampusCourseTeacher> Teachers { get; set; } = null!;
    public DbSet<Notification> Notifications { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CampusGroup>()
            .HasMany(g => g.Courses)
            .WithOne(c => c.CampusGroup)
            .HasForeignKey(c => c.CampusGroupId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CampusCourseStudent>()
            .HasKey(cs => cs.UserId); 

        modelBuilder.Entity<CampusCourseStudent>()
            .HasOne(cs => cs.CampusCourse)
            .WithMany(c => c.Students)
            .HasForeignKey(cs => cs.CampusCourseId);

        modelBuilder.Entity<CampusCourseStudent>()
            .HasOne(cs => cs.User)
            .WithMany()
            .HasForeignKey(cs => cs.UserId);

        modelBuilder.Entity<CampusCourseTeacher>()
            .HasKey(ct => new { ct.UserId, ct.CampusCourseId });

        modelBuilder.Entity<CampusCourseTeacher>()
            .HasOne(ct => ct.CampusCourse)
            .WithMany(c => c.Teachers)
            .HasForeignKey(ct => ct.CampusCourseId);

        modelBuilder.Entity<CampusCourseTeacher>()
            .HasOne(ct => ct.User)
            .WithMany()
            .HasForeignKey(ct => ct.UserId);

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.CampusCourse)
            .WithMany(c => c.Notifications)
            .HasForeignKey(n => n.CampusCourseId)
            .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);
    }
}