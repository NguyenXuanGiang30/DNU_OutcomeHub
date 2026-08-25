using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Academic;

public sealed class CourseVersionConfiguration : IEntityTypeConfiguration<CourseVersion>
{
    public void Configure(EntityTypeBuilder<CourseVersion> builder)
    {
        builder.ToTable("course_version", "academic", table =>
        {
            table.HasCheckConstraint("ck_course_version_no", "version_no > 0");
            table.HasCheckConstraint("ck_course_version_credit", "credit_value > 0 AND credit_value <> 'NaN'::numeric AND credit_value NOT IN ('Infinity'::numeric, '-Infinity'::numeric)");
            table.HasCheckConstraint("ck_course_version_type", "course_type IN ('STANDARD','PRACTICE','INTERNSHIP','PROJECT','THESIS','CLINICAL')");
            table.HasCheckConstraint("ck_course_version_status", "status IN ('DRAFT','IN_REVIEW','APPROVED','ACTIVE','EXPIRED','REJECTED')");
            table.HasCheckConstraint("ck_course_version_range", "effective_to IS NULL OR effective_to > effective_from");
            table.HasCheckConstraint("ck_course_version_checksum", "checksum ~ '^[0-9a-f]{64}$'");
        });
        builder.HasKey(x => x.Id).HasName("pk_course_version");
        builder.HasAlternateKey(x => new { x.Id, x.CourseId }).HasName("uq_course_version_id_course");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.CourseId).HasColumnName("course_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.VersionNo).HasColumnName("version_no").HasColumnType("integer").IsRequired();
        builder.Property(x => x.Name).HasColumnName("name").HasColumnType("varchar(255)").HasMaxLength(255).IsRequired();
        builder.Property(x => x.CreditValue).HasColumnName("credit_value").HasColumnType("numeric(10,2)").HasPrecision(10, 2).IsRequired();
        builder.Property(x => x.CourseType).HasColumnName("course_type").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.Property(x => x.EffectiveFrom).HasColumnName("effective_from").HasColumnType("date").IsRequired();
        builder.Property(x => x.EffectiveTo).HasColumnName("effective_to").HasColumnType("date").IsRequired(false);
        builder.Property(x => x.SharedCoreFlag).HasColumnName("shared_core_flag").HasColumnType("boolean").IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.Property(x => x.DecisionId).HasColumnName("decision_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.WorkflowInstanceId).HasColumnName("workflow_instance_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.SupersedesId).HasColumnName("supersedes_id").HasColumnType("uuid").IsRequired(false);
        builder.Property(x => x.Checksum).HasColumnName("checksum").HasColumnType("char(64)").HasMaxLength(64).IsFixedLength().IsRequired();
        builder.HasIndex(x => new { x.CourseId, x.VersionNo }).IsUnique().HasDatabaseName("uq_course_version_course_no");
        builder.HasIndex(x => x.WorkflowInstanceId).IsUnique().HasDatabaseName("uq_course_version_workflow");
        builder.HasOne(x => x.Course).WithMany().HasForeignKey(x => x.CourseId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_course_version_course");
        builder.HasOne(x => x.Decision).WithMany().HasForeignKey(x => x.DecisionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_course_version_decision");
        builder.HasOne(x => x.WorkflowInstance).WithMany().HasForeignKey(x => x.WorkflowInstanceId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_course_version_workflow");
        builder.HasOne(x => x.Supersedes).WithMany().HasForeignKey(x => x.SupersedesId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_course_version_supersedes");
    }
}
