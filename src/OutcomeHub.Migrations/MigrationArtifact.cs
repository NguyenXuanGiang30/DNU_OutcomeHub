namespace OutcomeHub.Migrations;

internal sealed record MigrationArtifact(
    MigrationDefinition Definition,
    Guid Id,
    string ScriptSql,
    string PreconditionSql,
    string PostconditionSql);
