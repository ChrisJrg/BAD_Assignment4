using Microsoft.EntityFrameworkCore;
using AarhusSpaceProgramAPI.Models;
namespace AarhusSpaceProgramAPI.Data;

public class ApplicationDbContext :  DbContext
{
    public ApplicationDbContext(){}
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        if (options.IsConfigured) return;

        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<ApplicationDbContext>()
            .Build();

        string? connectionString = configuration["ConnectionStrings:DefaultConnection"];
        options.UseSqlServer(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Mission>()
            .HasOne(p => p.LaunchPad)
            .WithMany(m => m.Missions)
            .HasForeignKey(f => f.LaunchPadId);
        
        modelBuilder.Entity<Mission>()
            .HasOne(p => p.Manager)
            .WithMany(m => m.Missions)
            .HasForeignKey(f => f.ManagerId);
        
        modelBuilder.Entity<Mission>()
            .HasOne(p => p.TargetBody)
            .WithMany(m => m.Missions)
            .HasForeignKey(f => f.TargetBodyId);

        modelBuilder.Entity<Astronaut>()
            .HasMany(a => a.Missions)
            .WithMany(m => m.Astronauts);
        
        modelBuilder.Entity<Scientist>()
            .HasMany(s =>  s.Missions)
            .WithMany(m => m.Scientists);
        
        modelBuilder.Entity<CelestialBody>()
            .HasOne(c => c.ParentPlanet)
            .WithMany(c => c.Moons)
            .HasForeignKey(c => c.ParentPlanetId)
            .OnDelete(DeleteBehavior.Restrict);
    }
    
    public DbSet<Astronaut> Astronauts { get; set; }
    public DbSet<CelestialBody> CelestialBodies { get; set; }
    public DbSet<LaunchPad> LaunchPads { get; set; }
    public DbSet<Manager> Managers { get; set; }
    public DbSet<Mission> Missions { get; set; }
    public DbSet<Rocket> Rockets { get; set; }
    public DbSet<Scientist> Scientists { get; set; }
}