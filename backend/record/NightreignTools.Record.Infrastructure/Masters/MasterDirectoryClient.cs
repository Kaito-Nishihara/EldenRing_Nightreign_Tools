using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using NightreignTools.Record.Application.Masters;
using NightreignTools.SharedKernel;
using Polly.CircuitBreaker;
using Polly.Timeout;

namespace NightreignTools.Record.Infrastructure.Masters;

/// <summary>
/// master-service の内部の API(マスタの管理 4.4)を呼ぶクライアント(サービスの共通構成 5.5)。
/// 呼び先、サービス間キー、期限と再試行は、登録のとき(<see cref="DependencyInjection"/>)に HttpClient に付ける。
/// 応答がない、5xx、応答の形が読めないときは、<see cref="ServiceUnavailableException"/>(503)にする。
/// </summary>
internal sealed class MasterDirectoryClient(HttpClient http, ILogger<MasterDirectoryClient> logger) : IMasterDirectory
{
    public const string ResolvePath = "internal/v1/references/resolve";
    public const string UnavailableMessage = "マスタのサービスに接続できません。時間をおいてやり直してください";

    public async Task<MasterReferences> ResolveAsync(MasterReferenceQuery query, CancellationToken ct)
    {
        try
        {
            using var response = await http.PostAsJsonAsync(ResolvePath, query, ct);
            if (!response.IsSuccessStatusCode)
            {
                // 4xx は設定の誤り(サービス間キーの食い違いなど)なので、5xx より重く出す
                var level = (int)response.StatusCode >= 500 ? LogLevel.Warning : LogLevel.Error;
                logger.Log(level, "master-service が {Status} を返しました", (int)response.StatusCode);
                throw new ServiceUnavailableException(UnavailableMessage);
            }

            return await response.Content.ReadFromJsonAsync<MasterReferences>(ct)
                ?? throw new ServiceUnavailableException(UnavailableMessage);
        }
        catch (Exception ex) when (IsUnavailable(ex, ct))
        {
            logger.LogWarning(ex, "master-service を呼べませんでした");
            throw new ServiceUnavailableException(UnavailableMessage, ex);
        }
    }

    /// <summary>応答がないことを表す例外か。呼んだ側が取り消したときは含めない。</summary>
    private static bool IsUnavailable(Exception ex, CancellationToken ct) => ex switch
    {
        HttpRequestException or TimeoutRejectedException or BrokenCircuitException or JsonException => true,
        TaskCanceledException => !ct.IsCancellationRequested,
        _ => false,
    };
}
