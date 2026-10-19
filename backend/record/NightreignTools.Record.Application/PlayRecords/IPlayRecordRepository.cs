using NightreignTools.Record.Domain.Entities;

namespace NightreignTools.Record.Application.PlayRecords;

/// <summary>
/// プレイ記録の保存先のポート。
/// 実装は Infrastructure の EF Core のリポジトリ。
/// </summary>
public interface IPlayRecordRepository
{
    /// <summary>
    /// 記録と収縮先を 1 つのトランザクションで保存する。
    /// 保存の後、記録の Id と CreatedAt が入る。
    /// </summary>
    /// <param name="record">保存する記録。規則はエンティティで確かめ済みのもの。</param>
    /// <param name="ct">取り消しの合図。</param>
    Task AddAsync(PlayRecord record, CancellationToken ct);
}
