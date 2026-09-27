using System.Data;
using System.Text.Json;
using LifePlayed.Application;
using LifePlayed.Infrastructure.Persistence;
using Npgsql;
using Testcontainers.PostgreSql;

namespace LifePlayed.Integration.Tests;

public sealed class PostgreSqlInfrastructureTests
{
    [Fact]
    public async Task PostgreSqlContainerAcceptsRealConnectionsAndMigrationsAreIdempotent()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var postgres = new PostgreSqlBuilder("postgres:17-alpine").Build();
        await postgres.StartAsync(cancellationToken);

        var migrator = new PostgreSqlMigrator(postgres.GetConnectionString());
        await migrator.ApplyAsync(cancellationToken);
        await migrator.ApplyAsync(cancellationToken);

        await using var connection = new NpgsqlConnection(postgres.GetConnectionString());
        await connection.OpenAsync(cancellationToken);

        Assert.Equal(ConnectionState.Open, connection.State);

        var expectedTables = new[]
        {
            "identity.accounts",
            "life_os.actions",
            "life_os.quests",
            "life_os.campaigns",
            "life_os.campaign_phases",
            "progression.progression_events",
            "progression.reward_grants",
            "sync.client_mutations",
        };

        foreach (var table in expectedTables)
        {
            await using var tableCommand = new NpgsqlCommand("SELECT to_regclass(@table)::text", connection);
            tableCommand.Parameters.AddWithValue("table", table);
            var result = await tableCommand.ExecuteScalarAsync(cancellationToken);
            Assert.NotNull(result);
            Assert.NotEqual(DBNull.Value, result);
        }

        await using var migrationCountCommand = new NpgsqlCommand(
            "SELECT count(*)::integer FROM infrastructure.schema_migrations",
            connection);

        var migrationCount = await migrationCountCommand.ExecuteScalarAsync(cancellationToken);
        Assert.Equal(1, Assert.IsType<int>(migrationCount));
    }

    [Fact]
    public void ApplicationAssemblyHasNoPlatformOrProviderDependencies()
    {
        var referenced = typeof(AssemblyMarker).Assembly
            .GetReferencedAssemblies()
            .Select(static assembly => assembly.Name ?? string.Empty)
            .ToArray();

        Assert.DoesNotContain(referenced, static name => name.StartsWith("Unity", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(referenced, static name => name.StartsWith("Microsoft.AspNetCore", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(referenced, static name => name.Contains("EntityFrameworkCore", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(referenced, static name => name.StartsWith("Npgsql", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ContentReleaseSchemaIsValidAndVersioned()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "content-release.schema.json");
        using var document = JsonDocument.Parse(File.ReadAllText(path));
        var root = document.RootElement;

        Assert.Equal(
            "urn:life-played:schema:content-release:v1",
            root.GetProperty("$id").GetString());

        Assert.Equal(
            1,
            root.GetProperty("properties")
                .GetProperty("schemaVersion")
                .GetProperty("const")
                .GetInt32());
    }
}
