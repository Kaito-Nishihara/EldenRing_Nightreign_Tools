using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using NightreignTools.ServiceDefaults.Persistence;

namespace NightreignTools.Relic.Infrastructure.Persistence;

/// <summary>
/// dotnet ef がマイグレーションを作るときに使う。マイグレーションを作るだけなら接続しないので、仮の接続文字列でよい。
/// </summary>
internal sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<RelicDbContext>
{
    public RelicDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<RelicDbContext>();
        DatabaseServiceCollectionExtensions.UseServiceDatabase(options, "Host=localhost;Database=nightreign_relic_design");
        return new RelicDbContext(options.Options);
    }
}
