namespace NightreignTools.Record.Application.Patterns;

/// <summary>
/// パターン検索(パターン検索 4 章)。
/// </summary>
public interface IPatternSearchService
{
    Task<PatternResponse> SearchAsync(PatternSearchRequest request, CancellationToken ct);
}
