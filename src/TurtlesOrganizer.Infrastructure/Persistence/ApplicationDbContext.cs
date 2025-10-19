using Microsoft.EntityFrameworkCore;
using TurtlesOrganizer.Domain.Aggregates.TrainingAggregate;
using TurtlesOrganizer.Domain.Entities;
using TurtlesOrganizer.Infrastructure.Persistence.Configurations;

namespace TurtlesOrganizer.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Person> Persons => Set<Person>();
    public DbSet<Training> Trainings => Set<Training>();
    public DbSet<TrainingSession> TrainingSessions => Set<TrainingSession>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new PersonConfiguration());
        modelBuilder.ApplyConfiguration(new TrainingConfiguration());
        modelBuilder.ApplyConfiguration(new TrainingSessionConfiguration());
    }
}
