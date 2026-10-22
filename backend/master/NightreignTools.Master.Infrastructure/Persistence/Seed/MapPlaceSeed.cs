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
    /// 通常のマップの拠点候補地(決定の記録 0018)。
    /// 攻略 wiki の「地図と拠点の詳細」と「フィールドボス」の地図を、マップ画像に特徴点で重ねて写した座標。
    /// 1 から 15 は旧版の V7 の行で、どれも大拠点の丸に重なったので、大拠点にして座標を直した。
    /// 実際のゲームでは確かめていない(要件定義の 12 章)。
    /// </summary>
    public static readonly object[] BasePoints =
    [
        Base(1, BasePointKinds.Major, 406m, 801m),
        Base(2, BasePointKinds.Major, 622m, 709m),
        Base(3, BasePointKinds.Major, 285m, 670m),
        Base(4, BasePointKinds.Major, 325m, 568m),
        Base(5, BasePointKinds.Major, 287m, 462m),
        Base(6, BasePointKinds.Major, 234m, 288m),
        Base(7, BasePointKinds.Major, 409m, 182m),
        Base(8, BasePointKinds.Major, 425m, 308m),
        Base(9, BasePointKinds.Major, 642m, 303m),
        Base(10, BasePointKinds.Major, 730m, 229m),
        Base(11, BasePointKinds.Major, 791m, 436m),
        Base(12, BasePointKinds.Major, 597m, 439m),
        Base(13, BasePointKinds.Major, 679m, 476m),
        Base(14, BasePointKinds.Major, 763m, 760m),
        Base(15, BasePointKinds.Major, 771m, 643m),
        Base(16, BasePointKinds.Major, 644m, 608m),
        Base(17, BasePointKinds.Minor, 549m, 245m),
        Base(18, BasePointKinds.Minor, 805m, 361m),
        Base(19, BasePointKinds.Minor, 228m, 368m),
        Base(20, BasePointKinds.Minor, 709m, 381m),
        Base(21, BasePointKinds.Minor, 368m, 412m),
        Base(22, BasePointKinds.Minor, 216m, 572m),
        Base(23, BasePointKinds.Minor, 828m, 597m),
        Base(24, BasePointKinds.Minor, 563m, 655m),
        Base(25, BasePointKinds.Minor, 464m, 721m),
        Base(26, BasePointKinds.Minor, 208m, 727m),
        Base(27, BasePointKinds.Minor, 590m, 827m),
        Base(28, BasePointKinds.FieldBoss, 247m, 198m),
        Base(29, BasePointKinds.FieldBoss, 644m, 213m),
        Base(30, BasePointKinds.FieldBoss, 335m, 241m),
        Base(31, BasePointKinds.FieldBoss, 477m, 370m),
        Base(32, BasePointKinds.FieldBoss, 313m, 380m),
        Base(33, BasePointKinds.FieldBoss, 561m, 386m),
        Base(34, BasePointKinds.FieldBoss, 776m, 513m),
        Base(35, BasePointKinds.FieldBoss, 422m, 736m),
        Base(36, BasePointKinds.FieldBoss, 704m, 792m),
        Base(37, BasePointKinds.FieldBoss, 315m, 835m),
    ];

    private static object Point(int id, string nameJa, decimal x, decimal y) =>
        new { Id = id, NameJa = nameJa, X = x, Y = y, CreatedAt = SeedData.CreatedAt };

    private static object Base(int id, string kind, decimal x, decimal y) =>
        new { Id = id, MapId = BasePoint.NormalMapId, Kind = kind, X = x, Y = y, CreatedAt = SeedData.CreatedAt };
}
