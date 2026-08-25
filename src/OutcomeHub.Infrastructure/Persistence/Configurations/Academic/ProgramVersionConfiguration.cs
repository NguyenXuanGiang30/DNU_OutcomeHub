using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Academic;

public sealed class ProgramVersionConfiguration : IEntityTypeConfiguration<ProgramVersion>
{
    public void Configure(EntityTypeBuilder<ProgramVersion> builder)
    {
        builder.ToTable("program_version", "academic", table =>
        {
            table.HasCheckConstraint("ck_program_version_no", "version_no > 0");
            table.HasCheckConstraint("ck_program_version_code", "code = upper(btrim(code)) AND char_length(code) > 0");
            table.HasCheckConstraint("ck_program_version_status", "status IN ('DRAFT','IN_REVIEW','APPROVED','ACTIVE','EXPIRED','REJECTED')");
            table.HasCheckConstraint("ck_program_version_credits", "total_credits > 0 AND total_credits <> 'NaN'::numeric AND total_credits NOT IN ('Infinity'::numeric, '-Infinity'::numeric)");
            table.HasCheckConstraint("ck_program_version_range", "effective_to IS NULL OR effective_to > effective_from");
            table.HasCheckConstraint("ck_program_version_checksum", "checksum ~ '^[0-9a-f]{64}$'");
        });
        builder.HasKey(x => x.Id).HasName("pk_program_version");
        builder.HasAlternateKey(x => new { x.Id, x.ProgramId }).HasName("uq_program_version_id_program");
        builder.HasAlternateKey(x => new { x.Id, x.InstitutionTemplateVersionId }).HasName("uq_program_version_id_template");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.ProgramId).HasColumnName("program_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.InstitutionTemplateVersionId).HasColumnName("institution_template_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.VersionNo).HasColumnName("version_no").HasColumnType("integer").IsRequired();
        builder.Property(x => x.Code).HasColumnName("code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.DecisionId).HasColumnName("decision_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.EffectiveFrom).HasColumnName("effective_from").HasColumnType("date").IsRequired();
        builder.Property(x => x.EffectiveTo).HasColumnName("effective_to").HasColumnType("date").IsRequired(false);
        builder.Property(x => x.Status).HasColumnName("status").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.Property(x => x.TotalCredits).HasColumnName("total_credits").HasColumnType("numeric(10,2)").HasPrecision(10, 2).IsRequired();
        builder.Property(x => x.WorkflowInstanceId).HasColumnName("workflow_instance_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.SupersedesId).HasColumnName("supersedes_id").HasColumnType("uuid").IsRequired(false);
        builder.Property(x => x.Checksum).HasColumnName("checksum").HasColumnType("char(64)").HasMaxLength(64).IsFixedLength().IsRequired();
        builder.Property(x => x.RowVersion).HasColumnName("row_version").HasColumnType("bigint").HasDefaultValue(1L).IsConcurrencyToken().IsRequired();
        builder.HasIndex(x => new { x.ProgramId, x.VersionNo }).IsUnique().HasDatabaseName("uq_program_version_program_no");
        builder.HasIndex(x => new { x.ProgramId, x.Code }).IsUnique().HasDatabaseName("uq_program_version_program_code");
        builder.HasIndex(x => x.WorkflowInstanceId).IsUnique().HasDatabaseName("uq_program_version_workflow");
        builder.HasOne(x => x.Program).WithMany().HasForeignKey(x => x.ProgramId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_program_version_program");
        builder.HasOne(x => x.InstitutionTemplateVersion).WithMany().HasForeignKey(x => x.InstitutionTemplateVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_program_version_template_version");
        builder.HasOne(x => x.Decision).WithMany().HasForeignKey(x => x.DecisionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_program_version_decision");
        builder.HasOne(x => x.WorkflowInstance).WithMany().HasForeignKey(x => x.WorkflowInstanceId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_program_version_workflow");
        builder.HasOne(x => x.Supersedes).WithMany().HasForeignKey(x => x.SupersedesId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_program_version_supersedes");
    }
}
