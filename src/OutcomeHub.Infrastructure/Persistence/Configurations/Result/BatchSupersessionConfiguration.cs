using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Result;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Result;

public sealed class BatchSupersessionConfiguration : IEntityTypeConfiguration<BatchSupersession>
{
    public void Configure(EntityTypeBuilder<BatchSupersession> builder)
    {
        builder.ToTable("batch_supersession", "result");

        builder.HasKey(entity => entity.OldBatchId)
            .HasName("pk_batch_supersession");

        builder.Property(entity => entity.OldBatchId)
            .HasColumnName("old_batch_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.NewBatchId)
            .HasColumnName("new_batch_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.Reason)
            .HasColumnName("reason")
            .HasColumnType("text")
            .IsRequired(true);

        builder.Property(entity => entity.CreatedBy)
            .HasColumnName("created_by")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamptz")
            .IsRequired(true);

        builder.ToTable(table => table.HasCheckConstraint("ck_batch_supersession_no_self", "old_batch_id <> new_batch_id"));
        builder.HasOne(entity => entity.OldBatch).WithMany().HasForeignKey(entity => entity.OldBatchId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_batch_supersession_old_batch");
        builder.HasOne(entity => entity.NewBatch).WithMany().HasForeignKey(entity => entity.NewBatchId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_batch_supersession_new_batch");
        builder.HasOne(entity => entity.CreatedByPrincipal).WithMany().HasForeignKey(entity => entity.CreatedBy).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_batch_supersession_created_by");
    }
}
