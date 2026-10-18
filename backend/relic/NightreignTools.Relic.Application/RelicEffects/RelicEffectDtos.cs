namespace NightreignTools.Relic.Application.RelicEffects;

// 遺物の効果の一覧の出力(ビルドシミュレーター 4 章)。エンティティを応答に返さず、これに詰め替える

/// <summary>遺物の効果。分類の名前と並び順を含める。画面で分類ごとにまとめて並べるためである。</summary>
public sealed record RelicEffectDto(
    int Id,
    string CategoryId,
    string CategoryNameJa,
    int CategorySortOrder,
    string NameJa,
    bool IsDeep,
    string EffectSign,
    string CalcType,
    string? Attribute,
    string Stackable,
    int RankMin,
    int RankMax,
    string? ConditionJa,
    string? NotesJa,
    IReadOnlyList<RelicEffectRankDto> Ranks);

/// <summary>ランクごとの値。<paramref name="Value"/> は数で表せない効果(special)で null。</summary>
public sealed record RelicEffectRankDto(int Rank, decimal? Value, string Unit, bool HasDemerit);
