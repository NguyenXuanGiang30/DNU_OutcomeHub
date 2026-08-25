using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Governance;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Governance;

public sealed class LegalHoldItemConfiguration : IEntityTypeConfiguration<LegalHoldItem>
{
    public void Configure(EntityTypeBuilder<LegalHoldItem> builder)
    {
        builder.ToTable("legal_hold_item", "governance");
        builder.HasKey(x => new { x.LegalHoldId, x.GovernedResourceId }).HasName("pk_legal_hold_item");
        builder.Property(x => x.LegalHoldId).HasColumnName("legal_hold_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.GovernedResourceId).HasColumnName("governed_resource_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.AddedAt).HasColumnName("added_at").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.AddedBy).HasColumnName("added_by").HasColumnType("uuid").IsRequired();

        builder.HasIndex(x => x.GovernedResourceId).HasDatabaseName("ix_legal_hold_item_resource");
        builder.HasOne(x => x.LegalHold).WithMany(x => x.Items).HasForeignKey(x => x.LegalHoldId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_legal_hold_item_hold");
        builder.HasOne(x => x.GovernedResource).WithMany().HasForeignKey(x => x.GovernedResourceId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_legal_hold_item_resource");
        builder.HasOne(x => x.AddedByPrincipal).WithMany().HasForeignKey(x => x.AddedBy).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_legal_hold_item_added_by");
    }
}

