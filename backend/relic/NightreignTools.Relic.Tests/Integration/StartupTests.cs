namespace NightreignTools.Relic.Tests.Integration;

/// <summary>
/// 起動の時点で、必要な設定がないことに気付いて止まる(サービスの共通構成 5.7)。
/// </summary>
public class StartupTests
{
    [Fact]
    public void AC_SVC_01_SERVICE_KEYを渡さずに起動すると起動が失敗し文言にSERVICE_KEYが出る()
    {
        using var factory = new FactoryWithoutServiceKey();

        var exception = Xunit.Record.Exception(() => factory.CreateClient());

        Assert.NotNull(exception);
        Assert.Contains("SERVICE_KEY", exception.ToString());
    }

    [Fact]
    public void AC_SVC_02_接続文字列を渡さずに起動すると起動が失敗する()
    {
        using var factory = new FactoryWithoutConnectionString();

        var exception = Xunit.Record.Exception(() => factory.CreateClient());

        Assert.NotNull(exception);
        Assert.Contains("ConnectionStrings__Default", exception.ToString());
    }

    [Fact]
    public async Task AC_API_09_healthはキーなしで200を返す()
    {
        using var factory = new RelicServiceFactory();

        var response = await factory.CreateClient().GetAsync("/health");

        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    private sealed class FactoryWithoutServiceKey : RelicServiceFactory
    {
        protected override string ConfiguredServiceKey => "";
    }

    private sealed class FactoryWithoutConnectionString : RelicServiceFactory
    {
        protected override string ConfiguredConnectionString => "";
    }
}
