using LifePlayed.Application.Persistence;
using LifePlayed.Application.Sync;
using LifePlayed.Contracts.Sync;
using LifePlayed.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("LifePlayed");

if (!string.IsNullOrWhiteSpace(connectionString))
{
    var persistence = new PostgreSqlLifeOsPersistence(connectionString);
    builder.Services.AddSingleton<ILifeOsUnitOfWorkFactory>(persistence);
    builder.Services.AddSingleton<ISyncChangeReader>(persistence);
    builder.Services.AddSingleton<SyncService>();
}

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

if (string.IsNullOrWhiteSpace(connectionString))
{
    app.MapPost(
        "/sync",
        () => Results.Problem(
            title: "Sync unavailable",
            detail: "The Life Played database connection is not configured.",
            statusCode: StatusCodes.Status503ServiceUnavailable));
}
else
{
    app.MapPost(
        "/sync",
        async (
            SyncBatchRequest request,
            SyncService syncService,
            CancellationToken cancellationToken) =>
            Results.Ok(await syncService.SyncAsync(request, cancellationToken)));
}

app.Run();

public partial class Program
{
}
