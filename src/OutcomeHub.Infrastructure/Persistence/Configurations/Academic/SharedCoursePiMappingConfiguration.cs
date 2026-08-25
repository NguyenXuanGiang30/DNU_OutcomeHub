using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Academic;

public sealed class SharedCoursePiMappingConfiguration : IEntityTypeConfiguration<SharedCoursePiMapping>
{
    public void Configure(EntityTypeBuilder<SharedCoursePiMapping> builder)
    {
        builder.ToTable("shared_course_pi_mapping", "academic", table =>
        {
            table.HasCheckConstraint("ck_shared_course_pi_mapping_version", "version_no > 0");
            table.HasCheckConstraint("ck_shared_course_pi_mapping_contribution", "contribution_level IN ('I','R','M')");
            table.HasCheckConstraint("ck_shared_course_pi_mapping_status", "status IN ('DRAFT','IN_REVIEW','APPROVED','ACTIVE','EXPIRED','REJECTED')");
            table.HasCheckConstraint("ck_shared_course_pi_mapping_checksum", "checksum ~ '^[0-9a-f]{64}$'");
        });
        builder.HasKey(x => x.Id).HasName("pk_shared_course_pi_mapping");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.CourseVersionId).HasColumnName("course_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.InstitutionTemplateVersionId).HasColumnName("institution_template_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.TemplatePiId).HasColumnName("template_pi_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.VersionNo).HasColumnName("version_no").HasColumnType("integer").IsRequired();
        builder.Property(x => x.ContributionLevel).HasColumnName("contribution_level").HasColumnType("char(1)").HasMaxLength(1).IsFixedLength().IsRequired();
        builder.Property(x => x.IsDirectAssessment).HasColumnName("is_direct_assessment").HasColumnType("boolean").IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.Property(x => x.DecisionId).HasColumnName("decision_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.WorkflowInstanceId).HasColumnName("workflow_instance_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Checksum).HasColumnName("checksum").HasColumnType("char(64)").HasMaxLength(64).IsFixedLength().IsRequired();
        builder.HasIndex(x => new { x.CourseVersionId, x.TemplatePiId, x.VersionNo }).IsUnique().HasDatabaseName("uq_shared_course_pi_mapping_version");
        builder.HasIndex(x => x.WorkflowInstanceId).IsUnique().HasDatabaseName("uq_shared_course_pi_mapping_workflow");
        builder.HasOne(x => x.CourseVersion).WithMany().HasForeignKey(x => x.CourseVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_shared_course_pi_mapping_course_version");
        builder.HasOne(x => x.InstitutionTemplateVersion).WithMany().HasForeignKey(x => x.InstitutionTemplateVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_shared_course_pi_mapping_template_version");
        builder.HasOne(x => x.TemplatePi).WithMany().HasForeignKey(x => new { x.TemplatePiId, x.InstitutionTemplateVersionId }).HasPrincipalKey(x => new { x.Id, x.InstitutionTemplateVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_shared_course_pi_mapping_pi_version");
        builder.HasOne(x => x.Decision).WithMany().HasForeignKey(x => x.DecisionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_shared_course_pi_mapping_decision");
        builder.HasOne(x => x.WorkflowInstance).WithMany().HasForeignKey(x => x.WorkflowInstanceId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_shared_course_pi_mapping_workflow");
    }
}
