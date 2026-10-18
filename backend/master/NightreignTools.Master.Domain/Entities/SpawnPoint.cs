namespace NightreignTools.Master.Domain.Entities;

/// <summary>開始地点。プレイの始まりの場所。</summary>
public sealed class SpawnPoint : MapPlace
{
    /// <summary>日本語の名前。</summary>
    public string NameJa { get; private set; } = "";

    /// <summary>x 座標。1024 四方の座標空間の数。</summary>
    public decimal X { get; private set; }

    /// <summary>y 座標。1024 四方の座標空間の数。</summary>
    public decimal Y { get; private set; }
}
