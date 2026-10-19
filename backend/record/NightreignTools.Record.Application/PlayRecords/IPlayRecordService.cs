namespace NightreignTools.Record.Application.PlayRecords;

/// <summary>
/// プレイ記録の保存(プレイ記録 4 章)。
/// </summary>
public interface IPlayRecordService
{
    Task<CreatePlayRecordResponse> CreateAsync(CreatePlayRecordRequest request, CancellationToken ct);
}
