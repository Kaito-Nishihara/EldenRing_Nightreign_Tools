using System.Net;

namespace NightreignTools.Record.Tests;

/// <summary>
/// 決まった応答を返す HTTP の偽物。送られた要求を覚え、master-service のクライアントのテストで中身を確かめる。
/// </summary>
/// <param name="respond">送られた要求から応答を作る。</param>
internal sealed class StubHttpHandler(Func<HttpRequestMessage, HttpResponseMessage> respond) : HttpMessageHandler
{
    public List<HttpRequestMessage> Requests { get; } = [];
    public List<string> Bodies { get; } = [];

    /// <summary>
    /// 毎回同じ状態と本文を返す。
    /// </summary>
    /// <param name="status">返す HTTP の状態。</param>
    /// <param name="json">返す本文。</param>
    /// <returns>毎回同じ応答を返す偽物。</returns>
    public static StubHttpHandler Returning(HttpStatusCode status, string json = "{}") =>
        new(_ => new HttpResponseMessage(status) { Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json") });

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Requests.Add(request);
        Bodies.Add(request.Content is null ? "" : await request.Content.ReadAsStringAsync(cancellationToken));
        return respond(request);
    }
}
