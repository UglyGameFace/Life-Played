using System.Data;
using Npgsql;
using Testcontainers.PostgreSql;

namespace LifePlayed.Integration.Tests;

public sealed class PostgreSqlInfrastructureTests
{
    [Fact]
    public async Task PostgreSqlContainerAcceptsRealConnections()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var postgres = new PostgreSqlBuilder("postgres:17-alpine").Build();
        await postgres.StartAsync(cancellationToken);

        await using var connection = new NpgsqlConnection(postgres.GetConnectionString());
        await connection.OpenAsync(cancellationToken);

        await using var command = new NpgsqlCommand("SELECT 1", connection);
        var result = await command.ExecuteScalarAsync(cancellationToken);
        var value = Assert.IsType<int>(result);

        Assert.Equal(ConnectionState.Open, connection.State);
        Assert.Equal(1, value);
    }
}
