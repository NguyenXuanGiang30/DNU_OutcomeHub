namespace OutcomeHub.Migrations;

internal sealed record MigrationLedgerEntry(
    Guid Id,
    string MigrationName,
    string Checksum,
    string TransactionMode,
    string Status);
