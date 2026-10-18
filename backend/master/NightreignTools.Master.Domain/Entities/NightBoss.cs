namespace NightreignTools.Master.Domain.Entities;

/// <summary>1 日目か 2 日目の夜に出る夜ボス。夜の王とは別に持つ。</summary>
public sealed class NightBoss
{
    /// <summary>識別子。</summary>
    public int Id { get; private set; }

    /// <summary>日本語の名前。</summary>
    public string NameJa { get; private set; } = "";

    /// <summary>出る日。1 か 2。</summary>
    public int Day { get; private set; }

    /// <summary>行を作った日時(UTC)。データベースの既定値で入る。</summary>
    public DateTime CreatedAt { get; private set; }
}
