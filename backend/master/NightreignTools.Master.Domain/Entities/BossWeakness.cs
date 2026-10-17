namespace NightreignTools.Master.Domain.Entities;

/// <summary>夜の王の弱点の属性。例: holy、fire。</summary>
public sealed class BossWeakness
{
    public int Id { get; private set; }
    public string BossId { get; private set; } = "";
    public string Attribute { get; private set; } = "";
    public DateTime CreatedAt { get; private set; }
}
