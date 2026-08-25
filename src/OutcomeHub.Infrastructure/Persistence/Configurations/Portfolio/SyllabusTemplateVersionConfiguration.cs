using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Portfolio;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Portfolio;

public sealed class SyllabusTemplateVersionConfiguration : IEntityTypeConfiguration<SyllabusTemplateVersion>
{
    public void Configure(EntityTypeBuilder<SyllabusTemplateVersion> builder)
    {
        builder.ToTable("syllabus_template_version", "portfolio", table =>
            {
                table.HasCheckConstraint("ck_syllabus_template_version_version_no", "version_no > 0");
                table.HasCheckConstraint("ck_syllabus_template_version_effective_range", "effective_to IS NULL OR effective_to > effective_from");
                table.HasCheckConstraint("ck_syllabus_template_version_status", "status IN ('DRAFT','IN_REVIEW','APPROVED','ACTIVE','EXPIRED','REJECTED')");
                table.HasCheckConstraint("ck_syllabus_template_version_checksum", "checksum ~ '^[0-9a-f]{64}$'");
            });
        builder.HasKey(x => x.Id).HasName("pk_syllabus_template_version");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.SyllabusTemplateId).HasColumnName("syllabus_template_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.InstitutionTemplateVersionId).HasColumnName("institution_template_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.VersionNo).HasColumnName("version_no").HasColumnType("integer").IsRequired();
        builder.Property(x => x.DecisionId).HasColumnName("decision_id").HasColumnType("uuid").IsRequired(false);
        builder.Property(x => x.EffectiveFrom).HasColumnName("effective_from").HasColumnType("date").IsRequired();
        builder.Property(x => x.EffectiveTo).HasColumnName("effective_to").HasColumnType("date").IsRequired(false);
        builder.Property(x => x.Status).HasColumnName("status").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.Property(x => x.WorkflowInstanceId).HasColumnName("workflow_instance_id").HasColumnType("uuid").IsRequired(false);
        builder.Property(x => x.SupersedesId).HasColumnName("supersedes_id").HasColumnType("uuid").IsRequired(false);
        builder.Property(x => x.Checksum).HasColumnName("checksum").HasColumnType("char(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.CreatedBy).HasColumnName("created_by").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.UpdatedBy).HasColumnName("updated_by").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.RowVersion).HasColumnName("row_version").HasColumnType("bigint").IsRequired();

        builder.HasAlternateKey(x => new { x.Id, x.InstitutionTemplateVersionId }).HasName("uq_syllabus_template_version_id_institution");
        builder.HasIndex(x => new { x.SyllabusTemplateId, x.VersionNo }).IsUnique().HasDatabaseName("uq_syllabus_template_version_template_version_no");
        builder.HasIndex(x => x.WorkflowInstanceId).IsUnique().HasFilter("workflow_instance_id IS NOT NULL").HasDatabaseName("uq_syllabus_template_version_workflow");
        builder.Property(x => x.RowVersion).HasDefaultValue(1L).IsConcurrencyToken();
        builder.HasOne(x => x.SyllabusTemplate).WithMany(x => x.Versions).HasForeignKey(x => x.SyllabusTemplateId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_syllabus_template_version_template");
        builder.HasOne(x => x.InstitutionTemplateVersion).WithMany().HasForeignKey(x => x.InstitutionTemplateVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_syllabus_template_version_institution_template_version");
        builder.HasOne(x => x.Decision).WithMany().HasForeignKey(x => x.DecisionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_syllabus_template_version_decision");
        builder.HasOne(x => x.WorkflowInstance).WithMany().HasForeignKey(x => x.WorkflowInstanceId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_syllabus_template_version_workflow");
        builder.HasOne(x => x.Supersedes).WithMany(x => x.Successors).HasForeignKey(x => x.SupersedesId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_syllabus_template_version_supersedes");
    }
}

