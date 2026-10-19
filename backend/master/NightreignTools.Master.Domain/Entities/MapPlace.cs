using NightreignTools.SharedKernel;

namespace NightreignTools.Master.Domain.Entities;

/// <summary>
/// 管理の画面で座標を登録し、直す、マップの上の場所の基底(開始地点、収縮候補地、第二収縮地点、拠点候補地)。
/// 要らなくなった行は消さずに非表示にする(決定の記録 0013)。
/// 入力の決まり(マスタの管理 3.4)はここにまとめ、4 つの種類で同じにする。
/// </summary>
public abstract class MapPlace
{
    public const int NameMaxLength = 100;
    public const decimal CoordinateMin = 0m;
    public const decimal CoordinateMax = 1024m;
    public const decimal RadiusMin = 10m;
    public const decimal RadiusMax = 512m;

    /// <summary>
    /// 座標と半径の小数の桁。
    /// 列は numeric(7,2) である
    /// </summary>
    private const int Scale = 2;

    public const string NameRequiredMessage = "名前を入れてください";
    public const string CoordinateMessage = "0 から 1024 の数を入れてください";
    public const string RadiusMessage = "10 から 512 の数を入れてください";

    /// <summary>
    /// 識別子。
    /// 初期データは 1000 以下、管理の画面で足した行は 1001 から。
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// 非表示にした日時。
    /// null は表示中。
    /// </summary>
    public DateTime? HiddenAt { get; private set; }

    /// <summary>
    /// 行を作った日時(UTC)。
    /// データベースの既定値で入る。
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    public bool IsHidden => HiddenAt is not null;

    /// <summary>
    /// 非表示にするか、表示に戻す。すでにその状態なら何もしない。
    /// 非表示にした日時は、初めて非表示にした時のまま残す。
    /// </summary>
    /// <param name="hidden">true なら非表示にし、false なら表示に戻す。</param>
    /// <param name="nowUtc">今の日時(UTC)。初めて非表示にするときだけ使う。</param>
    public void SetHidden(bool hidden, DateTime nowUtc)
    {
        if (hidden && HiddenAt is null)
        {
            HiddenAt = nowUtc;
        }
        else if (!hidden)
        {
            HiddenAt = null;
        }
    }

    /// <summary>
    /// 名前を確かめ、前後の空白を除いて返す。
    /// 必須の名前が空なら 400 にする
    /// </summary>
    /// <param name="name">入力の名前。</param>
    /// <returns>前後の空白を除いた名前。</returns>
    protected static string RequireName(string? name)
    {
        var trimmed = OptionalName(name);
        return trimmed ?? throw new BusinessRuleException(NameRequiredMessage);
    }

    /// <summary>
    /// 任意の名前を確かめる。
    /// 空なら null にする
    /// </summary>
    /// <param name="name">入力の名前。</param>
    /// <returns>前後の空白を除いた名前。空なら null。</returns>
    protected static string? OptionalName(string? name)
    {
        var trimmed = name?.Trim();
        if (string.IsNullOrEmpty(trimmed))
        {
            return null;
        }

        return trimmed.Length <= NameMaxLength
            ? trimmed
            : throw new BusinessRuleException($"名前は {NameMaxLength} 文字までです");
    }

    /// <summary>
    /// 座標を確かめる。
    /// 0 以上 1024 以下で、小数は 2 桁まで
    /// </summary>
    /// <param name="value">入力の座標。</param>
    /// <returns>受け取った値。</returns>
    protected static decimal RequireCoordinate(decimal value) =>
        InRange(value, CoordinateMin, CoordinateMax) ? value : throw new BusinessRuleException(CoordinateMessage);

    /// <summary>
    /// 収縮の円の半径を確かめる。
    /// 10 以上 512 以下で、小数は 2 桁まで
    /// </summary>
    /// <param name="value">入力の半径。</param>
    /// <returns>受け取った値。</returns>
    protected static decimal RequireRadius(decimal value) =>
        InRange(value, RadiusMin, RadiusMax) ? value : throw new BusinessRuleException(RadiusMessage);

    private static bool InRange(decimal value, decimal min, decimal max) =>
        value >= min && value <= max && decimal.Round(value, Scale) == value;
}
