namespace NightreignTools.Master.Domain.Entities;

/// <summary>拠点候補地。マップ(通常か地変)ごとに持つ。拠点の種類は持たない(決定の記録 0010)。</summary>
public sealed class BasePoint : MapPlace
{
    public const string NormalMapId = "normal";

    /// <summary><see cref="NormalMapId"/> か、地変の識別子。</summary>
    public string MapId { get; private set; } = NormalMapId;

    public string? NameJa { get; private set; }
    public decimal X { get; private set; }
    public decimal Y { get; private set; }
}
