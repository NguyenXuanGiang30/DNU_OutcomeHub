using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Portfolio;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Portfolio;

public sealed class SyllabusTemplateRubricScaleConfiguration : IEntityTypeConfiguration<SyllabusTemplateRubricScale>
{
    public void Configure(EntityTypeBuilder<SyllabusTemplateRubricScale> builder)
    {
        builder.ToTable("syllabus_template_rubric_scale", "portfolio", table =>
            {
                table.HasCheckConstraint("ck_syllabus_template_rubric_scale_code", "code = upper(btrim(code)) AND char_length(code) > 0");
            });
        builder.HasKey(x => x.Id).HasName("pk_syllabus_template_rubric_scale");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.SyllabusTemplateVersionId).HasColumnName("syllabus_template_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Code).HasColumnName("code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Name).HasColumnName("name").HasColumnType("varchar(255)").HasMaxLength(255).IsRequired();

        builder.HasAlternateKey(x => new { x.Id, x.SyllabusTemplateVersionId }).HasName("uq_syllabus_template_rubric_scale_id_version");
        builder.HasIndex(x => new { x.SyllabusTemplateVersionId, x.Code }).IsUnique().HasDatabaseName("uq_syllabus_template_rubric_scale_code");
        builder.HasOne(x => x.SyllabusTemplateVersion).WithMany(x => x.RubricScales).HasForeignKey(x => x.SyllabusTemplateVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_syllabus_template_rubric_scale_version");
    }
}

