namespace NightreignTools.Relic.Domain.Entities;

/// <summary>
/// 遺物に付く 1 つの効果。通常効果と深層効果を同じテーブルで持ち、深層効果かで分ける。マイグレーションの初期データだけで管理する。
/// </summary>
public sealed class RelicEffect
{
    private readonly List<RelicEffectRank> _ranks = [];

    /// <summary>識別子。</summary>
    public int Id { get; private set; }

    /// <summary>分類の識別子。</summary>
    public string CategoryId { get; private set; } = "";

    /// <summary>日本語の名前。</summary>
    public string NameJa { get; private set; } = "";

    /// <summary>深き夜の遺物に付く効果(深層効果)か。</summary>
    public bool IsDeep { get; private set; }

    /// <summary>効果がメリットかデメリットか。positive か negative。</summary>
    public string EffectSign { get; private set; } = RelicEffectSigns.Positive;

    /// <summary>効果を重ねるときの合わせ方。multiply、percent_add、flat_add、special のどれか。</summary>
    public string CalcType { get; private set; } = "";

    /// <summary>効果がかかる対象。例: physical、strength。null は効果ごとに別に合わせる。</summary>
    public string? Attribute { get; private set; }

    /// <summary>同じ効果を重ねて付けられるか。yes、no、conditional(条件付き)のどれか。</summary>
    public string Stackable { get; private set; } = RelicStackables.Yes;

    /// <summary>ランクの下限。ランクのない効果は 0。</summary>
    public int RankMin { get; private set; }

    /// <summary>ランクの上限。ランクのない効果は 0。</summary>
    public int RankMax { get; private set; }

    /// <summary>発動の条件。例: HP40%未満で発動。null は条件なし。</summary>
    public string? ConditionJa { get; private set; }

    /// <summary>備考。null はなし。</summary>
    public string? NotesJa { get; private set; }

    /// <summary>行を作った日時(UTC)。データベースの既定値で入る。</summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>ランクごとの値。ランクのない効果は、ランク 0 の 1 件だけを持つ。</summary>
    public IReadOnlyList<RelicEffectRank> Ranks => _ranks;
}

/// <summary>効果の向き(<see cref="RelicEffect.EffectSign"/>)の値。</summary>
public static class RelicEffectSigns
{
    public const string Positive = "positive";
    public const string Negative = "negative";

    public static readonly IReadOnlyList<string> All = [Positive, Negative];
}

/// <summary>計算の種類(<see cref="RelicEffect.CalcType"/>)の値(ビルドシミュレーター 3.2)。</summary>
public static class RelicCalcTypes
{
    public const string Multiply = "multiply";
    public const string PercentAdd = "percent_add";
    public const string FlatAdd = "flat_add";
    public const string Special = "special";

    public static readonly IReadOnlyList<string> All = [Multiply, PercentAdd, FlatAdd, Special];
}

/// <summary>重ねて付けられるか(<see cref="RelicEffect.Stackable"/>)の値。</summary>
public static class RelicStackables
{
    public const string Yes = "yes";
    public const string No = "no";
    public const string Conditional = "conditional";

    public static readonly IReadOnlyList<string> All = [Yes, No, Conditional];
}
