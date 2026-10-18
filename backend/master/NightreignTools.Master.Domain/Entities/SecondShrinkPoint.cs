namespace NightreignTools.Master.Domain.Entities;

/// <summary>第二収縮地点。第 2 次収縮の向かう点(決定の記録 0008)。</summary>
public sealed class SecondShrinkPoint : MapPlace
{
    /// <summary>日本語の名前。</summary>
    public string NameJa { get; private set; } = "";

    /// <summary>x 座標。1024 四方の座標空間の数。</summary>
    public decimal X { get; private set; }

    /// <summary>y 座標。1024 四方の座標空間の数。</summary>
    public decimal Y { get; private set; }

    private SecondShrinkPoint()
    {
    }

    /// <summary>入力の決まり(マスタの管理 3.4)を確かめて作る。決まりに反するときは 400 にする</summary>
    public static SecondShrinkPoint Create(string? nameJa, decimal x, decimal y)
    {
        var place = new SecondShrinkPoint();
        place.Update(nameJa, x, y);
        return place;
    }

    /// <summary>入力の決まり(マスタの管理 3.4)を確かめて直す。決まりに反するときは何も変えずに 400 にする</summary>
    public void Update(string? nameJa, decimal x, decimal y)
    {
        var name = RequireName(nameJa);
        var validX = RequireCoordinate(x);
        var validY = RequireCoordinate(y);
        (NameJa, X, Y) = (name, validX, validY);
    }
}
