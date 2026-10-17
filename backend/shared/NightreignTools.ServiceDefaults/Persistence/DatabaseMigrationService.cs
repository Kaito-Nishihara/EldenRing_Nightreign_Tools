using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace NightreignTools.ServiceDefaults.Persistence;

/// <summary>
/// 起動のとき、要求を受け付ける前に、自分のデータベースへ未適用のマイグレーションを適用する(データベースの共通仕様 5.5)。
/// 適用に失敗したら例外を投げ、起動を止める。文言は RunServiceAsync が起動の失敗としてログに出す。
/// </summary>
public sealed class DatabaseMigrationService<TContext>(
    IServiceScopeFactory scopeFactory,
    ILogger<DatabaseMigrationService<TContext>> logger) : IHostedService
    where TContext : DbContext
{
    /// <summary>データベースがない、利用者がない、合言葉が違う、作る権限がない、のときの PostgreSQL の SQLSTATE。</summary>
    private static readonly HashSet<string> MissingDatabaseOrUserStates =
    [
        PostgresErrorCodes.InvalidCatalogName,
        PostgresErrorCodes.InvalidPassword,
        PostgresErrorCodes.InvalidAuthorizationSpecification,
        PostgresErrorCodes.InsufficientPrivilege,
    ];

    internal const string MissingDatabaseHint =
        "サービスごとのデータベースと利用者は、db のボリュームが空のときにだけ db/init/ の手順で作られます。" +
        "旧版のボリュームが残っているときは、docker compose down の後に " +
        "docker volume rm eldenring_nightreign_tools_postgres_data でボリュームを消してから起動してください" +
        "(データベースの共通仕様 5.5)";

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<TContext>();

        try
        {
            var pending = (await db.Database.GetPendingMigrationsAsync(cancellationToken)).ToList();
            if (pending.Count == 0)
            {
                logger.LogInformation("適用するマイグレーションはありません");
                return;
            }

            logger.LogInformation("マイグレーションを適用します: {Migrations}", string.Join(", ", pending));
            await db.Database.MigrateAsync(cancellationToken);
        }
        // 接続の誤りの文言だけでは、旧版のボリュームが残っていることに気付きにくいので、手順を添える
        catch (PostgresException ex) when (MissingDatabaseOrUserStates.Contains(ex.SqlState))
        {
            throw new InvalidOperationException($"データベースにつなげません({ex.SqlState}: {ex.MessageText})。{MissingDatabaseHint}", ex);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
