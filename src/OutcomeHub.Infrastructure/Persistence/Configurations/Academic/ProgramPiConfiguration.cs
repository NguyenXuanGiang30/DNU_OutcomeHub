using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Academic;

public sealed class ProgramPiConfiguration : IEntityTypeConfiguration<ProgramPi>
{
    public void Configure(EntityTypeBuilder<ProgramPi> builder)
    {
        builder.ToTable("program_pi", "academic", table =>
        {
            table.HasCheckConstraint("ck_program_pi_code", "code = upper(btrim(code)) AND char_length(code) > 0");
            table.HasCheckConstraint("ck_program_pi_source_lock", "source_template_pi_id IS NULL OR is_locked");
            table.HasCheckConstraint("ck_program_pi_weight", "weight_ratio IS NULL OR (weight_ratio >= 0 AND weight_ratio <= 1 AND weight_ratio <> 'NaN'::numeric AND weight_ratio NOT IN ('Infinity'::numeric, '-Infinity'::numeric))");
            table.HasCheckConstraint("ck_program_pi_sort_order", "sort_order >= 0");
        });
        builder.HasKey(x => x.Id).HasName("pk_program_pi");
        builder.HasAlternateKey(x => new { x.Id, x.ProgramVersionId }).HasName("uq_program_pi_id_version");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.ProgramVersionId).HasColumnName("program_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.ProgramPloId).HasColumnName("program_plo_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Code).HasColumnName("code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Description).HasColumnName("description").HasColumnType("text").IsRequired();
        builder.Property(x => x.SourceTemplatePiId).HasColumnName("source_template_pi_id").HasColumnType("uuid").IsRequired(false);
        builder.Property(x => x.IsLocked).HasColumnName("is_locked").HasColumnType("boolean").IsRequired();
        builder.Property(x => x.IsCore).HasColumnName("is_core").HasColumnType("boolean").IsRequired();
        builder.Property(x => x.WeightRatio).HasColumnName("weight_ratio").HasColumnType("numeric(12,10)").HasPrecision(12, 10).IsRequired(false);
        builder.Property(x => x.SortOrder).HasColumnName("sort_order").HasColumnType("integer").IsRequired();
        builder.HasIndex(x => new { x.ProgramVersionId, x.Code }).IsUnique().HasDatabaseName("uq_program_pi_version_code");
        builder.HasOne(x => x.ProgramVersion).WithMany().HasForeignKey(x => x.ProgramVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_program_pi_version");
        builder.HasOne(x => x.ProgramPlo).WithMany().HasForeignKey(x => new { x.ProgramPloId, x.ProgramVersionId }).HasPrincipalKey(x => new { x.Id, x.ProgramVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_program_pi_plo_version");
        builder.HasOne(x => x.SourceTemplatePi).WithMany().HasForeignKey(x => x.SourceTemplatePiId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_program_pi_source_template");
    }
}
