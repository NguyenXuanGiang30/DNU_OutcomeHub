using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Governance;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Governance;

public sealed class DispositionItemConfiguration : IEntityTypeConfiguration<DispositionItem>
{
    public void Configure(EntityTypeBuilder<DispositionItem> builder)
    {
        builder.ToTable("disposition_item", "governance", table =>
            {
                table.HasCheckConstraint("ck_disposition_item_status", "status IN ('PENDING','RUNNING','COMPLETED','FAILED','SKIPPED')");
            });
        builder.HasKey(x => x.Id).HasName("pk_disposition_item");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.DispositionCaseId).HasColumnName("disposition_case_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.GovernedResourceId).HasColumnName("governed_resource_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.RetentionBindingId).HasColumnName("retention_binding_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.PlannedAction).HasColumnName("planned_action").HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();
        builder.Property(x => x.ObjectDeleted).HasColumnName("object_deleted").HasColumnType("boolean").IsRequired();
        builder.Property(x => x.DatabaseAnonymized).HasColumnName("database_anonymized").HasColumnType("boolean").IsRequired();
        builder.Property(x => x.Error).HasColumnName("error").HasColumnType("text").IsRequired(false);
        builder.Property(x => x.CompletedAt).HasColumnName("completed_at").HasColumnType("timestamptz").IsRequired(false);

        builder.HasIndex(x => new { x.DispositionCaseId, x.GovernedResourceId }).IsUnique().HasDatabaseName("uq_disposition_item_case_resource");
        builder.HasIndex(x => new { x.Status, x.CompletedAt }).HasDatabaseName("ix_disposition_item_status_completed");
        builder.HasOne(x => x.DispositionCase).WithMany(x => x.Items).HasForeignKey(x => x.DispositionCaseId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_disposition_item_case");
        builder.HasOne(x => x.GovernedResource).WithMany().HasForeignKey(x => x.GovernedResourceId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_disposition_item_resource");
        builder.HasOne(x => x.RetentionBinding).WithMany().HasForeignKey(x => x.RetentionBindingId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_disposition_item_retention_binding");
    }
}

