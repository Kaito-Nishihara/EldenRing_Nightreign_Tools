namespace NightreignTools.Master.Domain.Entities;

/// <summary>夜の王の弱点の属性。例: holy、fire。</summary>
public sealed class BossWeakness
{
    /// <summary>識別子。</summary>
    public int Id { get; private set; }

    /// <summary>夜の王の識別子。</summary>
    public string BossId { get; private set; } = "";

    /// <summary>弱点の属性。例: holy、fire。</summary>
    public string Attribute { get; private set; } = "";

    /// <summary>行を作った日時(UTC)。データベースの既定値で入る。</summary>
    public DateTime CreatedAt { get; private set; }
}
