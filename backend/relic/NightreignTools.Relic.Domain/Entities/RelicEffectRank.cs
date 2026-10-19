namespace NightreignTools.Relic.Domain.Entities;

/// <summary>遺物の効果のランクごとの値。効果とランクの組は 1 行だけ。</summary>
public sealed class RelicEffectRank
{
    /// <summary>識別子。</summary>
    public int Id { get; private set; }

    /// <summary>効果の識別子。</summary>
    public int EffectId { get; private set; }

    /// <summary>ランク。ランクのない効果は 0。</summary>
    public int Rank { get; private set; }

    /// <summary>このランクの値。単位は <see cref="Unit"/> による。null は数で表せない効果(special)。</summary>
    public decimal? Value { get; private set; }

    /// <summary>値の単位。multiplier(倍率)、percent(% の点数)、flat(固定値)、per_sec(1 秒ごと)、none(数なし)のどれか。</summary>
    public string Unit { get; private set; } = "";

    /// <summary>このランクからデメリットが付くか。深層効果で使う。</summary>
    public bool HasDemerit { get; private set; }

    /// <summary>行を作った日時(UTC)。データベースの既定値で入る。</summary>
    public DateTime CreatedAt { get; private set; }
}

/// <summary>値の単位(<see cref="RelicEffectRank.Unit"/>)の値。</summary>
public static class RelicValueUnits
{
    public const string Multiplier = "multiplier";
    public const string Percent = "percent";
    public const string Flat = "flat";
    public const string PerSec = "per_sec";
    public const string None = "none";

    public static readonly IReadOnlyList<string> All = [Multiplier, Percent, Flat, PerSec, None];
}
