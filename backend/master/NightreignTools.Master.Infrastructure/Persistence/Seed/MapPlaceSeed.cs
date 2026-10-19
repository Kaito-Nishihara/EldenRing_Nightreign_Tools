using NightreignTools.Master.Domain.Entities;

namespace NightreignTools.Master.Infrastructure.Persistence.Seed;

/// <summary>
/// マップの上の場所(開始地点、収縮候補地、第二収縮地点、拠点候補地)の初期データ(データベースの共通仕様 5.6)。
/// 収縮候補地と第二収縮地点の座標はダミーで、管理の画面で取り直す(要件定義の 12 章)。
/// </summary>
internal static class MapPlaceSeed
{
    /// <summary>
    /// 収縮候補地の半径。旧版のデータベースで、利用者が実際のゲームを見て登録した円の半径が 255 だった。
    /// </summary>
    public const decimal ZoneRadius = 255m;

    /// <summary>
    /// 開始地点(旧版の V8)。
    /// </summary>
    public static readonly object[] SpawnPoints =
    [
        Point(1, "北東の教会", 351m, 387m),
        Point(2, "西の教会", 216m, 578m),
        Point(3, "南西の教会", 218m, 763m),
        Point(4, "南の教会", 577m, 830m),
        Point(5, "南東の教会", 561m, 679m),
        Point(6, "北の教会", 547m, 215m),
        Point(7, "北西の教会", 218m, 360m),
        Point(8, "東の教会", 697m, 398m),
        Point(9, "東の野営地", 808m, 569m),
    ];

    /// <summary>
    /// 12 の地域の名前と座標(旧版の V10 の第二収縮地点)。収縮候補地の初期データにも使う。
    /// </summary>
    private static readonly (string Name, decimal X, decimal Y)[] Areas =
    [
        ("最北西", 219m, 233m),
        ("中央西", 342m, 488m),
        ("最北東", 789m, 248m),
        ("最北", 583m, 297m),
        ("最東", 739m, 518m),
        ("中央東", 558m, 552m),
        ("最南東", 715m, 692m),
        ("最南", 493m, 774m),
        ("中央南西", 383m, 711m),
        ("最南西", 277m, 728m),
        ("最西", 211m, 402m),
        ("中央南", 408m, 624m),
    ];

    /// <summary>
    /// 第二収縮地点(旧版の V10)。
    /// </summary>
    public static readonly object[] SecondShrinkPoints =
        Areas.Select((a, i) => Point(i + 1, a.Name, a.X, a.Y)).ToArray();

    /// <summary>
    /// 収縮候補地。旧版の V3 の 12 か所(収縮地点A〜L)は格子に並べただけのダミーだったので、第二収縮地点と同じ名前と中心に置き直した。
    /// </summary>
    public static readonly object[] ShrinkZones =
        Areas.Select((a, i) => (object)new
        {
            Id = i + 1, NameJa = a.Name, CenterX = a.X, CenterY = a.Y, Radius = ZoneRadius, CreatedAt = SeedData.CreatedAt,
        }).ToArray();

    /// <summary>
    /// 通常のマップの拠点候補地(旧版の V7)。
    /// 実際のゲームで確かめた座標。
    /// </summary>
    public static readonly object[] BasePoints =
    [
        Base(1, 404m, 798m),
        Base(2, 615m, 698m),
        Base(3, 279m, 667m),
        Base(4, 317m, 562m),
        Base(5, 278m, 459m),
        Base(6, 225m, 272m),
        Base(7, 410m, 197m),
        Base(8, 434m, 316m),
        Base(9, 638m, 301m),
        Base(10, 742m, 232m),
        Base(11, 801m, 438m),
        Base(12, 575m, 451m),
        Base(13, 677m, 501m),
        Base(14, 766m, 763m),
        Base(15, 777m, 636m),
    ];

    private static object Point(int id, string nameJa, decimal x, decimal y) =>
        new { Id = id, NameJa = nameJa, X = x, Y = y, CreatedAt = SeedData.CreatedAt };

    private static object Base(int id, decimal x, decimal y) =>
        new { Id = id, MapId = BasePoint.NormalMapId, X = x, Y = y, CreatedAt = SeedData.CreatedAt };
}
