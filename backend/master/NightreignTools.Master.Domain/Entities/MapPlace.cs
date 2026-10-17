namespace NightreignTools.Master.Domain.Entities;

/// <summary>
/// 管理の画面で座標を登録し、直す、マップの上の場所の基底(開始地点、収縮候補地、第二収縮地点、拠点候補地)。
/// 要らなくなった行は消さずに非表示にする(決定の記録 0013)。
/// </summary>
public abstract class MapPlace
{
    public int Id { get; private set; }

    /// <summary>非表示にした日時。null は表示中。</summary>
    public DateTime? HiddenAt { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public bool IsHidden => HiddenAt is not null;
}
