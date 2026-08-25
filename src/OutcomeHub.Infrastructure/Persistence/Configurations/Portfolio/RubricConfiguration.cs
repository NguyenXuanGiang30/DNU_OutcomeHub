using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Portfolio;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Portfolio;

public sealed class RubricConfiguration : IEntityTypeConfiguration<Rubric>
{
    public void Configure(EntityTypeBuilder<Rubric> builder)
    {
        builder.ToTable("rubric", "portfolio", table =>
            {
                table.HasCheckConstraint("ck_rubric_code", "code = upper(btrim(code)) AND char_length(code) > 0");
                table.HasCheckConstraint("ck_rubric_max_score", "max_score NOT IN ('NaN'::numeric,'Infinity'::numeric,'-Infinity'::numeric) AND max_score > 0");
                table.HasCheckConstraint("ck_rubric_checksum", "checksum ~ '^[0-9a-f]{64}$'");
            });
        builder.HasKey(x => x.Id).HasName("pk_rubric");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.SyllabusVersionId).HasColumnName("syllabus_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.SyllabusTemplateVersionId).HasColumnName("syllabus_template_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.AssessmentItemId).HasColumnName("assessment_item_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Code).HasColumnName("code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Name).HasColumnName("name").HasColumnType("varchar(255)").HasMaxLength(255).IsRequired();
        builder.Property(x => x.MaxScore).HasColumnName("max_score").HasColumnType("numeric(20,10)").HasPrecision(20, 10).IsRequired();
        builder.Property(x => x.RubricScaleId).HasColumnName("rubric_scale_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Checksum).HasColumnName("checksum").HasColumnType("char(64)").HasMaxLength(64).IsRequired();

        builder.HasAlternateKey(x => new { x.Id, x.AssessmentItemId, x.SyllabusVersionId }).HasName("uq_rubric_full_binding");
        builder.HasIndex(x => x.AssessmentItemId).IsUnique().HasDatabaseName("uq_rubric_assessment_item");
        builder.HasIndex(x => new { x.SyllabusVersionId, x.Code }).IsUnique().HasDatabaseName("uq_rubric_version_code");
        builder.HasOne(x => x.SyllabusVersion).WithMany().HasForeignKey(x => new { x.SyllabusVersionId, x.SyllabusTemplateVersionId }).HasPrincipalKey(x => new { x.Id, x.SyllabusTemplateVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_rubric_syllabus_template");
        builder.HasOne(x => x.SyllabusTemplateVersion).WithMany().HasForeignKey(x => x.SyllabusTemplateVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_rubric_template_version");
        builder.HasOne(x => x.AssessmentItem).WithOne(x => x.Rubric).HasForeignKey<Rubric>(x => new { x.AssessmentItemId, x.SyllabusVersionId }).HasPrincipalKey<AssessmentItem>(x => new { x.Id, x.SyllabusVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_rubric_assessment_item_version");
        builder.HasOne(x => x.RubricScale).WithMany().HasForeignKey(x => new { x.RubricScaleId, x.SyllabusTemplateVersionId }).HasPrincipalKey(x => new { x.Id, x.SyllabusTemplateVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_rubric_scale_template_version");
    }
}
