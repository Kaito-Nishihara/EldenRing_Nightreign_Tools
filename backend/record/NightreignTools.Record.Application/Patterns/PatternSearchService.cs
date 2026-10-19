using NightreignTools.Record.Application.Masters;
using NightreignTools.SharedKernel;

namespace NightreignTools.Record.Application.Patterns;

/// <summary>
/// パターン検索(パターン検索 4 章、6 章)。
/// 条件に合う記録を読んでまとめ、出てきた収縮先の名前と座標を master-service に 1 回だけ問い合わせて入れる。
/// 条件の夜の王、開始地点、地変があるかも、同じ問い合わせで確かめる。
/// </summary>
/// <param name="reader">条件に合う記録を読む。</param>
/// <param name="masters">master-service から名前と座標を引く。</param>
internal sealed class PatternSearchService(IPatternRecordReader reader, IMasterDirectory masters) : IPatternSearchService
{
    public async Task<PatternResponse> SearchAsync(PatternSearchRequest request, CancellationToken ct)
    {
        var bossId = request.BossId ?? throw new BusinessRuleException("bossId は必須です");
        var spawnPointId = request.SpawnPointId ?? throw new BusinessRuleException("spawnPointId は必須です");
        var jihenId = string.IsNullOrWhiteSpace(request.JihenId) ? null : request.JihenId;

        var records = await reader.FindAsync(bossId, spawnPointId, jihenId, ct);
        var groups = PatternGrouper.Group(records);
        var shrinks = groups.SelectMany(g => g.Shrinks).ToList();

        var found = await masters.ResolveAsync(new MasterReferenceQuery(
            BossIds: [bossId],
            JihenIds: jihenId is null ? [] : [jihenId],
            SpawnPointIds: [spawnPointId],
            ZoneIds: shrinks.Where(s => s.ZoneId is not null).Select(s => s.ZoneId!.Value).Distinct().ToList(),
            SecondShrinkPointIds: shrinks.Where(s => s.SecondShrinkPointId is not null).Select(s => s.SecondShrinkPointId!.Value).Distinct().ToList(),
            NightBossIds: []), ct);

        EnsureConditions(bossId, spawnPointId, jihenId, found);

        // 非表示の収縮候補地と第二収縮地点も、名前と座標で返す(AC-PAT-21)
        var zones = found.Zones.ToDictionary(z => z.Id);
        var points = found.SecondShrinkPoints.ToDictionary(p => p.Id);
        var patterns = groups
            .Select((group, index) => new PatternDto(
                index + 1,
                group.RecordCount,
                group.LastRecordedAt,
                group.Shrinks.Select(s => new PatternEntryDto(
                    s.Day,
                    s.Phase,
                    s.ZoneId is { } zoneId ? ToDto(Lookup(zones, zoneId, "zoneId")) : null,
                    s.SecondShrinkPointId is { } pointId ? ToDto(Lookup(points, pointId, "secondShrinkPointId")) : null)).ToList()))
            .ToList();

        return new PatternResponse(records.Count, patterns);
    }

    /// <summary>
    /// 条件の行があること。
    /// ない行は 404 にする。
    /// </summary>
    /// <param name="bossId">条件の夜の王の識別子。</param>
    /// <param name="spawnPointId">条件の開始地点の識別子。</param>
    /// <param name="jihenId">条件の地変の識別子。null なら確かめない。</param>
    /// <param name="found">master-service から引いた行。</param>
    private static void EnsureConditions(string bossId, int spawnPointId, string? jihenId, MasterReferences found)
    {
        if (found.Bosses.All(b => b.Id != bossId))
        {
            throw new NotFoundException($"bossId '{bossId}' は存在しません");
        }

        if (found.SpawnPoints.All(p => p.Id != spawnPointId))
        {
            throw new NotFoundException($"spawnPointId '{spawnPointId}' は存在しません");
        }

        if (jihenId is not null && found.Jihen.All(j => j.Id != jihenId))
        {
            throw new NotFoundException($"jihenId '{jihenId}' は存在しません");
        }
    }

    /// <summary>
    /// 記録が指す収縮先を引く。マスタの行は消さないので(決定の記録 0013)、見つからないのはデータの食い違いである。
    /// 利用者には直せないので、想定していない誤り(500)にする。
    /// </summary>
    /// <typeparam name="T">マスタの行の型。</typeparam>
    /// <param name="rows">識別子で引けるようにしたマスタの行。</param>
    /// <param name="id">記録が指す識別子。</param>
    /// <param name="field">例外の文言に入れる項目の名前。</param>
    /// <returns>見つかった行。</returns>
    private static T Lookup<T>(IReadOnlyDictionary<int, T> rows, int id, string field) =>
        rows.TryGetValue(id, out var row)
            ? row
            : throw new InvalidOperationException($"記録が指す {field} '{id}' が master-service にありません");

    private static PatternZoneDto ToDto(MasterZone zone) => new(zone.Id, zone.NameJa, zone.CenterX, zone.CenterY, zone.Radius);

    private static PatternPointDto ToDto(MasterPoint point) => new(point.Id, point.NameJa, point.X, point.Y);
}
