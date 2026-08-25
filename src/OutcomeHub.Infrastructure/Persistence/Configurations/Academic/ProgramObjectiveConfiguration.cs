using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Academic;

public sealed class ProgramObjectiveConfiguration : IEntityTypeConfiguration<ProgramObjective>
{
    public void Configure(EntityTypeBuilder<ProgramObjective> builder)
    {
        builder.ToTable("program_objective", "academic", table =>
        {
            table.HasCheckConstraint("ck_program_objective_code", "code = upper(btrim(code)) AND char_length(code) > 0");
            table.HasCheckConstraint("ck_program_objective_sort_order", "sort_order >= 0");
        });
        builder.HasKey(x => x.Id).HasName("pk_program_objective");
        builder.HasAlternateKey(x => new { x.Id, x.ProgramVersionId }).HasName("uq_program_objective_id_version");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.ProgramVersionId).HasColumnName("program_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Code).HasColumnName("code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Description).HasColumnName("description").HasColumnType("text").IsRequired();
        builder.Property(x => x.SortOrder).HasColumnName("sort_order").HasColumnType("integer").IsRequired();
        builder.HasIndex(x => new { x.ProgramVersionId, x.Code }).IsUnique().HasDatabaseName("uq_program_objective_version_code");
        builder.HasOne(x => x.ProgramVersion).WithMany().HasForeignKey(x => x.ProgramVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_program_objective_version");
    }
}
