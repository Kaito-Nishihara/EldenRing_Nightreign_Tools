namespace NightreignTools.Master.Infrastructure.Persistence.Seed;

/// <summary>夜の王と弱点の初期データ(旧版の V3)。</summary>
internal static class BossSeed
{
    public static readonly object[] Bosses =
    [
        Boss(1, "gladius", "三つ首の獣", "Gladius"),
        Boss(2, "edere", "喰らいつく顎", "Edere"),
        Boss(3, "gnostor", "知性の蟲", "Gnostor"),
        Boss(4, "maris", "兆し", "Maris"),
        Boss(5, "libra", "調律の魔物", "Libra"),
        Boss(6, "fulgor", "闇駆ける狩人", "Fulgor"),
        Boss(7, "caligo", "霧の裂け目", "Caligo"),
        Boss(8, "nameless", "夜を象る者", "Nameless", unlockCondition: "4体撃破後"),
        Boss(9, "harmonia", "英雄武器の娘たち", "Harmonia", isDlc: true),
        Boss(10, "strages", "反逆のストラゲス", "Strages", isDlc: true),
    ];

    public static readonly object[] Weaknesses =
    [
        Weakness(1, "gladius", "holy"),
        Weakness(2, "edere", "poison"),
        Weakness(3, "gnostor", "fire"),
        Weakness(4, "maris", "lightning"),
        Weakness(5, "libra", "holy"),
        Weakness(6, "libra", "fire"),
        Weakness(7, "libra", "madness"),
        Weakness(8, "fulgor", "lightning"),
        Weakness(9, "caligo", "fire"),
        Weakness(10, "nameless", "holy"),
        Weakness(11, "nameless", "fire"),
        Weakness(12, "nameless", "lightning"),
        Weakness(13, "harmonia", "sleep"),
        Weakness(14, "strages", "holy"),
        Weakness(15, "strages", "fire"),
    ];

    /// <summary><paramref name="sortOrder"/> は画面での並び順。旧版の V3 の並びのとおり</summary>
    private static object Boss(int sortOrder, string id, string nameJa, string nameEn, bool isDlc = false, string? unlockCondition = null) =>
        new { Id = id, NameJa = nameJa, NameEn = nameEn, IsDlc = isDlc, UnlockCondition = unlockCondition, SortOrder = sortOrder, CreatedAt = SeedData.CreatedAt };

    private static object Weakness(int id, string bossId, string attribute) =>
        new { Id = id, BossId = bossId, Attribute = attribute, CreatedAt = SeedData.CreatedAt };
}
