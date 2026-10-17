namespace NightreignTools.Master.Domain.Entities;

/// <summary>夜の王(3 日目に戦う相手)。識別子は英小文字の固定の値。マイグレーションの初期データだけで管理する。</summary>
public sealed class Boss
{
    private readonly List<BossWeakness> _weaknesses = [];

    public string Id { get; private set; } = "";
    public string NameJa { get; private set; } = "";
    public string? NameEn { get; private set; }
    public string? ImagePath { get; private set; }
    public bool IsDlc { get; private set; }
    public string? UnlockCondition { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public IReadOnlyList<BossWeakness> Weaknesses => _weaknesses;
}
