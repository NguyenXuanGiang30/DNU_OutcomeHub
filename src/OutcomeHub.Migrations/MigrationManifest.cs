using System.Text.Json.Serialization;

namespace OutcomeHub.Migrations;

public sealed class MigrationManifest
{
    [JsonPropertyName("format_version")]
    public int FormatVersion { get; init; }

    [JsonPropertyName("product")]
    public required string Product { get; init; }

    [JsonPropertyName("postgres_major")]
    public int PostgresMajor { get; init; }

    [JsonPropertyName("minimum_runner_version")]
    public required string MinimumRunnerVersion { get; init; }

    [JsonPropertyName("advisory_lock_key")]
    public long AdvisoryLockKey { get; init; }

    [JsonPropertyName("migrations")]
    public required IReadOnlyList<MigrationDefinition> Migrations { get; init; }
}
