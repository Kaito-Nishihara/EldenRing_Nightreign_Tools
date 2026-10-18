using System.Net;
using System.Text.Json;

namespace NightreignTools.Relic.Tests.Integration;

/// <summary>
/// 遺物の効果の一覧の API(ビルドシミュレーター 4 章)。
/// </summary>
public class RelicEffectEndpointsTests(RelicServiceFactory factory) : IClassFixture<RelicServiceFactory>
{
    [Fact]
    public async Task AC_BLD_01_relic_effectsはV11の効果をすべて返し各効果のranksはランクの小さい順である()
    {
        var effects = await GetArrayAsync("/api/v1/relic-effects");

        Assert.Equal(75, effects.Count);
        Assert.Equal(121, effects.Sum(e => e.GetProperty("ranks").GetArrayLength()));
        Assert.All(effects, e =>
        {
            var ranks = e.GetProperty("ranks").EnumerateArray().Select(r => r.GetProperty("rank").GetInt32()).ToList();
            Assert.Equal(ranks.Order(), ranks);
        });
    }

    [Fact]
    public async Task 効果は分類の並び順と効果の識別子の順に返る()
    {
        var effects = await GetArrayAsync("/api/v1/relic-effects");

        var keys = effects.Select(e => (Sort: e.GetProperty("categorySortOrder").GetInt32(), Id: e.GetProperty("id").GetInt32())).ToList();
        Assert.Equal(keys.OrderBy(k => k.Sort).ThenBy(k => k.Id), keys);
        Assert.Equal("stat", effects[0].GetProperty("categoryId").GetString());
        Assert.Equal("other", effects[^1].GetProperty("categoryId").GetString());
    }

    [Fact]
    public async Task 物理攻撃力上昇の深層効果は仕様の例の形で返る()
    {
        var effects = await GetArrayAsync("/api/v1/relic-effects");

        var effect = effects.Single(e => e.GetProperty("nameJa").GetString() == "物理攻撃力上昇+0〜4（深層）");
        Assert.Equal("attack", effect.GetProperty("categoryId").GetString());
        Assert.Equal("攻撃力", effect.GetProperty("categoryNameJa").GetString());
        Assert.Equal(1, effect.GetProperty("categorySortOrder").GetInt32());
        Assert.True(effect.GetProperty("isDeep").GetBoolean());
        Assert.Equal("positive", effect.GetProperty("effectSign").GetString());
        Assert.Equal("percent_add", effect.GetProperty("calcType").GetString());
        Assert.Equal("physical", effect.GetProperty("attribute").GetString());
        Assert.Equal("yes", effect.GetProperty("stackable").GetString());
        Assert.Equal((0, 4), (effect.GetProperty("rankMin").GetInt32(), effect.GetProperty("rankMax").GetInt32()));
        Assert.Equal(JsonValueKind.Null, effect.GetProperty("conditionJa").ValueKind);

        var rank3 = effect.GetProperty("ranks")[3];
        Assert.Equal(3, rank3.GetProperty("rank").GetInt32());
        Assert.Equal(10.5m, rank3.GetProperty("value").GetDecimal());
        Assert.Equal("percent", rank3.GetProperty("unit").GetString());
        Assert.True(rank3.GetProperty("hasDemerit").GetBoolean());
    }

    [Fact]
    public async Task 数で表せない効果の値はnullで返る()
    {
        var effects = await GetArrayAsync("/api/v1/relic-effects");

        var regain = effects.Single(e => e.GetProperty("nameJa").GetString() == "リゲイン有効化");
        var rank = Assert.Single(regain.GetProperty("ranks").EnumerateArray());
        Assert.Equal(JsonValueKind.Null, rank.GetProperty("value").ValueKind);
        Assert.Equal("none", rank.GetProperty("unit").GetString());
    }

    [Fact]
    public async Task AC_API_03_キーなしで呼ぶと401が返る()
    {
        var response = await factory.CreateClient().GetAsync("/api/v1/relic-effects");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private async Task<List<JsonElement>> GetArrayAsync(string path)
    {
        var response = await factory.CreateClientWithServiceKey().GetAsync(path);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return document.RootElement.EnumerateArray().Select(e => e.Clone()).ToList();
    }
}
