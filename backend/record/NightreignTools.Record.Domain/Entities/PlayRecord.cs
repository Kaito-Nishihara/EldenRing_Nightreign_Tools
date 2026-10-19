using NightreignTools.SharedKernel;

namespace NightreignTools.Record.Domain.Entities;

/// <summary>
/// 1 回のプレイの記録(プレイ記録 5 章)。作った後は直さず、消さない(決定の記録 0005)。
/// 夜の王、地変、開始地点、夜ボスは master-service の行の識別子だけを持つ。あるかどうかはアプリケーションサービスが確かめる。
/// </summary>
public sealed class PlayRecord
{
    public const int NoteMaxLength = 500;
    public const int MinEntries = 1;
    public const int MaxEntries = 4;

    private readonly List<ShrinkEntry> _entries = [];

    private PlayRecord()
    {
    }

    /// <summary>
    /// 識別子。
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// 夜の王の識別子。
    /// master-service の bosses の id。
    /// </summary>
    public string BossId { get; private set; } = "";

    /// <summary>
    /// 地変の識別子。
    /// null は地変なし。
    /// </summary>
    public string? JihenId { get; private set; }

    /// <summary>
    /// 開始地点の識別子。
    /// master-service の spawn_points の id。
    /// </summary>
    public int SpawnPointId { get; private set; }

    /// <summary>
    /// 1 日目の夜ボスの識別子。
    /// null は選ばなかった。
    /// </summary>
    public int? Day1NightBossId { get; private set; }

    /// <summary>
    /// 2 日目の夜ボスの識別子。
    /// null は選ばなかった。
    /// </summary>
    public int? Day2NightBossId { get; private set; }

    /// <summary>
    /// 備考。
    /// 前後の空白を除いたもの。
    /// 空なら null。
    /// </summary>
    public string? Note { get; private set; }

    /// <summary>
    /// 記録を保存した日時(UTC)。
    /// データベースの既定値で入る。
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// 収縮先。
    /// 日、段階の順に並べる。
    /// </summary>
    public IReadOnlyList<ShrinkEntry> Entries => _entries;

    /// <summary>
    /// 記録の中だけで確かめられる規則(収縮先の数、日と段階の組の重複、段階と収縮先の種類、備考の長さ)を確かめて作る。
    /// 規則に反するときは <see cref="BusinessRuleException"/> を投げる。
    /// </summary>
    /// <param name="bossId">夜の王の識別子。空白だけは許さない。</param>
    /// <param name="jihenId">地変の識別子。null は地変なし。</param>
    /// <param name="spawnPointId">開始地点の識別子。</param>
    /// <param name="day1NightBossId">1 日目の夜ボスの識別子。null は記録しない。</param>
    /// <param name="day2NightBossId">2 日目の夜ボスの識別子。null は記録しない。</param>
    /// <param name="note">備考。前後の空白を除いて 500 文字以内。空白だけなら null にする。</param>
    /// <param name="entries">収縮先。1 件以上 4 件以下で、日と段階の組は重ならない。</param>
    /// <returns>保存の前の記録。Id と CreatedAt は保存の後に入る。</returns>
    public static PlayRecord Create(
        string bossId,
        string? jihenId,
        int spawnPointId,
        int? day1NightBossId,
        int? day2NightBossId,
        string? note,
        IReadOnlyCollection<ShrinkTarget> entries)
    {
        if (string.IsNullOrWhiteSpace(bossId))
        {
            throw new BusinessRuleException("bossId は必須です");
        }

        if (entries.Count is < MinEntries or > MaxEntries)
        {
            throw new BusinessRuleException($"entries は {MinEntries} 件以上 {MaxEntries} 件以下です");
        }

        var duplicate = entries
            .GroupBy(e => (e.Day, e.Phase))
            .FirstOrDefault(g => g.Count() > 1);
        if (duplicate is not null)
        {
            throw new BusinessRuleException($"day={duplicate.Key.Day}, phase={duplicate.Key.Phase} が重複しています");
        }

        var trimmedNote = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        if (trimmedNote is { Length: > NoteMaxLength })
        {
            throw new BusinessRuleException($"note は {NoteMaxLength} 文字以内です");
        }

        var record = new PlayRecord
        {
            BossId = bossId,
            JihenId = string.IsNullOrWhiteSpace(jihenId) ? null : jihenId,
            SpawnPointId = spawnPointId,
            Day1NightBossId = day1NightBossId,
            Day2NightBossId = day2NightBossId,
            Note = trimmedNote,
        };
        record._entries.AddRange(entries
            .OrderBy(e => e.Day)
            .ThenBy(e => e.Phase)
            .Select(ShrinkEntry.Create));
        return record;
    }
}
