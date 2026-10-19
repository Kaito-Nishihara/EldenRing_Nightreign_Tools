using Microsoft.EntityFrameworkCore;
using NightreignTools.Record.Application.Patterns;
using NightreignTools.Record.Domain.Entities;
using NightreignTools.Record.Infrastructure.Persistence;

namespace NightreignTools.Record.Infrastructure.Repositories;

/// <summary>
/// パターン検索のための記録の読み出し(パターン検索 5 章)。
/// 読むだけで、書かない。
/// </summary>
/// <param name="db">record-service のデータベース。</param>
internal sealed class PatternRecordReader(RecordDbContext db) : IPatternRecordReader
{
    public async Task<IReadOnlyList<RecordedShrinks>> FindAsync(string bossId, int spawnPointId, string? jihenId, CancellationToken ct)
    {
        // jihenId が null のとき、EF Core は jihen_id IS NULL に直す。地変なしの記録だけを読む(AC-PAT-04)
        var rows = await db.PlayRecords
            .AsNoTracking()
            .Where(r => r.BossId == bossId && r.SpawnPointId == spawnPointId && r.JihenId == jihenId)
            .Select(r => new
            {
                r.CreatedAt,
                Shrinks = r.Entries.Select(e => new { e.Day, e.Phase, e.ZoneId, e.SecondShrinkPointId }).ToList(),
            })
            .ToListAsync(ct);

        return rows
            .Select(r => new RecordedShrinks(
                r.CreatedAt,
                r.Shrinks.Select(e => new ShrinkTarget(e.Day, e.Phase, e.ZoneId, e.SecondShrinkPointId)).ToList()))
            .ToList();
    }
}
