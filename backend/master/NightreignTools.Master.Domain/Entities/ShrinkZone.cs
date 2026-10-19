using NightreignTools.SharedKernel;

namespace NightreignTools.Master.Domain.Entities;

/// <summary>
/// 収縮候補地。
/// 第 1 次収縮の円の中心と半径。
/// </summary>
public sealed class ShrinkZone : MapPlace
{
    public const int IconPathMaxLength = 200;

    /// <summary>
    /// 日本語の名前。
    /// </summary>
    public string NameJa { get; private set; } = "";

    /// <summary>
    /// アイコンの画像のパス。
    /// null はアイコンなし。
    /// </summary>
    public string? IconPath { get; private set; }

    /// <summary>
    /// 円の中心の x 座標。
    /// 1024 四方の座標空間の数。
    /// </summary>
    public decimal CenterX { get; private set; }

    /// <summary>
    /// 円の中心の y 座標。
    /// 1024 四方の座標空間の数。
    /// </summary>
    public decimal CenterY { get; private set; }

    /// <summary>
    /// 円の半径。
    /// 1024 四方の座標空間の数。
    /// </summary>
    public decimal Radius { get; private set; }

    private ShrinkZone()
    {
    }

    /// <summary>
    /// 入力の決まり(マスタの管理 3.4)を確かめて作る。
    /// 決まりに反するときは 400 にする
    /// </summary>
    /// <param name="nameJa">日本語の名前。必須。前後の空白を除いて 100 文字まで。</param>
    /// <param name="iconPath">アイコンのパス。200 文字まで。空なら null にする。</param>
    /// <param name="centerX">中心の X 座標。0 以上 1024 以下で、小数は 2 桁まで。</param>
    /// <param name="centerY">中心の Y 座標。0 以上 1024 以下で、小数は 2 桁まで。</param>
    /// <param name="radius">半径。10 以上 512 以下で、小数は 2 桁まで。</param>
    /// <returns>保存の前の行。</returns>
    public static ShrinkZone Create(string? nameJa, string? iconPath, decimal centerX, decimal centerY, decimal radius)
    {
        var zone = new ShrinkZone();
        zone.Update(nameJa, iconPath, centerX, centerY, radius);
        return zone;
    }

    /// <summary>
    /// 入力の決まり(マスタの管理 3.4)を確かめて直す。
    /// 決まりに反するときは何も変えずに 400 にする
    /// </summary>
    /// <param name="nameJa">日本語の名前。必須。前後の空白を除いて 100 文字まで。</param>
    /// <param name="iconPath">アイコンのパス。200 文字まで。空なら null にする。</param>
    /// <param name="centerX">中心の X 座標。0 以上 1024 以下で、小数は 2 桁まで。</param>
    /// <param name="centerY">中心の Y 座標。0 以上 1024 以下で、小数は 2 桁まで。</param>
    /// <param name="radius">半径。10 以上 512 以下で、小数は 2 桁まで。</param>
    public void Update(string? nameJa, string? iconPath, decimal centerX, decimal centerY, decimal radius)
    {
        var name = RequireName(nameJa);
        var icon = string.IsNullOrWhiteSpace(iconPath) ? null : iconPath.Trim();
        if (icon is { Length: > IconPathMaxLength })
        {
            throw new BusinessRuleException($"アイコンのパスは {IconPathMaxLength} 文字までです");
        }

        var x = RequireCoordinate(centerX);
        var y = RequireCoordinate(centerY);
        var r = RequireRadius(radius);
        (NameJa, IconPath, CenterX, CenterY, Radius) = (name, icon, x, y, r);
    }
}
