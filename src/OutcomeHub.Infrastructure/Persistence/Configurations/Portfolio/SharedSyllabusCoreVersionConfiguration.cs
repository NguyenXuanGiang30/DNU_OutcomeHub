using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Portfolio;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Portfolio;

public sealed class SharedSyllabusCoreVersionConfiguration : IEntityTypeConfiguration<SharedSyllabusCoreVersion>
{
    public void Configure(EntityTypeBuilder<SharedSyllabusCoreVersion> builder)
    {
        builder.ToTable("shared_syllabus_core_version", "portfolio", table =>
            {
                table.HasCheckConstraint("ck_shared_syllabus_core_version_version_no", "version_no > 0");
                table.HasCheckConstraint("ck_shared_syllabus_core_version_status", "status IN ('DRAFT','IN_REVIEW','APPROVED','ACTIVE','EXPIRED','REJECTED')");
                table.HasCheckConstraint("ck_shared_syllabus_core_version_checksum", "checksum ~ '^[0-9a-f]{64}$'");
            });
        builder.HasKey(x => x.Id).HasName("pk_shared_syllabus_core_version");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.SharedSyllabusCoreId).HasColumnName("shared_syllabus_core_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.CourseVersionId).HasColumnName("course_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.VersionNo).HasColumnName("version_no").HasColumnType("integer").IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.Property(x => x.DecisionId).HasColumnName("decision_id").HasColumnType("uuid").IsRequired(false);
        builder.Property(x => x.WorkflowInstanceId).HasColumnName("workflow_instance_id").HasColumnType("uuid").IsRequired(false);
        builder.Property(x => x.SupersedesId).HasColumnName("supersedes_id").HasColumnType("uuid").IsRequired(false);
        builder.Property(x => x.Checksum).HasColumnName("checksum").HasColumnType("char(64)").HasMaxLength(64).IsRequired();

        builder.HasAlternateKey(x => new { x.Id, x.CourseVersionId }).HasName("uq_shared_syllabus_core_version_id_course_version");
        builder.HasIndex(x => new { x.SharedSyllabusCoreId, x.VersionNo }).IsUnique().HasDatabaseName("uq_shared_syllabus_core_version_no");
        builder.HasIndex(x => x.WorkflowInstanceId).IsUnique().HasFilter("workflow_instance_id IS NOT NULL").HasDatabaseName("uq_shared_syllabus_core_version_workflow");
        builder.HasOne(x => x.SharedSyllabusCore).WithMany(x => x.Versions).HasForeignKey(x => x.SharedSyllabusCoreId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_shared_syllabus_core_version_core");
        builder.HasOne(x => x.CourseVersion).WithMany().HasForeignKey(x => x.CourseVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_shared_syllabus_core_version_course_version");
        builder.HasOne(x => x.Decision).WithMany().HasForeignKey(x => x.DecisionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_shared_syllabus_core_version_decision");
        builder.HasOne(x => x.WorkflowInstance).WithMany().HasForeignKey(x => x.WorkflowInstanceId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_shared_syllabus_core_version_workflow");
        builder.HasOne(x => x.Supersedes).WithMany(x => x.Successors).HasForeignKey(x => x.SupersedesId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_shared_syllabus_core_version_supersedes");
    }
}

