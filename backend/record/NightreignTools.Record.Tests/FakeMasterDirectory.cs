using NightreignTools.Record.Application.Masters;
using NightreignTools.SharedKernel;

namespace NightreignTools.Record.Tests;

/// <summary>
/// master-service の偽物(サービスの共通構成 5.9)。単体のテストと、API のテストで共有する。
/// 夜の王 gladius、地変 crater、開始地点 1 から 3(3 には記録を保存しない)、収縮候補地 4 と 7、第二収縮地点 4、
/// 1 日目の夜ボス 3、2 日目の夜ボス 5 を持つ。問い合わせられた識別子のうち、持つものだけを返す。
/// </summary>
internal sealed class FakeMasterDirectory : IMasterDirectory
{
    public List<MasterReferenceQuery> Queries { get; } = [];

    /// <summary>非表示にした収縮候補地。</summary>
    public HashSet<int> HiddenZoneIds { get; } = [];

    /// <summary>4 のほかに持たせる第二収縮地点。</summary>
    public HashSet<int> ExtraPointIds { get; } = [];

    /// <summary>true にすると、master-service が応答しないときと同じ例外を投げる。</summary>
    public bool Unavailable { get; set; }

    public Task<MasterReferences> ResolveAsync(MasterReferenceQuery query, CancellationToken ct)
    {
        Queries.Add(query);
        if (Unavailable)
        {
            throw new ServiceUnavailableException("マスタのサービスに接続できません。時間をおいてやり直してください");
        }

        return Task.FromResult(new MasterReferences(
            query.BossIds.Where(id => id == "gladius").Select(id => new MasterBoss(id, "三つ首の獣")).ToList(),
            query.JihenIds.Where(id => id == "crater").Select(id => new MasterJihen(id, "山嶺")).ToList(),
            query.SpawnPointIds.Where(id => id is >= 1 and <= 3).Select(id => new MasterPoint(id, $"教会{id}", 351, 387, false)).ToList(),
            query.ZoneIds.Where(id => id is 4 or 7).Select(id => new MasterZone(id, $"候補{id}", 500, 500, 255, HiddenZoneIds.Contains(id))).ToList(),
            query.SecondShrinkPointIds.Where(id => id == 4 || ExtraPointIds.Contains(id))
                .Select(id => new MasterPoint(id, id == 4 ? "最北" : $"地点{id}", 583, 297, false)).ToList(),
            query.NightBossIds.Where(id => id is 3 or 5).Select(id => new MasterNightBoss(id, $"夜ボス{id}", id == 3 ? 1 : 2)).ToList()));
    }
}
