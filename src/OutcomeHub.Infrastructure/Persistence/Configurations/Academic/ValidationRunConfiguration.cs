using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Academic;

public sealed class ValidationRunConfiguration : IEntityTypeConfiguration<ValidationRun>
{
    public void Configure(EntityTypeBuilder<ValidationRun> builder)
    {
        builder.ToTable("validation_run", "academic", table =>
        {
            table.HasCheckConstraint("ck_validation_run_aggregate_type", "aggregate_type = upper(btrim(aggregate_type)) AND char_length(aggregate_type) > 0");
            table.HasCheckConstraint("ck_validation_run_content_hash", "content_hash ~ '^[0-9a-f]{64}$'");
        });
        builder.HasKey(x => x.Id).HasName("pk_validation_run");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.AggregateType).HasColumnName("aggregate_type").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.AggregateId).HasColumnName("aggregate_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.RulesetVersion).HasColumnName("ruleset_version").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.ContentHash).HasColumnName("content_hash").HasColumnType("char(64)").HasMaxLength(64).IsFixedLength().IsRequired();
        builder.Property(x => x.Passed).HasColumnName("passed").HasColumnType("boolean").IsRequired();
        builder.Property(x => x.RunAt).HasColumnName("run_at").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.RequestedBy).HasColumnName("requested_by").HasColumnType("uuid").IsRequired();
        builder.HasIndex(x => new { x.AggregateType, x.AggregateId, x.RunAt }).HasDatabaseName("ix_validation_run_aggregate_time");
        builder.HasOne(x => x.RequestedByPrincipal).WithMany().HasForeignKey(x => x.RequestedBy).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_validation_run_requested_by");
    }
}
