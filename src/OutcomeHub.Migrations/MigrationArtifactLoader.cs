using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace OutcomeHub.Migrations;

internal static partial class MigrationArtifactLoader
{
    private static readonly UTF8Encoding StrictUtf8 = new(
        encoderShouldEmitUTF8Identifier: false,
        throwOnInvalidBytes: true);

    public static async Task<(MigrationManifest Manifest, IReadOnlyList<MigrationArtifact> Artifacts)>
        LoadAsync(string migrationRoot, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(migrationRoot);

        string root = Path.GetFullPath(migrationRoot);
        byte[] manifestBytes = await File.ReadAllBytesAsync(
            Path.Combine(root, "manifest.json"),
            cancellationToken);
        EnsureCanonicalText(manifestBytes, "manifest.json");

        MigrationManifest manifest = JsonSerializer.Deserialize<MigrationManifest>(manifestBytes)
            ?? throw new InvalidOperationException("Migration manifest is empty.");

        ValidateManifest(manifest);

        var artifacts = new List<MigrationArtifact>(manifest.Migrations.Count);
        foreach (MigrationDefinition definition in manifest.Migrations.OrderBy(x => x.Sequence))
        {
            byte[] scriptBytes = await ReadArtifactAsync(root, definition.Script, cancellationToken);
            byte[] preconditionBytes = await ReadArtifactAsync(
                root,
                definition.Precondition,
                cancellationToken);
            byte[] postconditionBytes = await ReadArtifactAsync(
                root,
                definition.Postcondition,
                cancellationToken);

            VerifyChecksum(scriptBytes, definition.ScriptChecksum, definition.Script);
            VerifyChecksum(
                preconditionBytes,
                definition.PreconditionChecksum,
                definition.Precondition);
            VerifyChecksum(
                postconditionBytes,
                definition.PostconditionChecksum,
                definition.Postcondition);

            string scriptSql = StrictUtf8.GetString(scriptBytes);
            if (PsqlMetaCommandRegex().IsMatch(scriptSql))
            {
                throw new InvalidOperationException(
                    $"Migration '{definition.MigrationName}' contains a psql meta-command.");
            }

            artifacts.Add(new MigrationArtifact(
                definition,
                Guid.ParseExact(definition.Id, "D"),
                scriptSql,
                StrictUtf8.GetString(preconditionBytes),
                StrictUtf8.GetString(postconditionBytes)));
        }

        return (manifest, artifacts);
    }

    private static async Task<byte[]> ReadArtifactAsync(
        string root,
        string relativePath,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(relativePath) || Path.IsPathRooted(relativePath))
        {
            throw new InvalidOperationException("Migration artifact paths must be relative.");
        }

        string path = Path.GetFullPath(Path.Combine(root, relativePath));
        string rootPrefix = root.EndsWith(Path.DirectorySeparatorChar)
            ? root
            : root + Path.DirectorySeparatorChar;

        if (!path.StartsWith(rootPrefix, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"Migration artifact path escapes its root: '{relativePath}'.");
        }

        byte[] bytes = await File.ReadAllBytesAsync(path, cancellationToken);
        EnsureCanonicalText(bytes, relativePath);
        return bytes;
    }

    private static void EnsureCanonicalText(byte[] bytes, string relativePath)
    {
        if (bytes.AsSpan().StartsWith(Encoding.UTF8.Preamble))
        {
            throw new InvalidOperationException(
                $"Migration artifact '{relativePath}' must not contain a UTF-8 BOM.");
        }

        if (bytes.AsSpan().Contains((byte)'\r'))
        {
            throw new InvalidOperationException(
                $"Migration artifact '{relativePath}' must use LF line endings.");
        }

        _ = StrictUtf8.GetString(bytes);
    }

    private static void VerifyChecksum(byte[] bytes, string expected, string relativePath)
    {
        string actual = Convert.ToHexStringLower(SHA256.HashData(bytes));
        if (!string.Equals(actual, expected, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"Checksum drift detected for migration artifact '{relativePath}'.");
        }
    }

    private static void ValidateManifest(MigrationManifest manifest)
    {
        if (manifest.FormatVersion != 1
            || !string.Equals(manifest.Product, "OutcomeHub", StringComparison.Ordinal)
            || manifest.PostgresMajor != 18
            || manifest.AdvisoryLockKey == 0
            || manifest.Migrations.Count == 0)
        {
            throw new InvalidOperationException("Migration manifest contract is invalid.");
        }

        MigrationDefinition[] ordered = [.. manifest.Migrations.OrderBy(x => x.Sequence)];
        if (ordered.Select(x => x.Sequence).Distinct().Count() != ordered.Length
            || ordered.Select(x => x.Id).Distinct(StringComparer.Ordinal).Count() != ordered.Length
            || ordered.Select(x => x.MigrationName).Distinct(StringComparer.Ordinal).Count()
                != ordered.Length)
        {
            throw new InvalidOperationException("Migration manifest contains duplicate entries.");
        }

        for (int index = 0; index < ordered.Length; index++)
        {
            MigrationDefinition definition = ordered[index];
            if (definition.Sequence != index + 1
                || !MigrationNameRegex().IsMatch(definition.MigrationName)
                || !Guid.TryParseExact(definition.Id, "D", out _)
                || definition.TimeoutSeconds is < 1 or > 7_200
                || definition.ScriptChecksum.Length != 64
                || definition.PreconditionChecksum.Length != 64
                || definition.PostconditionChecksum.Length != 64
                || definition.TransactionMode is not ("TRANSACTIONAL" or "OPERATIONAL")
                || definition.RetryPolicy is not ("ATOMIC" or "IDEMPOTENT")
                || (definition.TransactionMode == "TRANSACTIONAL"
                    && definition.RetryPolicy != "ATOMIC")
                || (definition.TransactionMode == "OPERATIONAL"
                    && definition.RetryPolicy != "IDEMPOTENT"))
            {
                throw new InvalidOperationException(
                    $"Migration manifest entry '{definition.MigrationName}' is invalid.");
            }
        }
    }

    [GeneratedRegex("(?m)^\\s*\\\\")]
    private static partial Regex PsqlMetaCommandRegex();

    [GeneratedRegex("^[0-9]{4}_[a-z0-9_]+$")]
    private static partial Regex MigrationNameRegex();
}
