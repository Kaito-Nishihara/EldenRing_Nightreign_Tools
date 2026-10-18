namespace NightreignTools.Record.Tests.Integration;

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

    [Theory]
    [InlineData("")]
    [InlineData("master-service:8081")]
    public void AC_SVC_10_MASTER_SERVICE_URLを渡さずに起動すると起動が失敗し文言にMASTER_SERVICE_URLが出る(string url)
    {
        using var factory = new FactoryWithMasterServiceUrl(url);

        var exception = Xunit.Record.Exception(() => factory.CreateClient());

        Assert.NotNull(exception);
        Assert.Contains("MASTER_SERVICE_URL", exception.ToString());
    }

    [Fact]
    public async Task AC_API_09_healthはキーなしで200を返す()
    {
        using var factory = new RecordServiceFactory();

        var response = await factory.CreateClient().GetAsync("/health");

        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    private sealed class FactoryWithoutServiceKey : RecordServiceFactory
    {
        protected override string ConfiguredServiceKey => "";
    }

    private sealed class FactoryWithoutConnectionString : RecordServiceFactory
    {
        protected override string ConfiguredConnectionString => "";
    }

    private sealed class FactoryWithMasterServiceUrl(string url) : RecordServiceFactory
    {
        protected override string ConfiguredMasterServiceUrl => url;
    }
}
