using NightreignTools.Relic.Domain.Entities;

namespace NightreignTools.Relic.Infrastructure.Persistence.Seed;

/// <summary>
/// 遺物の効果の分類、効果、ランクごとの値の初期データ(旧版の V11)。
/// 効果の識別子は V11 で入れた順に 1 から振る。ランクごとの値の識別子は、効果の順、ランクの順に 1 から振る。
/// </summary>
internal static class RelicEffectSeed
{
    private const string Stat = "stat";
    private const string Attack = "attack";
    private const string CutRate = "cut_rate";
    private const string Recovery = "recovery";
    private const string StatusResist = "status_resist";
    private const string ArtsGauge = "arts_gauge";
    private const string Other = "other";

    public static readonly object[] Categories =
    [
        Category(Stat, "能力値", 0),
        Category(Attack, "攻撃力", 1),
        Category(CutRate, "カット率", 2),
        Category(Recovery, "回復", 3),
        Category(StatusResist, "状態異常耐性", 4),
        Category(ArtsGauge, "アーツゲージ", 5),
        Category("starting_item", "出撃装備", 6),
        Category(Other, "その他", 7),
    ];

    private static readonly (string Attribute, string NameJa)[] Stats =
    [
        ("vigor", "生命力"), ("mind", "精神力"), ("endurance", "持久力"), ("strength", "筋力"),
        ("dexterity", "技量"), ("intelligence", "知力"), ("faith", "信仰"), ("arcane", "神秘"),
    ];

    private static readonly (string Attribute, string NameJa)[] Elements =
    [
        ("magic", "魔力"), ("fire", "炎"), ("lightning", "雷"), ("holy", "聖"),
    ];

    private static readonly (string Attribute, string NameJa)[] Resists =
    [
        ("poison", "毒"), ("rot", "腐敗"), ("bleed", "出血"), ("frostbite", "冷気"),
        ("sleep", "睡眠"), ("madness", "発狂"), ("death", "抗死"),
    ];

    /// <summary>深層効果で下がる能力値の組(V11 の並び)。</summary>
    private static readonly (string First, string Second)[] StatDownPairs =
    [
        ("vigor", "arcane"), ("strength", "intelligence"), ("dexterity", "faith"),
        ("intelligence", "dexterity"), ("faith", "strength"),
    ];

    /// <summary>被ダメージ時に蓄積する状態異常と値(V11 の並び)。</summary>
    private static readonly (string Attribute, string NameJa, decimal Value)[] Buildups =
    [
        ("poison", "毒", 65), ("rot", "腐敗", 55), ("bleed", "出血", 55), ("frostbite", "冷気", 55),
    ];

