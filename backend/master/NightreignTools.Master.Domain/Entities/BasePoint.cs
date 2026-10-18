namespace NightreignTools.Master.Domain.Entities;

/// <summary>拠点候補地。マップ(通常か地変)ごとに持つ。拠点の種類は持たない(決定の記録 0010)。</summary>
public sealed class BasePoint : MapPlace
{
    public const string NormalMapId = "normal";

    /// <summary>どのマップの拠点か。normal は通常のマップ、ほかは地変の識別子。</summary>
    public string MapId { get; private set; } = NormalMapId;

    /// <summary>日本語の名前。null は名前なし。</summary>
    public string? NameJa { get; private set; }

    /// <summary>x 座標。1024 四方の座標空間の数。</summary>
    public decimal X { get; private set; }

    /// <summary>y 座標。1024 四方の座標空間の数。</summary>
    public decimal Y { get; private set; }
}
