using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Governance;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Governance;

public sealed class GovernedResourceConfiguration : IEntityTypeConfiguration<GovernedResource>
{
    public void Configure(EntityTypeBuilder<GovernedResource> builder)
    {
        builder.ToTable("governed_resource", "governance", table =>
            {
                table.HasCheckConstraint("ck_governed_resource_classification", "classification IN ('PUBLIC','INTERNAL','CONFIDENTIAL','RESTRICTED')");
                table.HasCheckConstraint("ck_governed_resource_disposition_status", "disposition_status IN ('ACTIVE','ON_HOLD','ELIGIBLE','PENDING','DISPOSED','FAILED')");
            });
        builder.HasKey(x => x.Id).HasName("pk_governed_resource");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.ResourceType).HasColumnName("resource_type").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Classification).HasColumnName("classification").HasColumnType("varchar(16)").HasMaxLength(16).IsRequired();
        builder.Property(x => x.DispositionStatus).HasColumnName("disposition_status").HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz").IsRequired();

        builder.HasIndex(x => new { x.ResourceType, x.DispositionStatus }).HasDatabaseName("ix_governed_resource_type_status");
    }
}