    private static readonly EffectDefinition[] Definitions =
    [
        // [通常効果] 能力値
        new(Stat, "最大HP上昇", RelicCalcTypes.FlatAdd, Stackable: RelicStackables.No, Ranks: [Flat(100)]),
        new(Stat, "最大FP上昇", RelicCalcTypes.FlatAdd, Ranks: [Flat(25)]),
        new(Stat, "最大スタミナ上昇", RelicCalcTypes.FlatAdd, Ranks: [Flat(10)]),
        .. Stats.Select(s => new EffectDefinition(Stat, $"{s.NameJa}+1〜3", RelicCalcTypes.FlatAdd, Attribute: s.Attribute,
            RankMin: 1, Ranks: [Flat(1), Flat(2), Flat(3)])),
        new(Stat, "強靭度+1〜3", RelicCalcTypes.PercentAdd, Attribute: "toughness", RankMin: 1, Ranks: [Percent(5), Percent(10), Percent(15)]),

        // [通常効果] 攻撃力
        .. new[] { ("physical", "物理") }.Concat(Elements).Select(e => new EffectDefinition(Attack, $"{e.Item2}攻撃力上昇+0〜2",
            RelicCalcTypes.PercentAdd, Attribute: e.Item1, Ranks: [Percent(4), Percent(5), Percent(6)])),
        new(Attack, "近接攻撃力上昇", RelicCalcTypes.Multiply, Attribute: "melee", Ranks: [Times(1.05m)]),
        new(Attack, "戦技攻撃力上昇", RelicCalcTypes.Multiply, Attribute: "skill", Ranks: [Times(1.15m)]),
        new(Attack, "通常攻撃の1段目強化", RelicCalcTypes.Multiply, Ranks: [Times(1.15m)]),
        new(Attack, "致命の一撃強化", RelicCalcTypes.Multiply, Ranks: [Times(1.17m)]),
        new(Attack, "致命の一撃強化+1", RelicCalcTypes.Multiply, Stackable: RelicStackables.No, Ranks: [Times(1.24m)]),
        new(Attack, "ガードカウンター強化", RelicCalcTypes.Multiply, Ranks: [Times(1.17m)]),
        new(Attack, "咆哮とブレス強化", RelicCalcTypes.Multiply, Attribute: "breath", Ranks: [Times(1.15m)]),
        new(Attack, "投擲壺の攻撃力上昇", RelicCalcTypes.Multiply, Attribute: "throw_pot", Ranks: [Times(1.15m)]),
        new(Attack, "投擲ナイフの攻撃力上昇", RelicCalcTypes.Multiply, Attribute: "throw_knife", Ranks: [Times(1.14m)]),
        new(Attack, "攻撃連続時、攻撃力上昇", RelicCalcTypes.Special, Stackable: RelicStackables.Conditional,
            NotesJa: "段階的に上昇: 1段目×1.05 → 累計×1.12 → 累計×1.22", Ranks: [NoValue()]),

        // [通常効果] カット率
        new(CutRate, "物理カット率上昇", RelicCalcTypes.PercentAdd, Attribute: "physical", Stackable: RelicStackables.No, Ranks: [Percent(8)]),
        .. Elements.Select(e => new EffectDefinition(CutRate, $"{e.NameJa}カット率上昇", RelicCalcTypes.PercentAdd, Attribute: e.Attribute,
            Ranks: [Percent(10)])),
        new(CutRate, "HP低下時、カット率上昇", RelicCalcTypes.PercentAdd, Stackable: RelicStackables.Conditional,
            ConditionJa: "HP40%未満で発動", Ranks: [Percent(15)]),
        new(CutRate, "ダメージで吹き飛ばされた時、強靭度とカット率上昇", RelicCalcTypes.Special, Stackable: RelicStackables.No,
            NotesJa: "カット率+20%・強靭度+20%、20秒持続", Ranks: [NoValue()]),

        // [通常効果] 状態異常耐性
        .. Resists.Select(r => new EffectDefinition(StatusResist, $"{r.NameJa}耐性上昇", RelicCalcTypes.FlatAdd, Attribute: r.Attribute,
            Ranks: [Flat(75)])),

        // [通常効果] 回復
        new(Recovery, "HP持続回復", RelicCalcTypes.FlatAdd, Ranks: [PerSec(2)]),
        new(Recovery, "HP低下時、周囲の味方を含めHPをゆっくり回復", RelicCalcTypes.Special, Stackable: RelicStackables.No,
            ConditionJa: "HP50%以下で発動", NotesJa: "最大HP0.5%+1/秒、50秒持続", Ranks: [NoValue()]),
        new(Recovery, "ガード成功時、HP回復", RelicCalcTypes.FlatAdd, Stackable: RelicStackables.No, NotesJa: "クールタイムあり", Ranks: [Flat(15)]),
        new(Recovery, "刺突カウンター発生時、HP回復", RelicCalcTypes.PercentAdd, Stackable: RelicStackables.No, Ranks: [Percent(2.5m)]),
        new(Recovery, "発狂状態になると、FP持続回復", RelicCalcTypes.Special, Stackable: RelicStackables.Conditional,
            ConditionJa: "発狂状態中のみ", NotesJa: "+2/秒、25秒間（合計+50）", Ranks: [PerSec(2)]),
        new(Recovery, "攻撃命中時、スタミナ回復", RelicCalcTypes.Special, Stackable: RelicStackables.No,
            NotesJa: "1ヒットにつき約2〜3回復", Ranks: [NoValue()]),
        new(Recovery, "致命の一撃で、スタミナ回復速度上昇", RelicCalcTypes.Special, Stackable: RelicStackables.No,
            NotesJa: "+15%、15秒間", Ranks: [NoValue()]),
        new(Recovery, "リゲイン有効化", RelicCalcTypes.Special, Stackable: RelicStackables.No,
            NotesJa: "ダメージ後の攻撃で一部HP回復", Ranks: [NoValue()]),

        // [通常効果] アーツゲージ
        new(ArtsGauge, "スキルクールタイム軽減+1〜3", RelicCalcTypes.PercentAdd, RankMin: 1, Ranks: [Percent(5), Percent(7.5m), Percent(10)]),
        new(ArtsGauge, "アーツゲージ自然蓄積+1〜3", RelicCalcTypes.PercentAdd, RankMin: 1, Ranks: [Percent(5), Percent(7.5m), Percent(10)]),
        new(ArtsGauge, "敵撃破時、アーツゲージ増加", RelicCalcTypes.Special, Stackable: RelicStackables.No, NotesJa: "約5%増加", Ranks: [NoValue()]),
        new(ArtsGauge, "致命の一撃でアーツゲージ増加", RelicCalcTypes.Special, Stackable: RelicStackables.No, NotesJa: "約5%増加", Ranks: [NoValue()]),

        // [深層効果] メリット
        new(Stat, "最大HP上昇+10%", RelicCalcTypes.PercentAdd, IsDeep: true, Ranks: [Percent(10, demerit: true)]),
        new(Stat, "最大FP上昇+15%", RelicCalcTypes.PercentAdd, IsDeep: true, Ranks: [Percent(15, demerit: true)]),
        new(Stat, "最大スタミナ上昇+12%", RelicCalcTypes.PercentAdd, IsDeep: true, Ranks: [Percent(12, demerit: true)]),
        new(Attack, "物理攻撃力上昇+0〜4（深層）", RelicCalcTypes.PercentAdd, IsDeep: true, Attribute: "physical",
            Ranks: [Percent(4), Percent(5), Percent(6), Percent(10.5m, demerit: true), Percent(12, demerit: true)]),
        .. Elements.Select(e => new EffectDefinition(Attack, $"{e.NameJa}攻撃力上昇+0〜2（深層）", RelicCalcTypes.PercentAdd, IsDeep: true,
            Attribute: e.Attribute, Ranks: [Percent(5), Percent(8, demerit: true), Percent(10, demerit: true)])),
        new(Attack, "ガードカウンター強化+0〜2（深層）", RelicCalcTypes.Multiply, IsDeep: true,
            Ranks: [Times(1.17m), Times(1.25m, demerit: true), Times(1.29m, demerit: true)]),

        // [深層効果] デメリット
        new(Recovery, "HP持続減少", RelicCalcTypes.FlatAdd, IsDeep: true, Negative: true, Ranks: [PerSec(-2)]),
        new(StatusResist, "すべての状態異常耐性低下", RelicCalcTypes.FlatAdd, IsDeep: true, Negative: true, Attribute: "all_status", Ranks: [Flat(-80)]),
        .. StatDownPairs.Select(p => new EffectDefinition(Stat, $"{StatName(p.First)}と{StatName(p.Second)}が低下", RelicCalcTypes.FlatAdd,
            IsDeep: true, Negative: true, Attribute: $"{p.First}_{p.Second}", Ranks: [Flat(-3)])),
        new(Other, "取得ルーン減少", RelicCalcTypes.Multiply, IsDeep: true, Negative: true, Ranks: [Times(0.9m)]),
        .. Buildups.Select(b => new EffectDefinition(StatusResist, $"被ダメージ時、{b.NameJa}を蓄積", RelicCalcTypes.Special,
            IsDeep: true, Negative: true, Attribute: b.Attribute, Ranks: [Flat(b.Value)])),
        new(CutRate, "回避連続時、カット率低下", RelicCalcTypes.PercentAdd, IsDeep: true, Negative: true, Stackable: RelicStackables.Conditional,
            NotesJa: "-35%、15秒持続", Ranks: [Percent(-35)]),
    ];

