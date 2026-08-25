using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Academic;

public sealed class InstitutionTemplateVersionConfiguration : IEntityTypeConfiguration<InstitutionTemplateVersion>
{
    public void Configure(EntityTypeBuilder<InstitutionTemplateVersion> builder)
    {
        builder.ToTable("institution_template_version", "academic", table =>
        {
            table.HasCheckConstraint("ck_institution_template_version_no", "version_no > 0");
            table.HasCheckConstraint("ck_institution_template_version_status", "status IN ('DRAFT','IN_REVIEW','APPROVED','ACTIVE','EXPIRED','REJECTED')");
            table.HasCheckConstraint("ck_institution_template_version_range", "effective_to IS NULL OR effective_to > effective_from");
            table.HasCheckConstraint("ck_institution_template_version_checksum", "checksum ~ '^[0-9a-f]{64}$'");
        });
        builder.HasKey(x => x.Id).HasName("pk_institution_template_version");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.InstitutionTemplateId).HasColumnName("institution_template_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.VersionNo).HasColumnName("version_no").HasColumnType("integer").IsRequired();
        builder.Property(x => x.DecisionId).HasColumnName("decision_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.EffectiveFrom).HasColumnName("effective_from").HasColumnType("date").IsRequired();
        builder.Property(x => x.EffectiveTo).HasColumnName("effective_to").HasColumnType("date").IsRequired(false);
        builder.Property(x => x.Status).HasColumnName("status").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.Property(x => x.LayoutConfiguration).HasColumnName("layout_configuration").HasColumnType("jsonb").IsRequired();
        builder.Property(x => x.PolicyConfiguration).HasColumnName("policy_configuration").HasColumnType("jsonb").IsRequired();
        builder.Property(x => x.WorkflowInstanceId).HasColumnName("workflow_instance_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Checksum).HasColumnName("checksum").HasColumnType("char(64)").HasMaxLength(64).IsFixedLength().IsRequired();
        builder.Property(x => x.SupersedesId).HasColumnName("supersedes_id").HasColumnType("uuid").IsRequired(false);
        builder.HasIndex(x => new { x.InstitutionTemplateId, x.VersionNo }).IsUnique().HasDatabaseName("uq_institution_template_version_template_no");
        builder.HasIndex(x => x.WorkflowInstanceId).IsUnique().HasDatabaseName("uq_institution_template_version_workflow");
        builder.HasOne(x => x.InstitutionTemplate).WithMany().HasForeignKey(x => x.InstitutionTemplateId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_institution_template_version_template");
        builder.HasOne(x => x.Decision).WithMany().HasForeignKey(x => x.DecisionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_institution_template_version_decision");
        builder.HasOne(x => x.WorkflowInstance).WithMany().HasForeignKey(x => x.WorkflowInstanceId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_institution_template_version_workflow");
        builder.HasOne(x => x.Supersedes).WithMany().HasForeignKey(x => x.SupersedesId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_institution_template_version_supersedes");
    }
}
