using NightreignTools.Record.Domain.Entities;

namespace NightreignTools.Record.Application.Patterns;

/// <summary>パターン検索のために、条件に合う記録を読むポート。実装は Infrastructure の EF Core のリポジトリ。</summary>
public interface IPatternRecordReader
{
    /// <summary>夜の王、開始地点、地変が一致する記録を読む。<paramref name="jihenId"/> が null なら、地変なしの記録だけを読む。</summary>
    Task<IReadOnlyList<RecordedShrinks>> FindAsync(string bossId, int spawnPointId, string? jihenId, CancellationToken ct);
}

/// <summary>1 件の記録の、保存した日時と収縮先。</summary>
public sealed record RecordedShrinks(DateTime CreatedAt, IReadOnlyList<ShrinkTarget> Shrinks);
