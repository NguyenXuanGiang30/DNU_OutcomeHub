using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Governance;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Governance;

public sealed class ObjectReferenceConfiguration : IEntityTypeConfiguration<ObjectReference>
{
    public void Configure(EntityTypeBuilder<ObjectReference> builder)
    {
        builder.ToTable("object_reference", "governance", table =>
            {
                table.HasCheckConstraint("ck_object_reference_effective_range", "effective_to IS NULL OR effective_to > effective_from");
            });
        builder.HasKey(x => new { x.GovernedResourceId, x.FileObjectId, x.ReferenceRole, x.EffectiveFrom }).HasName("pk_object_reference");
        builder.Property(x => x.GovernedResourceId).HasColumnName("governed_resource_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.FileObjectId).HasColumnName("file_object_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.ReferenceRole).HasColumnName("reference_role").HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();
        builder.Property(x => x.EffectiveFrom).HasColumnName("effective_from").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.EffectiveTo).HasColumnName("effective_to").HasColumnType("timestamptz").IsRequired(false);

        builder.HasIndex(x => new { x.FileObjectId, x.EffectiveTo }).HasDatabaseName("ix_object_reference_file_effective_to");
        builder.HasOne(x => x.GovernedResource).WithMany().HasForeignKey(x => x.GovernedResourceId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_object_reference_resource");
        builder.HasOne(x => x.FileObject).WithMany().HasForeignKey(x => x.FileObjectId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_object_reference_file_object");
    }
}

