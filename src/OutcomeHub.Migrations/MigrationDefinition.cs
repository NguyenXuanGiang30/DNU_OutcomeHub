using System.Text.Json.Serialization;

namespace OutcomeHub.Migrations;

public sealed class MigrationDefinition
{
    [JsonPropertyName("sequence")]
    public int Sequence { get; init; }

    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("migration_name")]
    public required string MigrationName { get; init; }

    [JsonPropertyName("transaction_mode")]
    public required string TransactionMode { get; init; }

    [JsonPropertyName("script")]
    public required string Script { get; init; }

    [JsonPropertyName("precondition")]
    public required string Precondition { get; init; }

    [JsonPropertyName("postcondition")]
    public required string Postcondition { get; init; }

    [JsonPropertyName("script_checksum")]
    public required string ScriptChecksum { get; init; }

    [JsonPropertyName("precondition_checksum")]
    public required string PreconditionChecksum { get; init; }

    [JsonPropertyName("postcondition_checksum")]
    public required string PostconditionChecksum { get; init; }

    [JsonPropertyName("timeout_seconds")]
    public int TimeoutSeconds { get; init; }

    [JsonPropertyName("retry_policy")]
    public required string RetryPolicy { get; init; }
}
