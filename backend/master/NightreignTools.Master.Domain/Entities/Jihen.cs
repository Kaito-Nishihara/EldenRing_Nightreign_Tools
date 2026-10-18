namespace NightreignTools.Master.Domain.Entities;

/// <summary>地変。地変ごとにマップ画像が替わる。中心の座標は 1024 四方の座標空間の数。</summary>
public sealed class Jihen
{
    /// <summary>識別子。英小文字の固定の値。例: crater。</summary>
    public string Id { get; private set; } = "";

    /// <summary>日本語の名前。</summary>
    public string NameJa { get; private set; } = "";

    /// <summary>地変のマップ画像のパス。null は画像なし。</summary>
    public string? ImagePath { get; private set; }

    /// <summary>地変が起きる方角。例: north、center、all。</summary>
    public string Area { get; private set; } = "";

    /// <summary>地変の中心の x 座標。1024 四方の座標空間の数。</summary>
    public decimal CenterX { get; private set; }

    /// <summary>地変の中心の y 座標。1024 四方の座標空間の数。</summary>
    public decimal CenterY { get; private set; }

    /// <summary>ダウンロードコンテンツで足された地変か。</summary>
    public bool IsDlc { get; private set; }

    /// <summary>起きるようになる条件。null は最初から起きる。</summary>
    public string? UnlockCondition { get; private set; }

    /// <summary>画面の選択の欄での並び順。1 から(データベースの共通仕様 5.6)。</summary>
    public int SortOrder { get; private set; }

    /// <summary>行を作った日時(UTC)。データベースの既定値で入る。</summary>
    public DateTime CreatedAt { get; private set; }
}
