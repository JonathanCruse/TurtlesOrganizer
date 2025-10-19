using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TurtlesOrganizer.Domain.Entities;

namespace TurtlesOrganizer.Infrastructure.Persistence.Configurations;

public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.ToTable("Persons");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.OwnsOne(p => p.Email, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("Email")
                .IsRequired()
                .HasMaxLength(255);
        });

        builder.Property(p => p.MembershipId);

        builder.Property(p => p.IsTrainer)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Ignore(p => p.IsMember);
        builder.Ignore(p => p.IsGuest);
    }
}
