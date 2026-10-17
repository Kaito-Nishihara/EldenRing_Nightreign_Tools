using NightreignTools.Record.Domain.Entities;

namespace NightreignTools.Record.Application.PlayRecords;

/// <summary>プレイ記録の保存先のポート。実装は Infrastructure の EF Core のリポジトリ。</summary>
public interface IPlayRecordRepository
{
    /// <summary>記録と収縮先を 1 つのトランザクションで保存する。保存の後、記録の Id と CreatedAt が入る。</summary>
    Task AddAsync(PlayRecord record, CancellationToken ct);
}
