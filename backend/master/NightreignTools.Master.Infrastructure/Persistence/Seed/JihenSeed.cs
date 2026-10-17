namespace NightreignTools.Master.Infrastructure.Persistence.Seed;

/// <summary>
/// 地変の初期データ(旧版の V3)。旧版は中心を 0 から 1 の割合で持っていたので、1024 を掛けて座標空間の数に直した(決定の記録 0004)。
/// </summary>
internal static class JihenSeed
{
    public static readonly object[] Rows =
    [
        Jihen(1, "crater", "火口", "north", 512m, 204.8m),
        Jihen(2, "mountain", "山嶺", "northwest", 256m, 256m),
        Jihen(3, "rotted_forest", "腐れ森", "southwest", 256m, 768m),
        Jihen(4, "noklateo", "隠れ都ノクラテオ", "center", 512m, 512m),
        Jihen(5, "great_cavern", "大空洞", "all", 512m, 512m, isDlc: true),
    ];

    /// <summary><paramref name="sortOrder"/> は画面での並び順。旧版の V3 の並びのとおり</summary>
    private static object Jihen(int sortOrder, string id, string nameJa, string area, decimal centerX, decimal centerY, bool isDlc = false) =>
        new { Id = id, NameJa = nameJa, Area = area, CenterX = centerX, CenterY = centerY, IsDlc = isDlc, SortOrder = sortOrder, CreatedAt = SeedData.CreatedAt };
}
