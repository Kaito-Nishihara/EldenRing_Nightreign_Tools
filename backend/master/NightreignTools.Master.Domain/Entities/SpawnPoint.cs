namespace NightreignTools.Master.Domain.Entities;

/// <summary>
/// 開始地点。
/// プレイの始まりの場所。
/// </summary>
public sealed class SpawnPoint : MapPlace
{
    /// <summary>
    /// 日本語の名前。
    /// </summary>
    public string NameJa { get; private set; } = "";

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

    private SpawnPoint()
    {
    }

    /// <summary>
    /// 入力の決まり(マスタの管理 3.4)を確かめて作る。
    /// 決まりに反するときは 400 にする
    /// </summary>
    /// <param name="nameJa">日本語の名前。必須。前後の空白を除いて 100 文字まで。</param>
    /// <param name="x">X 座標。0 以上 1024 以下で、小数は 2 桁まで。</param>
    /// <param name="y">Y 座標。0 以上 1024 以下で、小数は 2 桁まで。</param>
    /// <returns>保存の前の行。</returns>
    public static SpawnPoint Create(string? nameJa, decimal x, decimal y)
    {
        var place = new SpawnPoint();
        place.Update(nameJa, x, y);
        return place;
    }

    /// <summary>
    /// 入力の決まり(マスタの管理 3.4)を確かめて直す。
    /// 決まりに反するときは何も変えずに 400 にする
    /// </summary>
    /// <param name="nameJa">日本語の名前。必須。前後の空白を除いて 100 文字まで。</param>
    /// <param name="x">X 座標。0 以上 1024 以下で、小数は 2 桁まで。</param>
    /// <param name="y">Y 座標。0 以上 1024 以下で、小数は 2 桁まで。</param>
    public void Update(string? nameJa, decimal x, decimal y)
    {
        var name = RequireName(nameJa);
        var validX = RequireCoordinate(x);
        var validY = RequireCoordinate(y);
        (NameJa, X, Y) = (name, validX, validY);
    }
}
