namespace NightreignTools.Master.Domain.Entities;

/// <summary>地変。地変ごとにマップ画像が替わる。中心の座標は 1024 四方の座標空間の数。</summary>
public sealed class Jihen
{
    public string Id { get; private set; } = "";
    public string NameJa { get; private set; } = "";
    public string? ImagePath { get; private set; }
    public string Area { get; private set; } = "";
    public decimal CenterX { get; private set; }
    public decimal CenterY { get; private set; }
    public bool IsDlc { get; private set; }
    public string? UnlockCondition { get; private set; }

    /// <summary>画面の選択の欄での並び順。1 から(データベースの共通仕様 5.6)。</summary>
    public int SortOrder { get; private set; }

    public DateTime CreatedAt { get; private set; }
}
