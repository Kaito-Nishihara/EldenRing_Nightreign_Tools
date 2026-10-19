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

    /// <summary>
    /// <paramref name="sortOrder"/> は画面での並び順。
    /// 旧版の V3 の並びのとおり
    /// </summary>
    /// <param name="sortOrder">画面での並び順。</param>
    /// <param name="id">識別子。</param>
    /// <param name="nameJa">日本語の名前。</param>
    /// <param name="area">地変が起きる方角。</param>
    /// <param name="centerX">中心の X 座標(1024 四方の座標空間)。</param>
    /// <param name="centerY">中心の Y 座標(1024 四方の座標空間)。</param>
    /// <param name="isDlc">ダウンロードコンテンツで足された地変か。</param>
    /// <returns>HasData に渡す匿名の型の行。</returns>
    private static object Jihen(int sortOrder, string id, string nameJa, string area, decimal centerX, decimal centerY, bool isDlc = false) =>
        new { Id = id, NameJa = nameJa, Area = area, CenterX = centerX, CenterY = centerY, IsDlc = isDlc, SortOrder = sortOrder, CreatedAt = SeedData.CreatedAt };
}
