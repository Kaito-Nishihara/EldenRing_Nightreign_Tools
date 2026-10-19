using NightreignTools.SharedKernel;

namespace NightreignTools.Record.Domain.Entities;

/// <summary>
/// 1 つの記録の中の、日と段階ごとの収縮先(プレイ記録 5 章)。
/// 第 1 次(phase=1)は収縮候補地(ZoneId)だけ、第 2 次(phase=2)は第二収縮地点(SecondShrinkPointId)だけを持つ(決定の記録 0008)。
/// マスタの行は master-service が持つので、識別子だけを持つ。
/// </summary>
public sealed class ShrinkEntry
{
    public const int FirstPhase = 1;
    public const int SecondPhase = 2;

    private ShrinkEntry()
    {
    }

    /// <summary>
    /// 識別子。
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// この収縮先を持つ記録の識別子。
    /// </summary>
    public int PlayRecordId { get; private set; }

    /// <summary>
    /// 日。
    /// 1 か 2。
    /// </summary>
    public int Day { get; private set; }

    /// <summary>
    /// 段階。
    /// 1 か 2。
    /// </summary>
    public int Phase { get; private set; }

    /// <summary>
    /// 第 1 次の収縮先の収縮候補地の識別子。
    /// 第 2 次では null。
    /// </summary>
    public int? ZoneId { get; private set; }

    /// <summary>
    /// 第 2 次の収縮先の第二収縮地点の識別子。
    /// 第 1 次では null。
    /// </summary>
    public int? SecondShrinkPointId { get; private set; }

    /// <summary>
    /// 行を作った日時(UTC)。
    /// データベースの既定値で入る。
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// 規則を確かめて作る。
    /// 記録の中での重複は <see cref="PlayRecord"/> が確かめる。
    /// </summary>
    /// <param name="target">作る収縮先。</param>
    /// <returns>記録に付ける前の収縮先。</returns>
    internal static ShrinkEntry Create(ShrinkTarget target)
    {
        if (target.Day is not (1 or 2))
        {
            throw new BusinessRuleException($"day は 1 か 2 です(day={target.Day})");
        }

        if (target.Phase is not (FirstPhase or SecondPhase))
        {
            throw new BusinessRuleException($"phase は 1 か 2 です(phase={target.Phase})");
        }

        // 段階と収縮先の種類が合うこと(プレイ記録 4 章)
        if (target.Phase == FirstPhase && (target.ZoneId is null || target.SecondShrinkPointId is not null))
        {
            throw new BusinessRuleException($"day={target.Day}, phase=1 は zoneId で指定してください");
        }

        if (target.Phase == SecondPhase && (target.SecondShrinkPointId is null || target.ZoneId is not null))
        {
            throw new BusinessRuleException($"day={target.Day}, phase=2 は secondShrinkPointId で指定してください");
        }

        return new ShrinkEntry
        {
            Day = target.Day,
            Phase = target.Phase,
            ZoneId = target.ZoneId,
            SecondShrinkPointId = target.SecondShrinkPointId,
        };
    }
}

/// <summary>
/// 記録を作るときに渡す、1 つの収縮先。
/// </summary>
/// <param name="Day">日。1 か 2。</param>
/// <param name="Phase">段階。1 か 2。</param>
/// <param name="ZoneId">第 1 次の収縮候補地の識別子。第 2 次では null。</param>
/// <param name="SecondShrinkPointId">第 2 次の第二収縮地点の識別子。第 1 次では null。</param>
public readonly record struct ShrinkTarget(int Day, int Phase, int? ZoneId, int? SecondShrinkPointId);
