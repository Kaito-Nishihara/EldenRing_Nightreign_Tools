using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using NightreignTools.Relic.Domain;
using NightreignTools.Relic.Domain.Entities;
using NightreignTools.ServiceDefaults.Persistence;

namespace NightreignTools.Relic.Infrastructure.Persistence;

/// <summary>
/// relic-service のデータベース(nightreign_relic)の入口(データベースの共通仕様 5.1)。
/// リポジトリの実装からだけ使う。
/// </summary>
public sealed class RelicDbContext(DbContextOptions<RelicDbContext> options) : DbContext(options)
{
    public DbSet<RelicEffectCategory> RelicEffectCategories => Set<RelicEffectCategory>();
    public DbSet<RelicEffect> RelicEffects => Set<RelicEffect>();
    public DbSet<RelicEffectRank> RelicEffectRanks => Set<RelicEffectRank>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(DatabaseServiceCollectionExtensions.Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RelicDbContext).Assembly);

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

        // エンティティの <summary> を、テーブルと列のコメントにする(データベースの共通仕様 5.8)
        modelBuilder.ApplyDocumentationComments(typeof(DomainAssembly).Assembly);
    }
}
