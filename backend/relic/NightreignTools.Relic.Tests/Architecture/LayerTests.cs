using System.Reflection;
using NightreignTools.Relic.Application;
using NightreignTools.Relic.Domain;

namespace NightreignTools.Relic.Tests.Architecture;

/// <summary>
/// クリーンアーキテクチャの層の向き(サービスの共通構成 5.3、決定の記録 0011)。
/// Domain と Application が、EF Core と ASP.NET Core を知らないことを、参照するアセンブリで確かめる。
/// </summary>
public class LayerTests
{
    private static readonly string[] ForbiddenPrefixes = ["Microsoft.EntityFrameworkCore", "Microsoft.AspNetCore", "Npgsql"];

    public static TheoryData<string> InnerLayers => new()
    {
        typeof(DomainAssembly).Assembly.GetName().Name!,
        typeof(DependencyInjection).Assembly.GetName().Name!,
    };

    [Theory]
    [MemberData(nameof(InnerLayers))]
    public void AC_SVC_06_内側の層はEFCoreとASPNETCoreを参照しない(string assemblyName)
    {
        var assembly = Assembly.Load(assemblyName);

        var forbidden = assembly.GetReferencedAssemblies()
            .Select(a => a.Name ?? "")
            .Where(name => ForbiddenPrefixes.Any(prefix => name.StartsWith(prefix, StringComparison.Ordinal)))
            .ToList();

        Assert.Empty(forbidden);
    }
}
