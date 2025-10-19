using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TurtlesOrganizer.Domain.Aggregates.TrainingAggregate;

namespace TurtlesOrganizer.Infrastructure.Persistence.Configurations;

public class TrainingSessionConfiguration : IEntityTypeConfiguration<TrainingSession>
{
    public void Configure(EntityTypeBuilder<TrainingSession> builder)
    {
        builder.ToTable("TrainingSessions");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.TrainingId)
            .IsRequired();

        builder.Property(s => s.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(s => s.Date)
            .IsRequired();

        builder.Property(s => s.TrainerId)
            .IsRequired();

        builder.Property(s => s.MaxAttendees)
            .IsRequired();

        builder.Property<List<Guid>>("_attendeeIds")
            .HasColumnName("AttendeeIds")
            .HasConversion(
                v => string.Join(',', v.Select(id => id.ToString())),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(Guid.Parse)
                    .ToList()
            );

        builder.Ignore(s => s.IsFull);
        builder.Ignore(s => s.IsUpcoming);
        builder.Ignore(s => s.AvailableSpots);
    }
}
