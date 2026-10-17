namespace NightreignTools.Master.Domain.Entities;

/// <summary>第二収縮地点。第 2 次収縮の向かう点(決定の記録 0008)。</summary>
public sealed class SecondShrinkPoint : MapPlace
{
    public string NameJa { get; private set; } = "";
    public decimal X { get; private set; }
    public decimal Y { get; private set; }
}
