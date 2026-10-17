namespace NightreignTools.Master.Infrastructure.Persistence.Seed;

/// <summary>
/// 初期データの共通の値(データベースの共通仕様 5.6)。
/// </summary>
internal static class SeedData
{
    /// <summary>
    /// 初期データの created_at。決まった値にし、マイグレーションを作り直しても差分が出ないようにする。
    /// </summary>
    public static readonly DateTime CreatedAt = new(2026, 10, 17, 0, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// 初期データの整数の識別子はこの値以下とする。管理の画面から足した行は、この次の値から振る。
    /// </summary>
    public const int IdLimit = 1000;
}
