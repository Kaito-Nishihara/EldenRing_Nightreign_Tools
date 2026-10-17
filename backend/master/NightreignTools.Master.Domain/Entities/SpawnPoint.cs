namespace NightreignTools.Master.Domain.Entities;

/// <summary>開始地点。プレイの始まりの場所。</summary>
public sealed class SpawnPoint : MapPlace
{
    public string NameJa { get; private set; } = "";
    public decimal X { get; private set; }
    public decimal Y { get; private set; }
}
