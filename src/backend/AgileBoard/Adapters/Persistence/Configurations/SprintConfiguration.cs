using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AgileBoard.Domain;

namespace AgileBoard.Adapters.Persistence.Configurations;

public class SprintConfiguration : IEntityTypeConfiguration<Sprint>
{
    public void Configure(EntityTypeBuilder<Sprint> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .HasConversion(
                id => id.Value,
                value => new SprintId(value));

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.StartDate)
            .IsRequired();

        builder.Property(s => s.EndDate)
            .IsRequired();

        builder.Property(s => s.Description)
            .HasMaxLength(1000);

        builder.Property(s => s.IsDefault)
            .IsRequired();
    }
}
