namespace NightreignTools.Master.Domain.Entities;

/// <summary>収縮候補地。第 1 次収縮の円の中心と半径。</summary>
public sealed class ShrinkZone : MapPlace
{
    public string NameJa { get; private set; } = "";
    public string? IconPath { get; private set; }
    public decimal CenterX { get; private set; }
    public decimal CenterY { get; private set; }
    public decimal Radius { get; private set; }
}
