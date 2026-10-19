namespace NightreignTools.Master.Domain.Entities;

/// <summary>
/// 夜の王(3 日目に戦う相手)。
/// 識別子は英小文字の固定の値。
/// マイグレーションの初期データだけで管理する。
/// </summary>
public sealed class Boss
{
    private readonly List<BossWeakness> _weaknesses = [];

    /// <summary>
    /// 識別子。
    /// 英小文字の固定の値。
    /// 例: gladius。
    /// </summary>
    public string Id { get; private set; } = "";

    /// <summary>
    /// 日本語の名前。
    /// </summary>
    public string NameJa { get; private set; } = "";

    /// <summary>
    /// 英語の名前。
    /// </summary>
    public string? NameEn { get; private set; }

    /// <summary>
    /// 画像のパス。
    /// null は画像なし。
    /// </summary>
    public string? ImagePath { get; private set; }

    /// <summary>
    /// ダウンロードコンテンツで足された夜の王か。
    /// </summary>
    public bool IsDlc { get; private set; }

    /// <summary>
    /// 戦えるようになる条件。
    /// null は最初から戦える。
    /// </summary>
    public string? UnlockCondition { get; private set; }

    /// <summary>
    /// 画面の選択の欄での並び順。
    /// 1 から(データベースの共通仕様 5.6)。
    /// </summary>
    public int SortOrder { get; private set; }

    /// <summary>
    /// 行を作った日時(UTC)。
    /// データベースの既定値で入る。
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    public IReadOnlyList<BossWeakness> Weaknesses => _weaknesses;
}
