namespace NightreignTools.Relic.Domain.Entities;

/// <summary>遺物の効果の分類。例: 能力値、攻撃力。マイグレーションの初期データだけで管理する。</summary>
public sealed class RelicEffectCategory
{
    /// <summary>識別子。英小文字の固定の値。例: attack。</summary>
    public string Id { get; private set; } = "";

    /// <summary>日本語の名前。</summary>
    public string NameJa { get; private set; } = "";

    /// <summary>画面での並び順。小さいほうが先(ビルドシミュレーター 3.2)。</summary>
    public int SortOrder { get; private set; }

    /// <summary>行を作った日時(UTC)。データベースの既定値で入る。</summary>
    public DateTime CreatedAt { get; private set; }
}
