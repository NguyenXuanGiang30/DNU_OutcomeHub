using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Academic;

public sealed class CurriculumPathConfiguration : IEntityTypeConfiguration<CurriculumPath>
{
    public void Configure(EntityTypeBuilder<CurriculumPath> builder)
    {
        builder.ToTable("curriculum_path", "academic", table =>
        {
            table.HasCheckConstraint("ck_curriculum_path_code", "code = upper(btrim(code)) AND char_length(code) > 0");
            table.HasCheckConstraint("ck_curriculum_path_type", "path_type IN ('COMMON','MAJOR','SPECIALIZATION','ELECTIVE_ROUTE','GRADUATION_OPTION')");
            table.HasCheckConstraint("ck_curriculum_path_range", "effective_to IS NULL OR effective_to > effective_from");
        });
        builder.HasKey(x => x.Id).HasName("pk_curriculum_path");
        builder.HasAlternateKey(x => new { x.Id, x.ProgramVersionId }).HasName("uq_curriculum_path_id_version");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.ProgramVersionId).HasColumnName("program_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Code).HasColumnName("code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Name).HasColumnName("name").HasColumnType("varchar(255)").HasMaxLength(255).IsRequired();
        builder.Property(x => x.PathType).HasColumnName("path_type").HasColumnType("varchar(24)").HasMaxLength(24).IsRequired();
        builder.Property(x => x.EffectiveFrom).HasColumnName("effective_from").HasColumnType("date").IsRequired();
        builder.Property(x => x.EffectiveTo).HasColumnName("effective_to").HasColumnType("date").IsRequired(false);
        builder.Property(x => x.IsDefault).HasColumnName("is_default").HasColumnType("boolean").IsRequired();
        builder.Property(x => x.WorkflowInstanceId).HasColumnName("workflow_instance_id").HasColumnType("uuid").IsRequired();
        builder.HasIndex(x => new { x.ProgramVersionId, x.Code }).IsUnique().HasDatabaseName("uq_curriculum_path_version_code");
        builder.HasIndex(x => x.WorkflowInstanceId).IsUnique().HasDatabaseName("uq_curriculum_path_workflow");
        builder.HasOne(x => x.ProgramVersion).WithMany().HasForeignKey(x => x.ProgramVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_curriculum_path_program_version");
        builder.HasOne(x => x.WorkflowInstance).WithMany().HasForeignKey(x => x.WorkflowInstanceId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_curriculum_path_workflow");
    }
}
