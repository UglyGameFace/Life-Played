using System.Reflection;
using Npgsql;

namespace LifePlayed.Infrastructure.Persistence;

public sealed class PostgreSqlMigrator
{
    private const string MigrationMarker = ".Persistence.Migrations.";
    private readonly string _connectionString;

    public PostgreSqlMigrator(string connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
        _connectionString = connectionString;
    }

    public async Task ApplyAsync(CancellationToken cancellationToken = default)
    {
        await using var dataSource = NpgsqlDataSource.Create(_connectionString);
        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);

        await EnsureMigrationTableAsync(connection, cancellationToken);

        var assembly = typeof(PostgreSqlMigrator).Assembly;
        var resources = assembly.GetManifestResourceNames()
            .Where(static name => name.Contains(MigrationMarker, StringComparison.Ordinal))
            .Order(StringComparer.Ordinal)
            .ToArray();

        foreach (var resourceName in resources)
        {
            await ApplyMigrationAsync(connection, assembly, resourceName, cancellationToken);
        }
    }

    private static async Task EnsureMigrationTableAsync(
        NpgsqlConnection connection,
        CancellationToken cancellationToken)
    {
        const string sql = """
            CREATE SCHEMA IF NOT EXISTS infrastructure;

            CREATE TABLE IF NOT EXISTS infrastructure.schema_migrations (
                version text PRIMARY KEY,
                applied_at timestamptz NOT NULL DEFAULT now()
            );
            """;

        await using var command = new NpgsqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static async Task ApplyMigrationAsync(
        NpgsqlConnection connection,
        Assembly assembly,
        string resourceName,
        CancellationToken cancellationToken)
    {
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

        if (await HasAppliedAsync(connection, transaction, resourceName, cancellationToken))
        {
            await transaction.CommitAsync(cancellationToken);
            return;
        }

        var sql = await ReadMigrationAsync(assembly, resourceName, cancellationToken);
        await using (var command = new NpgsqlCommand(sql, connection, transaction))
        {
            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        const string recordSql = """
            INSERT INTO infrastructure.schema_migrations (version)
            VALUES (@version);
            """;

        await using (var recordCommand = new NpgsqlCommand(recordSql, connection, transaction))
        {
            recordCommand.Parameters.AddWithValue("version", resourceName);
            await recordCommand.ExecuteNonQueryAsync(cancellationToken);
        }

        await transaction.CommitAsync(cancellationToken);
    }

    private static async Task<bool> HasAppliedAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction transaction,
        string resourceName,
        CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT EXISTS (
                SELECT 1
                FROM infrastructure.schema_migrations
                WHERE version = @version
            );
            """;

        await using var command = new NpgsqlCommand(sql, connection, transaction);
        command.Parameters.AddWithValue("version", resourceName);
        var result = await command.ExecuteScalarAsync(cancellationToken);

        return result is true;
    }

    private static async Task<string> ReadMigrationAsync(
        Assembly assembly,
        string resourceName,
        CancellationToken cancellationToken)
    {
        await using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Missing migration resource '{resourceName}'.");

        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync(cancellationToken);
    }
}
