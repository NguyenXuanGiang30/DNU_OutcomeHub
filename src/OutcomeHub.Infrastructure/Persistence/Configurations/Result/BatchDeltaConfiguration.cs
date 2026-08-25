using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Result;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Result;

public sealed class BatchDeltaConfiguration : IEntityTypeConfiguration<BatchDelta>
{
    public void Configure(EntityTypeBuilder<BatchDelta> builder)
    {
        builder.ToTable("batch_delta", "result");

        builder.HasKey(entity => entity.Id)
            .HasName("pk_batch_delta");

        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .IsRequired(true);

        builder.Property(entity => entity.OldBatchId)
            .HasColumnName("old_batch_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.NewBatchId)
            .HasColumnName("new_batch_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.EntityType)
            .HasColumnName("entity_type")
            .HasColumnType("varchar(64)")
            .HasMaxLength(64)
            .IsRequired(true);

        builder.Property(entity => entity.EntityKey)
            .HasColumnName("entity_key")
            .HasColumnType("jsonb")
            .IsRequired(true);

        builder.Property(entity => entity.OldValue)
            .HasColumnName("old_value")
            .HasColumnType("numeric(20,10)")
            .IsRequired(false);

        builder.Property(entity => entity.NewValue)
            .HasColumnName("new_value")
            .HasColumnType("numeric(20,10)")
            .IsRequired(false);

        builder.Property(entity => entity.Delta)
            .HasColumnName("delta")
            .HasColumnType("numeric(20,10)")
            .IsRequired(false);

        builder.Property(entity => entity.Reason)
            .HasColumnName("reason")
            .HasColumnType("text")
            .IsRequired(false);

        builder.ToTable(table => table.HasCheckConstraint("ck_batch_delta_no_self", "old_batch_id <> new_batch_id"));
        builder.HasOne(entity => entity.OldBatch).WithMany().HasForeignKey(entity => entity.OldBatchId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_batch_delta_old_batch");
        builder.HasOne(entity => entity.NewBatch).WithMany().HasForeignKey(entity => entity.NewBatchId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_batch_delta_new_batch");
        builder.HasIndex(entity => new { entity.OldBatchId, entity.NewBatchId, entity.EntityType }).HasDatabaseName("ix_batch_delta_batches_entity");
    }
}
