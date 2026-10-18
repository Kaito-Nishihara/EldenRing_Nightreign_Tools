namespace NightreignTools.Master.Domain.Entities;

/// <summary>収縮候補地。第 1 次収縮の円の中心と半径。</summary>
public sealed class ShrinkZone : MapPlace
{
    /// <summary>日本語の名前。</summary>
    public string NameJa { get; private set; } = "";

    /// <summary>アイコンの画像のパス。null はアイコンなし。</summary>
    public string? IconPath { get; private set; }

    /// <summary>円の中心の x 座標。1024 四方の座標空間の数。</summary>
    public decimal CenterX { get; private set; }

    /// <summary>円の中心の y 座標。1024 四方の座標空間の数。</summary>
    public decimal CenterY { get; private set; }

    /// <summary>円の半径。1024 四方の座標空間の数。</summary>
    public decimal Radius { get; private set; }
}
