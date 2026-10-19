namespace NightreignTools.Master.Infrastructure.Persistence.Seed;

/// <summary>
/// 夜ボスの初期データ(旧版の V10)。
/// 1 日目 14 体、2 日目 21 体。
/// </summary>
internal static class NightBossSeed
{
    public static readonly object[] Rows =
    [
        NightBoss(1, "亜人の女王＆亜人の剣聖", 1),
        NightBoss(2, "鈴玉狩り", 1),
        NightBoss(3, "貪食ドラゴン", 1),
        NightBoss(4, "夜の騎兵：グレイブ＆夜の騎兵：フレイル", 1),
        NightBoss(5, "英雄のガーゴイル", 1),
        NightBoss(6, "ミミズ顔", 1),
        NightBoss(7, "百足のデーモン", 1),
        NightBoss(8, "熔鉄デーモン", 1),
        NightBoss(9, "戦場の宿将", 1),
        NightBoss(10, "爛れた樹霊", 1),
        NightBoss(11, "接ぎ木の君主", 1),
        NightBoss(12, "ティビアの呼び舟＆死に生きる者たち", 1),
        NightBoss(13, "王族の幽鬼", 1),
        NightBoss(14, "公のフレイディア", 1),
        NightBoss(15, "ツリーガード＆王都の騎兵", 2),
        NightBoss(16, "忌み鬼", 2),
        NightBoss(17, "古竜", 2),
        NightBoss(18, "坩堝の騎士＆黄金カバ", 2),
        NightBoss(19, "僻地の宿将", 2),
        NightBoss(20, "ノクスの竜人兵", 2),
        NightBoss(21, "竜のツリーガード＆王都の騎兵", 2),
        NightBoss(22, "大土竜", 2),
        NightBoss(23, "神肌の貴種＆神肌の使徒", 2),
        NightBoss(24, "降る星の成獣", 2),
        NightBoss(25, "冷たい谷の踊り子", 2),
        NightBoss(26, "無名の王", 2),
        NightBoss(27, "死儀礼の鳥", 2),
        NightBoss(28, "傷ついたデーモン＆うろ底のデーモン", 2),
        NightBoss(29, "デーモンの王子", 2),
        NightBoss(30, "呪剣士＆神獣の戦士", 2),
        NightBoss(31, "血の君主", 2),
        NightBoss(32, "大赤熊", 2),
        NightBoss(33, "神獣獅子舞", 2),
        NightBoss(34, "死の騎士", 2),
        NightBoss(35, "騎士アルトリウス", 2),
    ];

    private static object NightBoss(int id, string nameJa, int day) =>
        new { Id = id, NameJa = nameJa, Day = day, CreatedAt = SeedData.CreatedAt };
}
