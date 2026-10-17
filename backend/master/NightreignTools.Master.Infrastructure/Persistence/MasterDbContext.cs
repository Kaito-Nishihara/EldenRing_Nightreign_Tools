using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using NightreignTools.Master.Domain.Entities;
using NightreignTools.ServiceDefaults.Persistence;

namespace NightreignTools.Master.Infrastructure.Persistence;

/// <summary>
/// master-service のデータベース(nightreign_master)の入口(データベースの共通仕様 5.1)。
/// リポジトリの実装からだけ使う。
/// </summary>
public sealed class MasterDbContext(DbContextOptions<MasterDbContext> options) : DbContext(options)
{
    public DbSet<Boss> Bosses => Set<Boss>();
    public DbSet<BossWeakness> BossWeaknesses => Set<BossWeakness>();
    public DbSet<Jihen> Jihen => Set<Jihen>();
    public DbSet<SpawnPoint> SpawnPoints => Set<SpawnPoint>();
    public DbSet<ShrinkZone> ShrinkZones => Set<ShrinkZone>();
    public DbSet<SecondShrinkPoint> SecondShrinkPoints => Set<SecondShrinkPoint>();
    public DbSet<BasePoint> BasePoints => Set<BasePoint>();
    public DbSet<NightBoss> NightBosses => Set<NightBoss>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(DatabaseServiceCollectionExtensions.Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MasterDbContext).Assembly);

        // created_at はデータベースの既定値で入れ、アプリから入れない(データベースの共通仕様 5.3)
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var createdAt = entityType.FindProperty("CreatedAt");
            if (createdAt is null)
            {
                continue;
            }

            createdAt.SetDefaultValueSql("now()");
            createdAt.ValueGenerated = ValueGenerated.OnAdd;
        }
    }
}
