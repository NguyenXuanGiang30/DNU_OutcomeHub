using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Portfolio;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Portfolio;

public sealed class CloConfiguration : IEntityTypeConfiguration<Clo>
{
    public void Configure(EntityTypeBuilder<Clo> builder)
    {
        builder.ToTable("clo", "portfolio", table =>
            {
                table.HasCheckConstraint("ck_clo_code", "code = upper(btrim(code)) AND char_length(code) > 0");
                table.HasCheckConstraint("ck_clo_sort_order", "sort_order >= 0");
            });
        builder.HasKey(x => x.Id).HasName("pk_clo");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.SyllabusVersionId).HasColumnName("syllabus_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Code).HasColumnName("code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Description).HasColumnName("description").HasColumnType("text").IsRequired();
        builder.Property(x => x.Domain).HasColumnName("domain").HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();
        builder.Property(x => x.BloomLevel).HasColumnName("bloom_level").HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();
        builder.Property(x => x.IsCore).HasColumnName("is_core").HasColumnType("boolean").IsRequired();
        builder.Property(x => x.SortOrder).HasColumnName("sort_order").HasColumnType("integer").IsRequired();

        builder.HasAlternateKey(x => new { x.Id, x.SyllabusVersionId }).HasName("uq_clo_id_version");
        builder.HasIndex(x => new { x.SyllabusVersionId, x.Code }).IsUnique().HasDatabaseName("uq_clo_version_code");
        builder.HasOne(x => x.SyllabusVersion).WithMany().HasForeignKey(x => x.SyllabusVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_clo_syllabus_version");
    }
}

