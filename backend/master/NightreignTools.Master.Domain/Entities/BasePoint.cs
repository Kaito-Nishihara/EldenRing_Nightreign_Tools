namespace NightreignTools.Master.Domain.Entities;

/// <summary>
/// 拠点候補地。
/// マップ(通常か地変)ごとに持つ。
/// 拠点の種類は持たない(決定の記録 0010)。
/// </summary>
public sealed class BasePoint : MapPlace
{
    public const string NormalMapId = "normal";

    /// <summary>
    /// どのマップの拠点か。
    /// normal は通常のマップ、ほかは地変の識別子。
    /// </summary>
    public string MapId { get; private set; } = NormalMapId;

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
    /// <param name="nameJa">日本語の名前。任意。前後の空白を除いて 100 文字まで。</param>
    /// <param name="x">X 座標。0 以上 1024 以下で、小数は 2 桁まで。</param>
    /// <param name="y">Y 座標。0 以上 1024 以下で、小数は 2 桁まで。</param>
    /// <returns>保存の前の行。</returns>
    public static BasePoint Create(string mapId, string? nameJa, decimal x, decimal y)
    {
        var point = new BasePoint();
        point.Update(mapId, nameJa, x, y);
        return point;
    }

    /// <summary>
    /// 入力の決まり(マスタの管理 3.4)を確かめて直す。
    /// 決まりに反するときは何も変えずに 400 にする
    /// </summary>
    /// <param name="mapId">マップの識別子。あるかどうかはここでは確かめない。</param>
    /// <param name="nameJa">日本語の名前。任意。前後の空白を除いて 100 文字まで。</param>
    /// <param name="x">X 座標。0 以上 1024 以下で、小数は 2 桁まで。</param>
    /// <param name="y">Y 座標。0 以上 1024 以下で、小数は 2 桁まで。</param>
    public void Update(string mapId, string? nameJa, decimal x, decimal y)
    {
        var name = OptionalName(nameJa);
        var validX = RequireCoordinate(x);
        var validY = RequireCoordinate(y);
        (MapId, NameJa, X, Y) = (mapId, name, validX, validY);
    }
}
