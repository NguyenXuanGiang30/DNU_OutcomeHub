using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Portfolio;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Portfolio;

public sealed class LearningMaterialConfiguration : IEntityTypeConfiguration<LearningMaterial>
{
    public void Configure(EntityTypeBuilder<LearningMaterial> builder)
    {
        builder.ToTable("learning_material", "portfolio", table =>
            {
                table.HasCheckConstraint("ck_learning_material_sort_order", "sort_order >= 0");
            });
        builder.HasKey(x => x.Id).HasName("pk_learning_material");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.SyllabusVersionId).HasColumnName("syllabus_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.MaterialType).HasColumnName("material_type").HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();
        builder.Property(x => x.Citation).HasColumnName("citation").HasColumnType("text").IsRequired();
        builder.Property(x => x.Url).HasColumnName("url").HasColumnType("varchar(2048)").HasMaxLength(2048).IsRequired(false);
        builder.Property(x => x.Required).HasColumnName("required").HasColumnType("boolean").IsRequired();
        builder.Property(x => x.SortOrder).HasColumnName("sort_order").HasColumnType("integer").IsRequired();

        builder.HasAlternateKey(x => new { x.Id, x.SyllabusVersionId }).HasName("uq_learning_material_id_version");
        builder.HasIndex(x => new { x.SyllabusVersionId, x.SortOrder }).HasDatabaseName("ix_learning_material_version_sort");
        builder.HasOne(x => x.SyllabusVersion).WithMany().HasForeignKey(x => x.SyllabusVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_learning_material_syllabus_version");
    }
}

