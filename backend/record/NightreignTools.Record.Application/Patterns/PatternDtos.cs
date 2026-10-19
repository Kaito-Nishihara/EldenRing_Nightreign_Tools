using System.ComponentModel.DataAnnotations;

namespace NightreignTools.Record.Application.Patterns;

// パターン検索の入力と出力(パターン検索 4 章)

/// <summary>
/// 検索の条件(クエリ)。
/// 地変を省くと、地変なしの記録だけを探す。
/// </summary>
/// <param name="BossId">夜の王の識別子。必須。50 文字以内。</param>
/// <param name="SpawnPointId">開始地点の識別子。必須。</param>
/// <param name="JihenId">地変の識別子。50 文字以内。省くと地変なし。</param>
public sealed record PatternSearchRequest(
    [property: Required(ErrorMessage = "bossId は必須です")]
    [property: MaxLength(PatternSearchRequest.IdMaxLength, ErrorMessage = "bossId は 50 文字以内です")]
    string? BossId,
    [property: Required(ErrorMessage = "spawnPointId は必須です")]
    int? SpawnPointId,
    [property: MaxLength(PatternSearchRequest.IdMaxLength, ErrorMessage = "jihenId は 50 文字以内です")]
    string? JihenId)
{
    public const int IdMaxLength = 50;
}

/// <summary>
/// 検索の結果。
/// 記録がないときは totalRecords が 0 で、patterns が空。
/// </summary>
/// <param name="TotalRecords">条件に合った記録の数。</param>
/// <param name="Patterns">記録の数の多い順に並べたパターン。</param>
public sealed record PatternResponse(int TotalRecords, IReadOnlyList<PatternDto> Patterns);

/// <summary>
/// 1 つのパターン。
/// patternIndex は並び順に 1 から振る。
/// </summary>
/// <param name="PatternIndex">並び順。1 から振る。</param>
/// <param name="RecordCount">このパターンになった記録の数。</param>
/// <param name="LastRecordedAt">このパターンの最後の記録の日時(UTC)。</param>
/// <param name="Entries">日、段階の順の収縮先。</param>
public sealed record PatternDto(int PatternIndex, int RecordCount, DateTime LastRecordedAt, IReadOnlyList<PatternEntryDto> Entries);

/// <summary>
/// 日と段階ごとの収縮先。
/// 第 1 次は zone、第 2 次は secondShrinkPoint だけを持つ。
/// </summary>
/// <param name="Day">日。1 か 2。</param>
/// <param name="Phase">段階。1 か 2。</param>
/// <param name="Zone">第 1 次の収縮先。第 2 次では null。</param>
/// <param name="SecondShrinkPoint">第 2 次の収縮先。第 1 次では null。</param>
public sealed record PatternEntryDto(int Day, int Phase, PatternZoneDto? Zone, PatternPointDto? SecondShrinkPoint);

public sealed record PatternZoneDto(int Id, string NameJa, decimal CenterX, decimal CenterY, decimal Radius);

public sealed record PatternPointDto(int Id, string NameJa, decimal X, decimal Y);
