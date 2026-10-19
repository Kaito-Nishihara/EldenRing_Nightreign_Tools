using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NightreignTools.Relic.Domain.Entities;
using NightreignTools.Relic.Infrastructure.Persistence;

namespace NightreignTools.Relic.Tests.Integration;

/// <summary>
/// マイグレーションに含めた初期データ(データベースの共通仕様 5.6)。旧版の V11 の件数と値に合っていることを確かめる。
/// API から見た初期データ(AC-BLD-01)は、遺物の効果の API のテストで確かめる。
/// </summary>
/// <param name="factory">テストのために起動した relic-service。</param>
public class SeedDataTests(RelicServiceFactory factory) : IClassFixture<RelicServiceFactory>
{
    [Fact]
    public async Task 初期データの件数が旧版のV11と一致する()
    {
        await using var scope = CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<RelicDbContext>();

        Assert.Equal(8, await db.RelicEffectCategories.CountAsync());
        Assert.Equal(75, await db.RelicEffects.CountAsync());
        Assert.Equal(22, await db.RelicEffects.CountAsync(e => e.IsDeep));
        Assert.Equal(13, await db.RelicEffects.CountAsync(e => e.EffectSign == RelicEffectSigns.Negative));
        Assert.Equal(121, await db.RelicEffectRanks.CountAsync());
    }

    [Fact]
    public async Task 物理攻撃力上昇の深層効果はランク3からデメリットが付く()
    {
        await using var scope = CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<RelicDbContext>();

        var effect = await db.RelicEffects.AsNoTracking().Include(e => e.Ranks)
            .SingleAsync(e => e.NameJa == "物理攻撃力上昇+0〜4（深層）");

        Assert.True(effect.IsDeep);
        Assert.Equal((0, 4), (effect.RankMin, effect.RankMax));
        var ranks = effect.Ranks.OrderBy(r => r.Rank).ToList();
        Assert.Equal([4m, 5m, 6m, 10.5m, 12m], ranks.Select(r => r.Value));
        Assert.Equal([false, false, false, true, true], ranks.Select(r => r.HasDemerit));
    }

    [Fact]
    public async Task ランクのある効果はランクの下限から上限までの値を1行ずつ持つ()
    {
        await using var scope = CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<RelicDbContext>();

        var effects = await db.RelicEffects.AsNoTracking().Include(e => e.Ranks).ToListAsync();

        Assert.All(effects, e => Assert.Equal(
            Enumerable.Range(e.RankMin, e.RankMax - e.RankMin + 1),
            e.Ranks.Select(r => r.Rank).Order()));
    }

    [Fact]
    public async Task 数で表せない効果の値はnullで単位はnoneである()
    {
        await using var scope = CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<RelicDbContext>();

        var effect = await db.RelicEffects.AsNoTracking().Include(e => e.Ranks).SingleAsync(e => e.NameJa == "リゲイン有効化");
        var rank = Assert.Single(effect.Ranks);

        Assert.Null(rank.Value);
        Assert.Equal(RelicValueUnits.None, rank.Unit);
    }

    [Theory]
    [InlineData("relic_effects_id_seq")]
    [InlineData("relic_effect_ranks_id_seq")]
    public async Task 初期データの後に足す行の識別子は1001から振られる(string sequence)
    {
        await using var scope = CreateScope();

        var start = await TestDatabase.QueryAsync(
            $"SELECT start_value FROM pg_sequences WHERE schemaname = 'game' AND sequencename = '{sequence}'");

        Assert.Equal(["1001"], start);
    }

    private AsyncServiceScope CreateScope()
    {
        factory.CreateClient();
        return factory.Services.CreateAsyncScope();
    }
}
