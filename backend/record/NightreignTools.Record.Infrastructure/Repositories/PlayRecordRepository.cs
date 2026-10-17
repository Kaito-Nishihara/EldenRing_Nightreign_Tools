using NightreignTools.Record.Application.PlayRecords;
using NightreignTools.Record.Domain.Entities;
using NightreignTools.Record.Infrastructure.Persistence;

namespace NightreignTools.Record.Infrastructure.Repositories;

/// <summary>プレイ記録の保存(プレイ記録 5 章)。</summary>
internal sealed class PlayRecordRepository(RecordDbContext db) : IPlayRecordRepository
{
    public async Task AddAsync(PlayRecord record, CancellationToken ct)
    {
        // 記録と収縮先を 1 回の SaveChanges で入れる。EF Core が 1 つのトランザクションにまとめる
        db.PlayRecords.Add(record);
        await db.SaveChangesAsync(ct);
    }
}