    public static readonly object[] Effects = Definitions.Select((d, index) => d.ToEffect(index + 1)).ToArray();

    public static readonly object[] Ranks = Definitions
        .SelectMany((d, index) => d.Ranks.Select((rank, offset) => (EffectId: index + 1, Rank: d.RankMin + offset, Value: rank)))
        .Select((r, index) => r.Value.ToRank(index + 1, r.EffectId, r.Rank))
        .ToArray();

    private static string StatName(string attribute) => Stats.Single(s => s.Attribute == attribute).NameJa;

    private static object Category(string id, string nameJa, int sortOrder) =>
        new { Id = id, NameJa = nameJa, SortOrder = sortOrder, CreatedAt = SeedData.CreatedAt };

    private static RankValue Flat(decimal value) => new(value, RelicValueUnits.Flat, false);

    private static RankValue Percent(decimal value, bool demerit = false) => new(value, RelicValueUnits.Percent, demerit);

    private static RankValue Times(decimal value, bool demerit = false) => new(value, RelicValueUnits.Multiplier, demerit);

    private static RankValue PerSec(decimal value) => new(value, RelicValueUnits.PerSec, false);

    private static RankValue NoValue() => new(null, RelicValueUnits.None, false);

    /// <summary>1 つの効果の定義。ランクは <paramref name="RankMin"/> から順に並べる。ランクが 1 つなら、ランクのない効果(下限と上限が 0)とする。</summary>
    private sealed record EffectDefinition(
        string CategoryId,
        string NameJa,
        string CalcType,
        RankValue[] Ranks,
        bool IsDeep = false,
        bool Negative = false,
        string? Attribute = null,
        string Stackable = RelicStackables.Yes,
        int RankMin = 0,
        string? ConditionJa = null,
        string? NotesJa = null)
    {
        private int RankMax => Ranks.Length == 1 ? RankMin : RankMin + Ranks.Length - 1;

        public object ToEffect(int id) => new
        {
            Id = id,
            CategoryId,
            NameJa,
            IsDeep,
            EffectSign = Negative ? RelicEffectSigns.Negative : RelicEffectSigns.Positive,
            CalcType,
            Attribute,
            Stackable,
            RankMin,
            RankMax,
            ConditionJa,
            NotesJa,
            CreatedAt = SeedData.CreatedAt,
        };
    }

    private sealed record RankValue(decimal? Value, string Unit, bool HasDemerit)
    {
        public object ToRank(int id, int effectId, int rank) => new
        {
            Id = id,
            EffectId = effectId,
            Rank = rank,
            Value,
            Unit,
            HasDemerit,
            CreatedAt = SeedData.CreatedAt,
        };
    }
}
