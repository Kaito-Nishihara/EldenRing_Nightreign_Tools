using NightreignTools.Record.Domain.Entities;

namespace NightreignTools.Record.Application.Patterns;

/// <summary>
/// 記録をパターンにまとめる(パターン検索 3.2、4 章)。
/// 収縮先の並び(日、段階、収縮候補地か第二収縮地点)がすべて同じ記録を 1 つのパターンにする。
/// 記録した段階の数が違えば、同じ場所でも別のパターンになる(AC-PAT-07)。
/// </summary>
internal static class PatternGrouper
{
    /// <summary>まとめた 1 つのパターン。収縮先は日、段階の順。</summary>
    public sealed record PatternGroup(IReadOnlyList<ShrinkTarget> Shrinks, int RecordCount, DateTime LastRecordedAt);

    /// <summary>まとめて、記録の数の多い順、同じなら最後の記録の新しい順に並べる。</summary>
    public static IReadOnlyList<PatternGroup> Group(IEnumerable<RecordedShrinks> records) =>
        records
            .Select(r => (Shrinks: r.Shrinks.OrderBy(s => s.Day).ThenBy(s => s.Phase).ToList(), r.CreatedAt))
            .GroupBy(r => KeyOf(r.Shrinks))
            .Select(g => new PatternGroup(g.First().Shrinks, g.Count(), g.Max(r => r.CreatedAt)))
            .OrderByDescending(g => g.RecordCount)
            .ThenByDescending(g => g.LastRecordedAt)
            .ToList();

    /// <summary>並びを 1 つの文字列の鍵にする。例: 1-1-z4|1-2-p4</summary>
    private static string KeyOf(IEnumerable<ShrinkTarget> shrinks) =>
        string.Join('|', shrinks.Select(s => s.ZoneId is { } zone ? $"{s.Day}-{s.Phase}-z{zone}" : $"{s.Day}-{s.Phase}-p{s.SecondShrinkPointId}"));
}
