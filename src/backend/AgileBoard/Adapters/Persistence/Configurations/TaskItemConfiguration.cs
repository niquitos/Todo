using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AgileBoard.Domain;

namespace AgileBoard.Adapters.Persistence.Configurations;

public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .HasConversion(
                id => id.Value,
                value => new TaskItemId(value));

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Description)
            .HasMaxLength(1000);

        builder.Property(t => t.SprintId)
            .IsRequired()
            .HasConversion(
                id => id.Value,
                value => new SprintId(value));

        builder.Property(t => t.ColumnType)
            .IsRequired()
            .HasMaxLength(20)
            .HasConversion<string>();

        builder.Property(t => t.Position)
            .IsRequired();

        builder.HasIndex(t => new { t.SprintId, t.ColumnType, t.Position });

        builder.HasOne<Sprint>()
            .WithMany()
            .HasForeignKey(t => t.SprintId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
