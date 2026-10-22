using NightreignTools.SharedKernel;

namespace NightreignTools.Master.Domain.Entities;

/// <summary>
/// 拠点候補地。
/// マップ(通常か地変)ごとに持ち、大拠点、小拠点、強敵の種類を持つ(決定の記録 0018)。
/// その場所に出る施設の種類は持たない(決定の記録 0010)。
/// </summary>
public sealed class BasePoint : MapPlace
{
    public const string NormalMapId = "normal";

    public const string KindRequiredMessage = "種類を選んでください";

    /// <summary>
    /// どのマップの拠点か。
    /// normal は通常のマップ、ほかは地変の識別子。
    /// </summary>
    public string MapId { get; private set; } = NormalMapId;

    /// <summary>
    /// 拠点の種類。
    /// major は大拠点、minor は小拠点、field_boss は強敵。
    /// </summary>
    public string Kind { get; private set; } = BasePointKinds.Major;

    /// <summary>
    /// 日本語の名前。
    /// null は名前なし。
    /// </summary>
    public string? NameJa { get; private set; }

    /// <summary>
    /// x 座標。
    /// 1024 四方の座標空間の数。
    /// </summary>
    public decimal X { get; private set; }

    /// <summary>
    /// y 座標。
    /// 1024 四方の座標空間の数。
    /// </summary>
    public decimal Y { get; private set; }

    private BasePoint()
    {
    }

    /// <summary>
    /// 入力の決まり(マスタの管理 3.4)を確かめて作る。名前は任意。
    /// マップの識別子があるかどうかは地変のテーブルを見る必要があるので、アプリケーションサービスが確かめる。
    /// </summary>
    /// <param name="mapId">マップの識別子。あるかどうかはここでは確かめない。</param>
    /// <param name="kind">拠点の種類。<see cref="BasePointKinds.All"/> のどれか。</param>
    /// <param name="nameJa">日本語の名前。任意。前後の空白を除いて 100 文字まで。</param>
    /// <param name="x">X 座標。0 以上 1024 以下で、小数は 2 桁まで。</param>
    /// <param name="y">Y 座標。0 以上 1024 以下で、小数は 2 桁まで。</param>
    /// <returns>保存の前の行。</returns>
    public static BasePoint Create(string mapId, string? kind, string? nameJa, decimal x, decimal y)
    {
        var point = new BasePoint();
        point.Update(mapId, kind, nameJa, x, y);
        return point;
    }

    /// <summary>
    /// 入力の決まり(マスタの管理 3.4)を確かめて直す。
    /// 決まりに反するときは何も変えずに 400 にする
    /// </summary>
    /// <param name="mapId">マップの識別子。あるかどうかはここでは確かめない。</param>
    /// <param name="kind">拠点の種類。<see cref="BasePointKinds.All"/> のどれか。</param>
    /// <param name="nameJa">日本語の名前。任意。前後の空白を除いて 100 文字まで。</param>
    /// <param name="x">X 座標。0 以上 1024 以下で、小数は 2 桁まで。</param>
    /// <param name="y">Y 座標。0 以上 1024 以下で、小数は 2 桁まで。</param>
    public void Update(string mapId, string? kind, string? nameJa, decimal x, decimal y)
    {
        var validKind = RequireKind(kind);
        var name = OptionalName(nameJa);
        var validX = RequireCoordinate(x);
        var validY = RequireCoordinate(y);
        (MapId, Kind, NameJa, X, Y) = (mapId, validKind, name, validX, validY);
    }

    /// <summary>
    /// 種類を確かめる。
    /// 省いたときと、決まった値でないときは 400 にする
    /// </summary>
    /// <param name="kind">入力の種類。</param>
    /// <returns>前後の空白を除いた種類。</returns>
    private static string RequireKind(string? kind)
    {
        var trimmed = kind?.Trim();
        return trimmed is not null && BasePointKinds.All.Contains(trimmed)
            ? trimmed
            : throw new BusinessRuleException(KindRequiredMessage);
    }
}

/// <summary>
/// 拠点の種類(<see cref="BasePoint.Kind"/>)の値(決定の記録 0018)。
/// </summary>
public static class BasePointKinds
{
    /// <summary>
    /// 大拠点。参考にした地図の赤の丸
    /// </summary>
    public const string Major = "major";

    /// <summary>
    /// 小拠点。参考にした地図の青の丸
    /// </summary>
    public const string Minor = "minor";

    /// <summary>
    /// 強敵(フィールドボス)。出る場所は決まっていて、出るボスは毎回変わる
    /// </summary>
    public const string FieldBoss = "field_boss";

    public static readonly IReadOnlyList<string> All = [Major, Minor, FieldBoss];
}
