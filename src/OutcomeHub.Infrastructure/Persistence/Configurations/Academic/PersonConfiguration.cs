using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Academic;

public sealed class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.ToTable("person", "academic", table =>
        {
            table.HasCheckConstraint("ck_person_source_identity", "(source_system_id IS NULL) = (source_person_id IS NULL)");
            table.HasCheckConstraint("ck_person_source_person_id", "source_person_id IS NULL OR (source_person_id = btrim(source_person_id) AND char_length(source_person_id) > 0)");
            table.HasCheckConstraint("ck_person_status", "status IN ('ACTIVE','INACTIVE','SUSPENDED','EXPIRED')");
            table.HasCheckConstraint("ck_person_effective_range", "effective_to IS NULL OR effective_to > effective_from");
            table.HasCheckConstraint("ck_person_contact_hash", "contact_lookup_hash IS NULL OR contact_lookup_hash ~ '^[0-9a-f]{64}$'");
        });
        builder.HasKey(x => x.Id).HasName("pk_person");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.SourceSystemId).HasColumnName("source_system_id").HasColumnType("uuid").IsRequired(false);
        builder.Property(x => x.SourcePersonId).HasColumnName("source_person_id").HasColumnType("varchar(128)").HasMaxLength(128).IsRequired(false);
        builder.Property(x => x.FullName).HasColumnName("full_name").HasColumnType("varchar(255)").HasMaxLength(255).IsRequired();
        builder.Property(x => x.ContactCiphertext).HasColumnName("contact_ciphertext").HasColumnType("bytea").IsRequired(false);
        builder.Property(x => x.ContactLookupHash).HasColumnName("contact_lookup_hash").HasColumnType("char(64)").HasMaxLength(64).IsFixedLength().IsRequired(false);
        builder.Property(x => x.Status).HasColumnName("status").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.Property(x => x.EffectiveFrom).HasColumnName("effective_from").HasColumnType("date").IsRequired();
        builder.Property(x => x.EffectiveTo).HasColumnName("effective_to").HasColumnType("date").IsRequired(false);
        builder.HasIndex(x => new { x.SourceSystemId, x.SourcePersonId }).IsUnique().AreNullsDistinct(false).HasFilter("source_system_id IS NOT NULL AND source_person_id IS NOT NULL").HasDatabaseName("uq_person_source_identity");
        builder.HasIndex(x => x.ContactLookupHash).HasDatabaseName("ix_person_contact_lookup_hash");
        builder.HasOne(x => x.SourceSystem).WithMany().HasForeignKey(x => x.SourceSystemId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_person_source_system");
    }
}
