using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using NightreignTools.ServiceDefaults.Persistence;

namespace NightreignTools.Record.Infrastructure.Persistence;

/// <summary>
/// dotnet ef がマイグレーションを作るときに使う。マイグレーションを作るだけなら接続しないので、仮の接続文字列でよい。
/// </summary>
internal sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<RecordDbContext>
{
    public RecordDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<RecordDbContext>();
        DatabaseServiceCollectionExtensions.UseServiceDatabase(options, "Host=localhost;Database=nightreign_record_design");
        return new RecordDbContext(options.Options);
    }
}
