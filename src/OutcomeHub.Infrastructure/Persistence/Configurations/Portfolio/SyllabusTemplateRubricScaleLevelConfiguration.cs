using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Portfolio;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Portfolio;

public sealed class SyllabusTemplateRubricScaleLevelConfiguration : IEntityTypeConfiguration<SyllabusTemplateRubricScaleLevel>
{
    public void Configure(EntityTypeBuilder<SyllabusTemplateRubricScaleLevel> builder)
    {
        builder.ToTable("syllabus_template_rubric_scale_level", "portfolio", table =>
            {
                table.HasCheckConstraint("ck_syllabus_template_rubric_scale_level_range", "score_from < score_to");
                table.HasCheckConstraint("ck_syllabus_template_rubric_scale_level_order", "level_order >= 0");
            });
        builder.HasKey(x => x.Id).HasName("pk_syllabus_template_rubric_scale_level");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.RubricScaleId).HasColumnName("rubric_scale_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.LevelCode).HasColumnName("level_code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Label).HasColumnName("label").HasColumnType("varchar(255)").HasMaxLength(255).IsRequired();
        builder.Property(x => x.LevelOrder).HasColumnName("level_order").HasColumnType("integer").IsRequired();
        builder.Property(x => x.ScoreFrom).HasColumnName("score_from").HasColumnType("numeric(20,10)").HasPrecision(20, 10).IsRequired();
        builder.Property(x => x.ScoreTo).HasColumnName("score_to").HasColumnType("numeric(20,10)").HasPrecision(20, 10).IsRequired();
        builder.Property(x => x.NumericValue).HasColumnName("numeric_value").HasColumnType("numeric(20,10)").HasPrecision(20, 10).IsRequired(false);

        builder.HasIndex(x => new { x.RubricScaleId, x.LevelCode }).IsUnique().HasDatabaseName("uq_syllabus_template_rubric_scale_level_code");
        builder.HasIndex(x => new { x.RubricScaleId, x.LevelOrder }).IsUnique().HasDatabaseName("uq_syllabus_template_rubric_scale_level_order");
        builder.HasAnnotation("OutcomeHub:ExclusionConstraint:ex_syllabus_template_rubric_scale_level_range", "rubric_scale_id WITH =, numrange(score_from, score_to, '[)') WITH &&");
        builder.HasOne(x => x.RubricScale).WithMany(x => x.Levels).HasForeignKey(x => x.RubricScaleId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_syllabus_template_rubric_scale_level_scale");
    }
}
