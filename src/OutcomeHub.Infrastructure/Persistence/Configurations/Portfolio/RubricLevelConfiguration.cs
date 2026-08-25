using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NpgsqlTypes;
using OutcomeHub.Domain.Entities.Portfolio;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Portfolio;

public sealed class RubricLevelConfiguration : IEntityTypeConfiguration<RubricLevel>
{
    public void Configure(EntityTypeBuilder<RubricLevel> builder)
    {
        builder.ToTable("rubric_level", "portfolio", table =>
            {
                table.HasCheckConstraint("ck_rubric_level_range", "score_from NOT IN ('NaN'::numeric,'Infinity'::numeric,'-Infinity'::numeric) AND score_to NOT IN ('NaN'::numeric,'Infinity'::numeric,'-Infinity'::numeric) AND score_from < score_to");
                table.HasCheckConstraint("ck_rubric_level_order", "level_order >= 0");
            });
        builder.HasKey(x => x.Id).HasName("pk_rubric_level");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.RubricCriterionId).HasColumnName("rubric_criterion_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.LevelCode).HasColumnName("level_code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.LevelOrder).HasColumnName("level_order").HasColumnType("integer").IsRequired();
        builder.Property(x => x.Label).HasColumnName("label").HasColumnType("varchar(255)").HasMaxLength(255).IsRequired();
        builder.Property(x => x.Description).HasColumnName("description").HasColumnType("text").IsRequired(false);
        builder.Property(x => x.ScoreFrom).HasColumnName("score_from").HasColumnType("numeric(20,10)").HasPrecision(20, 10).IsRequired();
        builder.Property(x => x.ScoreTo).HasColumnName("score_to").HasColumnType("numeric(20,10)").HasPrecision(20, 10).IsRequired();
        builder.Property(x => x.NumericValue).HasColumnName("numeric_value").HasColumnType("numeric(20,10)").HasPrecision(20, 10).IsRequired(false);

        builder.Property<NpgsqlRange<decimal>>("ScoreRange").HasColumnName("score_range").HasColumnType("numrange").HasComputedColumnSql("numrange(score_from, score_to, '[)')", stored: true);
        builder.HasIndex(x => new { x.RubricCriterionId, x.LevelCode }).IsUnique().HasDatabaseName("uq_rubric_level_code");
        builder.HasIndex(x => new { x.RubricCriterionId, x.LevelOrder }).IsUnique().HasDatabaseName("uq_rubric_level_order");
        builder.HasAnnotation("OutcomeHub:ExclusionConstraint:ex_rubric_level_range", "rubric_criterion_id WITH =, score_range WITH &&");
        builder.HasOne(x => x.RubricCriterion).WithMany(x => x.Levels).HasForeignKey(x => x.RubricCriterionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_rubric_level_criterion");
    }
}
