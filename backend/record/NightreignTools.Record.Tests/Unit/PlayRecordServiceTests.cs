using NightreignTools.Record.Application.Masters;
using NightreignTools.Record.Application.PlayRecords;
using NightreignTools.Record.Domain.Entities;
using NightreignTools.SharedKernel;

namespace NightreignTools.Record.Tests.Unit;

/// <summary>
/// 記録の保存のアプリケーションサービス(プレイ記録 4 章、6 章)。master-service とリポジトリは手書きの偽物に差し替える。
/// </summary>
public class PlayRecordServiceTests
{
    private readonly FakeMasterDirectory _masters = new();
    private readonly FakeRepository _repository = new();

    private PlayRecordService CreateService() => new(_masters, _repository);

    private static CreatePlayRecordRequest Request(
        string bossId = "gladius",
        string? jihenId = null,
        int? day1NightBossId = 3,
        int? day2NightBossId = null,
        IReadOnlyList<ShrinkEntryRequest>? entries = null) =>
        new(bossId, jihenId, 1, day1NightBossId, day2NightBossId, Note: null, entries ??
        [
            new ShrinkEntryRequest(1, 1, ZoneId: 4, SecondShrinkPointId: null),
            new ShrinkEntryRequest(1, 2, ZoneId: null, SecondShrinkPointId: 4),
            new ShrinkEntryRequest(2, 1, ZoneId: 7, SecondShrinkPointId: null),
        ]);

    [Fact]
    public async Task AC_REC_01_参照がすべてあれば保存し識別子を返す()
    {
        var response = await CreateService().CreateAsync(Request(), CancellationToken.None);

        var saved = Assert.Single(_repository.Saved);
        Assert.Equal(3, saved.Entries.Count);
        Assert.Equal(FakeRepository.SavedId, response.Id);
    }

    [Fact]
    public async Task master_serviceへの問い合わせは1回で記録の参照する識別子をまとめて送る()
    {
        await CreateService().CreateAsync(Request(jihenId: "crater"), CancellationToken.None);

        var query = Assert.Single(_masters.Queries);
        Assert.Equal(["gladius"], query.BossIds);
        Assert.Equal(["crater"], query.JihenIds);
        Assert.Equal([1], query.SpawnPointIds);
        Assert.Equal([4, 7], query.ZoneIds);
        Assert.Equal([4], query.SecondShrinkPointIds);
        Assert.Equal([3], query.NightBossIds);
    }

    [Fact]
    public async Task 記録の中の規則に反するときはmaster_serviceを呼ばない()
    {
        var duplicated = Request(entries: [new(1, 1, 4, null), new(1, 1, 7, null)]);

        await Assert.ThrowsAsync<BusinessRuleException>(() => CreateService().CreateAsync(duplicated, CancellationToken.None));

        Assert.Empty(_masters.Queries);
        Assert.Empty(_repository.Saved);
    }

    [Fact]
    public async Task AC_REC_04_存在しない収縮候補地は404で保存しない()
    {
        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            CreateService().CreateAsync(Request(entries: [new(1, 1, 99, null)]), CancellationToken.None));

        Assert.Equal("zoneId '99' は存在しません", exception.Message);
        Assert.Empty(_repository.Saved);
    }

    [Fact]
    public async Task AC_REC_05_存在しない地変は404()
    {
        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            CreateService().CreateAsync(Request(jihenId: "unknown"), CancellationToken.None));

        Assert.Equal("jihenId 'unknown' は存在しません", exception.Message);
    }

    [Fact]
    public async Task 存在しない夜の王は404()
    {
        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            CreateService().CreateAsync(Request(bossId: "unknown"), CancellationToken.None));

        Assert.Equal("bossId 'unknown' は存在しません", exception.Message);
    }

    [Fact]
    public async Task AC_REC_18_存在しない第二収縮地点は404で保存しない()
    {
        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            CreateService().CreateAsync(Request(entries: [new(1, 2, null, 99)]), CancellationToken.None));

        Assert.Equal("secondShrinkPointId '99' は存在しません", exception.Message);
        Assert.Empty(_repository.Saved);
    }

    [Fact]
    public async Task AC_REC_24_非表示の収縮候補地は400で保存しない()
    {
        _masters.HiddenZoneIds.Add(4);

        var exception = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            CreateService().CreateAsync(Request(), CancellationToken.None));

        Assert.Equal("zoneId '4' は非表示のため、記録に使えません", exception.Message);
        Assert.Empty(_repository.Saved);
    }

    [Fact]
    public async Task AC_REC_19_1日目に2日目の夜ボスを指定すると400()
    {
        var exception = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            CreateService().CreateAsync(Request(day1NightBossId: 5), CancellationToken.None));

        Assert.Equal("day1NightBossId には 1 日目の夜ボスを指定してください", exception.Message);
    }

    [Fact]
    public async Task AC_REC_23_master_serviceが応答しないと503の例外をそのまま伝え保存しない()
    {
        _masters.Unavailable = true;

        await Assert.ThrowsAsync<ServiceUnavailableException>(() => CreateService().CreateAsync(Request(), CancellationToken.None));

        Assert.Empty(_repository.Saved);
    }

    private sealed class FakeRepository : IPlayRecordRepository
    {
        public const int SavedId = 0;

        public List<PlayRecord> Saved { get; } = [];

        public Task AddAsync(PlayRecord record, CancellationToken ct)
        {
            Saved.Add(record);
            return Task.CompletedTask;
        }
    }
}
