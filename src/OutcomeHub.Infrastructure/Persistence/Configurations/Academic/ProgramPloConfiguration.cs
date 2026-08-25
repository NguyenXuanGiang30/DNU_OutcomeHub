using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Academic;

public sealed class ProgramPloConfiguration : IEntityTypeConfiguration<ProgramPlo>
{
    public void Configure(EntityTypeBuilder<ProgramPlo> builder)
    {
        builder.ToTable("program_plo", "academic", table =>
        {
            table.HasCheckConstraint("ck_program_plo_code", "code = upper(btrim(code)) AND char_length(code) > 0");
            table.HasCheckConstraint("ck_program_plo_source_lock", "source_template_plo_id IS NULL OR is_locked");
            table.HasCheckConstraint("ck_program_plo_sort_order", "sort_order >= 0");
        });
        builder.HasKey(x => x.Id).HasName("pk_program_plo");
        builder.HasAlternateKey(x => new { x.Id, x.ProgramVersionId }).HasName("uq_program_plo_id_version");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.ProgramVersionId).HasColumnName("program_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Code).HasColumnName("code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Description).HasColumnName("description").HasColumnType("text").IsRequired();
        builder.Property(x => x.Domain).HasColumnName("domain").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.BloomLevel).HasColumnName("bloom_level").HasColumnType("varchar(32)").HasMaxLength(32).IsRequired(false);
        builder.Property(x => x.SourceTemplatePloId).HasColumnName("source_template_plo_id").HasColumnType("uuid").IsRequired(false);
        builder.Property(x => x.IsLocked).HasColumnName("is_locked").HasColumnType("boolean").IsRequired();
        builder.Property(x => x.SortOrder).HasColumnName("sort_order").HasColumnType("integer").IsRequired();
        builder.HasIndex(x => new { x.ProgramVersionId, x.Code }).IsUnique().HasDatabaseName("uq_program_plo_version_code");
        builder.HasOne(x => x.ProgramVersion).WithMany().HasForeignKey(x => x.ProgramVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_program_plo_version");
        builder.HasOne(x => x.SourceTemplatePlo).WithMany().HasForeignKey(x => x.SourceTemplatePloId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_program_plo_source_template");
    }
}
