using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Ai;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Ai;

public sealed class GroundTruthSuiteConfiguration : IEntityTypeConfiguration<GroundTruthSuite>
{
    public void Configure(EntityTypeBuilder<GroundTruthSuite> builder)
    {
        builder.ToTable("ground_truth_suite", "ai");

        builder.HasKey(entity => entity.Id)
            .HasName("pk_ground_truth_suite");

        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .IsRequired(true);

        builder.Property(entity => entity.Code)
            .HasColumnName("code")
            .HasColumnType("varchar(64)")
            .HasMaxLength(64)
            .IsRequired(true);

        builder.Property(entity => entity.Name)
            .HasColumnName("name")
            .HasColumnType("varchar(255)")
            .HasMaxLength(255)
            .IsRequired(true);

        builder.HasIndex(entity => entity.Code)
            .IsUnique()
            .HasDatabaseName("uq_ground_truth_suite_code");

        builder.ToTable(tableBuilder => tableBuilder.HasCheckConstraint(
            "ck_ground_truth_suite_code",
            "code = upper(btrim(code)) AND char_length(code) > 0"));
    }
}
