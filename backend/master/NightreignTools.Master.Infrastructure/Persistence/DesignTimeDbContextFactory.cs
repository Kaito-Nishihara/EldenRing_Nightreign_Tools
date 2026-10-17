using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using NightreignTools.ServiceDefaults.Persistence;

namespace NightreignTools.Master.Infrastructure.Persistence;

/// <summary>
/// dotnet ef がマイグレーションを作るときに使う。マイグレーションを作るだけなら接続しないので、仮の接続文字列でよい。
/// </summary>
internal sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<MasterDbContext>
{
    public MasterDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<MasterDbContext>();
        DatabaseServiceCollectionExtensions.UseServiceDatabase(options, "Host=localhost;Database=nightreign_master_design");
        return new MasterDbContext(options.Options);
    }
}
