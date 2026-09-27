using System.Data;
using Npgsql;
using Testcontainers.PostgreSql;

namespace LifePlayed.Integration.Tests;

public sealed class PostgreSqlInfrastructureTests
{
    [Fact]
    public async Task PostgreSqlContainerAcceptsRealConnections()
    {
        await using var postgres = new PostgreSqlBuilder("postgres:17-alpine").Build();
        await postgres.StartAsync();

        await using var connection = new NpgsqlConnection(postgres.GetConnectionString());
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand("SELECT 1", connection);
        var result = await command.ExecuteScalarAsync();

        Assert.Equal(ConnectionState.Open, connection.State);
        Assert.Equal(1, Convert.ToInt32(result));
    }
}
