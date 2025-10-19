using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TurtlesOrganizer.Domain.Aggregates.TrainingAggregate;

namespace TurtlesOrganizer.Infrastructure.Persistence.Configurations;

public class TrainingConfiguration : IEntityTypeConfiguration<Training>
{
    public void Configure(EntityTypeBuilder<Training> builder)
    {
        builder.ToTable("Trainings");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Topic)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(t => t.Description)
            .HasMaxLength(2000);

        builder.Property(t => t.CreatedByUserId)
            .IsRequired();

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Navigation(t => t.Sessions)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasField("_sessions");
    }
}
