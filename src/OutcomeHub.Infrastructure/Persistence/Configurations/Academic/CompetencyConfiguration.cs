using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Academic;

public sealed class CompetencyConfiguration : IEntityTypeConfiguration<Competency>
{
    public void Configure(EntityTypeBuilder<Competency> builder)
    {
        builder.ToTable("competency", "academic", table =>
        {
            table.HasCheckConstraint("ck_competency_code", "code = upper(btrim(code)) AND char_length(code) > 0");
            table.HasCheckConstraint("ck_competency_level", "level_no BETWEEN 1 AND 3");
            table.HasCheckConstraint("ck_competency_sort_order", "sort_order >= 0");
        });
        builder.HasKey(x => x.Id).HasName("pk_competency");
        builder.HasAlternateKey(x => new { x.Id, x.ProgramVersionId }).HasName("uq_competency_id_version");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.ProgramVersionId).HasColumnName("program_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.ParentId).HasColumnName("parent_id").HasColumnType("uuid").IsRequired(false);
        builder.Property(x => x.LevelNo).HasColumnName("level_no").HasColumnType("integer").IsRequired();
        builder.Property(x => x.Code).HasColumnName("code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Description).HasColumnName("description").HasColumnType("text").IsRequired();
        builder.Property(x => x.SortOrder).HasColumnName("sort_order").HasColumnType("integer").IsRequired();
        builder.HasIndex(x => new { x.ProgramVersionId, x.Code }).IsUnique().HasDatabaseName("uq_competency_version_code");
        builder.HasOne(x => x.ProgramVersion).WithMany().HasForeignKey(x => x.ProgramVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_competency_version");
        builder.HasOne(x => x.Parent).WithMany().HasForeignKey(x => new { x.ParentId, x.ProgramVersionId }).HasPrincipalKey(x => new { x.Id, x.ProgramVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_competency_parent_version");
    }
}
