namespace NightreignTools.Relic.Application.RelicEffects;

// 遺物の効果の一覧の出力(ビルドシミュレーター 4 章)。エンティティを応答に返さず、これに詰め替える

/// <summary>
/// 遺物の効果。
/// 分類の名前と並び順を含める。
/// 画面で分類ごとにまとめて並べるためである。
/// </summary>
/// <param name="Id">効果の識別子。</param>
/// <param name="CategoryId">分類の識別子。</param>
/// <param name="CategoryNameJa">分類の日本語の名前。</param>
/// <param name="CategorySortOrder">分類の並び順。小さいほど先に出す。</param>
/// <param name="NameJa">効果の日本語の名前。</param>
/// <param name="IsDeep">深層効果か。</param>
/// <param name="EffectSign">メリットかデメリットか。positive か negative。</param>
/// <param name="CalcType">重ねるときの合わせ方。multiply、percent_add、flat_add、special のどれか。</param>
/// <param name="Attribute">効果がかかる対象。null は効果ごとに別に合わせる。</param>
/// <param name="Stackable">同じ効果を重ねて付けられるか。yes、no、conditional のどれか。</param>
/// <param name="RankMin">ランクの下限。ランクのない効果は 0。</param>
/// <param name="RankMax">ランクの上限。ランクのない効果は 0。</param>
/// <param name="ConditionJa">発動の条件。null は条件なし。</param>
/// <param name="NotesJa">備考。null はなし。</param>
/// <param name="Ranks">ランクごとの値。ランクの小さい順。</param>
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

/// <summary>
/// ランクごとの値。
/// <paramref name="Value"/> は数で表せない効果(special)で null。
/// </summary>
/// <param name="Rank">ランク。ランクのない効果は 0。</param>
/// <param name="Value">このランクの値。単位は Unit による。</param>
/// <param name="Unit">値の単位。multiplier、percent、flat、per_sec、none のどれか。</param>
/// <param name="HasDemerit">このランクからデメリットが付くか。</param>
public sealed record RelicEffectRankDto(int Rank, decimal? Value, string Unit, bool HasDemerit);
