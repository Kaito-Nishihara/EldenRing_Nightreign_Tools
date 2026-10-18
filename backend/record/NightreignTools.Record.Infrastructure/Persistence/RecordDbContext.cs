using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using NightreignTools.Record.Domain;
using NightreignTools.Record.Domain.Entities;
using NightreignTools.ServiceDefaults.Persistence;

namespace NightreignTools.Record.Infrastructure.Persistence;

/// <summary>
/// record-service のデータベース(nightreign_record)の入口(データベースの共通仕様 5.1)。
/// リポジトリの実装からだけ使う。
/// </summary>
public sealed class RecordDbContext(DbContextOptions<RecordDbContext> options) : DbContext(options)
{
    public DbSet<PlayRecord> PlayRecords => Set<PlayRecord>();
    public DbSet<ShrinkEntry> ShrinkEntries => Set<ShrinkEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(DatabaseServiceCollectionExtensions.Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RecordDbContext).Assembly);

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
