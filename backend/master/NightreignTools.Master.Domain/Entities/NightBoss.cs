namespace NightreignTools.Master.Domain.Entities;

/// <summary>1 日目か 2 日目の夜に出る夜ボス。夜の王とは別に持つ。</summary>
public sealed class NightBoss
{
    public int Id { get; private set; }
    public string NameJa { get; private set; } = "";

    /// <summary>出る日。1 か 2。</summary>
    public int Day { get; private set; }

    public DateTime CreatedAt { get; private set; }
}
