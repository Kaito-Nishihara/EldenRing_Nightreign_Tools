namespace NightreignTools.Master.Tests.Integration;

/// <summary>
/// 起動の時点で、必要な設定がないことに気付いて止まる(サービスの共通構成 5.7)。
/// </summary>
public class StartupTests
{
    [Fact]
    public void AC_SVC_01_SERVICE_KEYを渡さずに起動すると起動が失敗し文言にSERVICE_KEYが出る()
    {
        using var factory = new FactoryWithoutServiceKey();

        var exception = Record.Exception(() => factory.CreateClient());

        Assert.NotNull(exception);
        Assert.Contains("SERVICE_KEY", exception.ToString());
    }

    private sealed class FactoryWithoutServiceKey : MasterServiceFactory
    {
        protected override string ConfiguredServiceKey => "";
    }
}
