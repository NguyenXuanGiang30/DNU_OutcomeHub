using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OutcomeHub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPostgreSqlIntegrityFoundation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_outbox_message_claim",
                schema: "integration",
                table: "outbox_message");

            migrationBuilder.DropIndex(
                name: "ix_operation_job_claim",
                schema: "ops",
                table: "operation_job");

            migrationBuilder.DropIndex(
                name: "ix_audit_event_actor_occurred_at",
                schema: "audit",
                table: "audit_event");

            migrationBuilder.DropIndex(
                name: "ix_audit_event_chain_sequence",
                schema: "audit",
                table: "audit_event");

            migrationBuilder.DropIndex(
                name: "ix_audit_event_event_hash",
                schema: "audit",
                table: "audit_event");

            migrationBuilder.DropIndex(
                name: "ix_audit_event_occurred_at",
                schema: "audit",
                table: "audit_event");

            migrationBuilder.DropIndex(
                name: "ix_audit_event_program_version_occurred_at",
                schema: "audit",
                table: "audit_event");

            migrationBuilder.DropIndex(
                name: "ix_audit_event_resource_occurred_at",
                schema: "audit",
                table: "audit_event");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:btree_gist", ",,")
                .Annotation("Npgsql:PostgresExtension:citext", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:citext", ",,");

            migrationBuilder.Sql(
                """
                ALTER TABLE iam.database_principal_binding
                    ADD CONSTRAINT ex_database_principal_binding_active_range
                    EXCLUDE USING gist (
                        database_role_name WITH =,
                        daterange(effective_from, COALESCE(effective_to, 'infinity'::date), '[)') WITH &&
                    ) WHERE (status = 'ACTIVE');

                ALTER TABLE iam.role_assignment
                    ADD CONSTRAINT ex_role_assignment_active_range
                    EXCLUDE USING gist (
                        principal_id WITH =,
                        role_id WITH =,
                        access_scope_id WITH =,
                        tstzrange(effective_from, effective_to, '[)') WITH &&
                    ) WHERE (status = 'ACTIVE');

                ALTER TABLE academic.program_version_cohort
                    ADD CONSTRAINT ex_program_version_cohort_default_range
                    EXCLUDE USING gist (
                        cohort_id WITH =,
                        daterange(effective_from, COALESCE(effective_to, 'infinity'::date), '[)') WITH &&
                    ) WHERE (is_default);

                ALTER TABLE academic.direct_measurement_plan
                    ADD CONSTRAINT ex_direct_measurement_plan_active_range
                    EXCLUDE USING gist (
                        program_version_id WITH =,
                        curriculum_path_id WITH =,
                        program_pi_id WITH =,
                        daterange(effective_from, COALESCE(effective_to, 'infinity'::date), '[)') WITH &&
                    ) WHERE (status = 'ACTIVE');

                ALTER TABLE academic.student_path
                    ADD CONSTRAINT ex_student_path_primary_overlap
                    EXCLUDE USING gist (
                        student_id WITH =,
                        program_id WITH =,
                        daterange(effective_from, COALESCE(effective_to, 'infinity'::date), '[)') WITH &&
                    ) WHERE (is_primary AND path_status = 'ACTIVE');

                ALTER TABLE portfolio.syllabus_template_rubric_scale_level
                    ADD CONSTRAINT ex_syllabus_template_rubric_scale_level_range
                    EXCLUDE USING gist (
                        rubric_scale_id WITH =,
                        numrange(score_from, score_to, '[)') WITH &&
                    );

                ALTER TABLE portfolio.rubric_level
                    ADD CONSTRAINT ex_rubric_level_range
                    EXCLUDE USING gist (
                        rubric_criterion_id WITH =,
                        score_range WITH &&
                    );

                ALTER TABLE measurement.enrollment_revision
                    ADD CONSTRAINT ex_enrollment_revision_effective_range
                    EXCLUDE USING gist (
                        enrollment_id WITH =,
                        tstzrange(effective_from, effective_to, '[)') WITH &&
                    );

                ALTER TABLE measurement.program_policy_binding
                    ADD CONSTRAINT ex_program_policy_binding_active_range
                    EXCLUDE USING gist (
                        program_version_id WITH =,
                        daterange(effective_from, COALESCE(effective_to, 'infinity'::date), '[)') WITH &&
                    ) WHERE (status = 'ACTIVE');

                CREATE FUNCTION audit.reject_mutation()
                RETURNS trigger
                LANGUAGE plpgsql
                SET search_path = pg_catalog, audit
                AS $function$
                BEGIN
                    RAISE EXCEPTION 'audit events are immutable' USING ERRCODE = '55000';
                END;
                $function$;

                CREATE TRIGGER trg_audit_event_immutable
                BEFORE UPDATE OR DELETE ON audit.audit_event
                FOR EACH ROW EXECUTE FUNCTION audit.reject_mutation();
                """);

            migrationBuilder.CreateIndex(
                name: "ix_outbox_message_claim",
                schema: "integration",
                table: "outbox_message",
                columns: new[] { "available_at", "occurred_at" },
                filter: "published_at IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_operation_job_claim",
                schema: "ops",
                table: "operation_job",
                columns: new[] { "queue_name", "status", "available_at", "priority", "created_at" },
                descending: new[] { false, false, false, true, false },
                filter: "status IN ('QUEUED','RETRY_WAIT')");

            migrationBuilder.CreateIndex(
                name: "ix_operation_job_expired_lease",
                schema: "ops",
                table: "operation_job",
                column: "lease_until",
                filter: "status = 'RUNNING'");

            migrationBuilder.CreateIndex(
                name: "ix_audit_event_actor_occurred_at",
                schema: "audit",
                table: "audit_event",
                columns: new[] { "actor_principal_id", "occurred_at" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "ix_audit_event_occurred_at",
                schema: "audit",
                table: "audit_event",
                column: "occurred_at",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "ix_audit_event_program_version_occurred_at",
                schema: "audit",
                table: "audit_event",
                columns: new[] { "program_version_id", "occurred_at" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "ix_audit_event_resource_occurred_at",
                schema: "audit",
                table: "audit_event",
                columns: new[] { "resource_type", "resource_id", "occurred_at" },
                descending: new[] { false, false, true });

            migrationBuilder.CreateIndex(
                name: "uq_audit_event_chain_sequence",
                schema: "audit",
                table: "audit_event",
                columns: new[] { "chain_id", "chain_sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_audit_event_event_hash",
                schema: "audit",
                table: "audit_event",
                column: "event_hash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_outbox_message_claim",
                schema: "integration",
                table: "outbox_message");

            migrationBuilder.DropIndex(
                name: "ix_operation_job_claim",
                schema: "ops",
                table: "operation_job");

            migrationBuilder.DropIndex(
                name: "ix_operation_job_expired_lease",
                schema: "ops",
                table: "operation_job");

            migrationBuilder.DropIndex(
                name: "ix_audit_event_actor_occurred_at",
                schema: "audit",
                table: "audit_event");

            migrationBuilder.DropIndex(
                name: "ix_audit_event_occurred_at",
                schema: "audit",
                table: "audit_event");

            migrationBuilder.DropIndex(
                name: "ix_audit_event_program_version_occurred_at",
                schema: "audit",
                table: "audit_event");

            migrationBuilder.DropIndex(
                name: "ix_audit_event_resource_occurred_at",
                schema: "audit",
                table: "audit_event");

            migrationBuilder.DropIndex(
                name: "uq_audit_event_chain_sequence",
                schema: "audit",
                table: "audit_event");

            migrationBuilder.DropIndex(
                name: "uq_audit_event_event_hash",
                schema: "audit",
                table: "audit_event");

            migrationBuilder.Sql(
                """
                DROP TRIGGER trg_audit_event_immutable ON audit.audit_event;
                DROP FUNCTION audit.reject_mutation();

                ALTER TABLE measurement.program_policy_binding
                    DROP CONSTRAINT ex_program_policy_binding_active_range;
                ALTER TABLE measurement.enrollment_revision
                    DROP CONSTRAINT ex_enrollment_revision_effective_range;
                ALTER TABLE portfolio.rubric_level
                    DROP CONSTRAINT ex_rubric_level_range;
                ALTER TABLE portfolio.syllabus_template_rubric_scale_level
                    DROP CONSTRAINT ex_syllabus_template_rubric_scale_level_range;
                ALTER TABLE academic.student_path
                    DROP CONSTRAINT ex_student_path_primary_overlap;
                ALTER TABLE academic.direct_measurement_plan
                    DROP CONSTRAINT ex_direct_measurement_plan_active_range;
                ALTER TABLE academic.program_version_cohort
                    DROP CONSTRAINT ex_program_version_cohort_default_range;
                ALTER TABLE iam.role_assignment
                    DROP CONSTRAINT ex_role_assignment_active_range;
                ALTER TABLE iam.database_principal_binding
                    DROP CONSTRAINT ex_database_principal_binding_active_range;
                """);

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:citext", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:btree_gist", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:citext", ",,");

            migrationBuilder.CreateIndex(
                name: "ix_outbox_message_claim",
                schema: "integration",
                table: "outbox_message",
                columns: new[] { "status", "available_at", "locked_until" });

            migrationBuilder.CreateIndex(
                name: "ix_operation_job_claim",
                schema: "ops",
                table: "operation_job",
                columns: new[] { "status", "queue_name", "available_at", "priority" });

            migrationBuilder.CreateIndex(
                name: "ix_audit_event_actor_occurred_at",
                schema: "audit",
                table: "audit_event",
                columns: new[] { "actor_principal_id", "occurred_at" });

            migrationBuilder.CreateIndex(
                name: "ix_audit_event_chain_sequence",
                schema: "audit",
                table: "audit_event",
                columns: new[] { "chain_id", "chain_sequence" });

            migrationBuilder.CreateIndex(
                name: "ix_audit_event_event_hash",
                schema: "audit",
                table: "audit_event",
                column: "event_hash");

            migrationBuilder.CreateIndex(
                name: "ix_audit_event_occurred_at",
                schema: "audit",
                table: "audit_event",
                column: "occurred_at");

            migrationBuilder.CreateIndex(
                name: "ix_audit_event_program_version_occurred_at",
                schema: "audit",
                table: "audit_event",
                columns: new[] { "program_version_id", "occurred_at" });

            migrationBuilder.CreateIndex(
                name: "ix_audit_event_resource_occurred_at",
                schema: "audit",
                table: "audit_event",
                columns: new[] { "resource_type", "resource_id", "occurred_at" });
        }
    }
}
