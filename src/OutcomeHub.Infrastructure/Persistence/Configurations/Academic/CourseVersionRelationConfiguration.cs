using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Academic;

public sealed class CourseVersionRelationConfiguration : IEntityTypeConfiguration<CourseVersionRelation>
{
    public void Configure(EntityTypeBuilder<CourseVersionRelation> builder)
    {
        builder.ToTable("course_version_relation", "academic", table =>
        {
            table.HasCheckConstraint("ck_course_version_relation_distinct", "from_course_version_id <> to_course_version_id");
            table.HasCheckConstraint("ck_course_version_relation_type", "relation_type IN ('EQUIVALENT','SUBSTITUTE','REPLACES','RECOGNIZED_AS')");
            table.HasCheckConstraint("ck_course_version_relation_status", "status IN ('DRAFT','IN_REVIEW','APPROVED','ACTIVE','EXPIRED','REJECTED')");
            table.HasCheckConstraint("ck_course_version_relation_range", "effective_to IS NULL OR effective_to > effective_from");
        });
        builder.HasKey(x => x.Id).HasName("pk_course_version_relation");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.FromCourseVersionId).HasColumnName("from_course_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.ToCourseVersionId).HasColumnName("to_course_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.ProgramVersionId).HasColumnName("program_version_id").HasColumnType("uuid").IsRequired(false);
        builder.Property(x => x.RelationType).HasColumnName("relation_type").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.Property(x => x.DecisionId).HasColumnName("decision_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.EffectiveFrom).HasColumnName("effective_from").HasColumnType("date").IsRequired();
        builder.Property(x => x.EffectiveTo).HasColumnName("effective_to").HasColumnType("date").IsRequired(false);
        builder.Property(x => x.Status).HasColumnName("status").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.Property(x => x.Rationale).HasColumnName("rationale").HasColumnType("text").IsRequired(false);
        builder.HasIndex(x => new { x.FromCourseVersionId, x.ToCourseVersionId, x.ProgramVersionId, x.RelationType }).IsUnique().HasDatabaseName("uq_course_version_relation_scope");
        builder.HasOne(x => x.FromCourseVersion).WithMany().HasForeignKey(x => x.FromCourseVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_course_version_relation_from");
        builder.HasOne(x => x.ToCourseVersion).WithMany().HasForeignKey(x => x.ToCourseVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_course_version_relation_to");
        builder.HasOne(x => x.ProgramVersion).WithMany().HasForeignKey(x => x.ProgramVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_course_version_relation_program_version");
        builder.HasOne(x => x.Decision).WithMany().HasForeignKey(x => x.DecisionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_course_version_relation_decision");
    }
}
