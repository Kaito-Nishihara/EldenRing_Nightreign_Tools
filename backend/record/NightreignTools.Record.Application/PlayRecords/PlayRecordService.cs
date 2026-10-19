using NightreignTools.Record.Application.Masters;
using NightreignTools.Record.Domain.Entities;
using NightreignTools.SharedKernel;

namespace NightreignTools.Record.Application.PlayRecords;

/// <summary>
/// プレイ記録の保存(プレイ記録 4 章、6 章)。
/// 記録の中の規則を確かめてから、master-service に参照を 1 回だけ問い合わせ、ない行は 404、非表示の行は 400 にする。
/// どこかで失敗したときは何も保存しない。
/// </summary>
/// <param name="masters">master-service に参照を問い合わせる。</param>
/// <param name="repository">記録を保存する。</param>
internal sealed class PlayRecordService(IMasterDirectory masters, IPlayRecordRepository repository) : IPlayRecordService
{
    public async Task<CreatePlayRecordResponse> CreateAsync(CreatePlayRecordRequest request, CancellationToken ct)
    {
        // 記録の中だけで確かめられる規則を先に確かめる。破っていれば master-service を呼ばない
        var record = PlayRecord.Create(
            request.BossId,
            request.JihenId,
            request.SpawnPointId ?? throw new BusinessRuleException("spawnPointId は必須です"),
            request.Day1NightBossId,
            request.Day2NightBossId,
            request.Note,
            request.Entries.Select(e => new ShrinkTarget(e.Day, e.Phase, e.ZoneId, e.SecondShrinkPointId)).ToList());

        var references = await masters.ResolveAsync(QueryFor(record), ct);
        EnsureReferences(record, references);

        await repository.AddAsync(record, ct);
        return new CreatePlayRecordResponse(record.Id, record.CreatedAt);
    }

    /// <summary>
    /// 記録が参照するマスタの識別子をまとめる。
    /// </summary>
    /// <param name="record">参照を引く記録。</param>
    /// <returns>記録が参照する識別子。使っていない種類は空。</returns>
    private static MasterReferenceQuery QueryFor(PlayRecord record) => new(
        BossIds: [record.BossId],
        JihenIds: record.JihenId is null ? [] : [record.JihenId],
        SpawnPointIds: [record.SpawnPointId],
        ZoneIds: record.Entries.Where(e => e.ZoneId is not null).Select(e => e.ZoneId!.Value).Distinct().ToList(),
        SecondShrinkPointIds: record.Entries.Where(e => e.SecondShrinkPointId is not null).Select(e => e.SecondShrinkPointId!.Value).Distinct().ToList(),
        NightBossIds: new[] { record.Day1NightBossId, record.Day2NightBossId }.OfType<int>().Distinct().ToList());

    /// <summary>
    /// 参照する行があり、記録に使える状態かを確かめる。
    /// 入力の項目の順に確かめ、最初の誤りを投げる。
    /// </summary>
    /// <param name="record">確かめる記録。</param>
    /// <param name="found">master-service から引いた行。</param>
    private static void EnsureReferences(PlayRecord record, MasterReferences found)
    {
        RequireFound("bossId", record.BossId, found.Bosses.Any(b => b.Id == record.BossId));
        if (record.JihenId is not null)
        {
            RequireFound("jihenId", record.JihenId, found.Jihen.Any(j => j.Id == record.JihenId));
        }

        RequireUsable("spawnPointId", record.SpawnPointId, found.SpawnPoints.FirstOrDefault(p => p.Id == record.SpawnPointId)?.IsHidden);

        foreach (var entry in record.Entries)
        {
            if (entry.ZoneId is { } zoneId)
            {
                RequireUsable("zoneId", zoneId, found.Zones.FirstOrDefault(z => z.Id == zoneId)?.IsHidden);
            }

            if (entry.SecondShrinkPointId is { } pointId)
            {
                RequireUsable("secondShrinkPointId", pointId, found.SecondShrinkPoints.FirstOrDefault(p => p.Id == pointId)?.IsHidden);
            }
        }

        RequireNightBoss("day1NightBossId", record.Day1NightBossId, day: 1, found.NightBosses);
        RequireNightBoss("day2NightBossId", record.Day2NightBossId, day: 2, found.NightBosses);
    }

    private static void RequireFound<T>(string field, T id, bool exists)
    {
        if (!exists)
        {
            throw new NotFoundException($"{field} '{id}' は存在しません");
        }
    }

    /// <summary>
    /// 行があり、非表示でないこと。
    /// <paramref name="isHidden"/> が null なら行がない。
    /// </summary>
    /// <param name="field">例外の文言に入れる項目の名前。</param>
    /// <param name="id">確かめる識別子。</param>
    /// <param name="isHidden">行の非表示の状態。null は行がない。</param>
    private static void RequireUsable(string field, int id, bool? isHidden)
    {
        RequireFound(field, id, isHidden is not null);
        if (isHidden == true)
        {
            throw new BusinessRuleException($"{field} '{id}' は非表示のため、記録に使えません");
        }
    }

    /// <summary>
    /// 夜ボスがあり、その日の夜ボスであること。
    /// </summary>
    /// <param name="field">例外の文言に入れる項目の名前。</param>
    /// <param name="id">夜ボスの識別子。null なら確かめない。</param>
    /// <param name="day">夜ボスを指定した日。</param>
    /// <param name="nightBosses">master-service から引いた夜ボス。</param>
    private static void RequireNightBoss(string field, int? id, int day, IReadOnlyList<MasterNightBoss> nightBosses)
    {
        if (id is not { } nightBossId)
        {
            return;
        }

        var nightBoss = nightBosses.FirstOrDefault(n => n.Id == nightBossId);
        RequireFound(field, nightBossId, nightBoss is not null);
        if (nightBoss!.Day != day)
        {
            throw new BusinessRuleException($"{field} には {day} 日目の夜ボスを指定してください");
        }
    }
}
