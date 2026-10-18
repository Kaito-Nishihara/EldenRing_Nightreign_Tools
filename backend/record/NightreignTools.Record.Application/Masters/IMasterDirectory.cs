namespace NightreignTools.Record.Application.Masters;

/// <summary>
/// master-service の持つマスタを、識別子で引くポート(サービスの共通構成 5.5)。
/// 実装は Infrastructure の HttpClient のクライアントで、master-service の内部の API(マスタの管理 4.4)を呼ぶ。
/// master-service が応答しないときは <see cref="NightreignTools.SharedKernel.ServiceUnavailableException"/> を投げる。
/// </summary>
public interface IMasterDirectory
{
    Task<MasterReferences> ResolveAsync(MasterReferenceQuery query, CancellationToken ct);
}
