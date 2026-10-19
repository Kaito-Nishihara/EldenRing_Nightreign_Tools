using NightreignTools.Relic.Api.Endpoints;
using NightreignTools.Relic.Application;
using NightreignTools.Relic.Infrastructure;
using NightreignTools.ServiceDefaults;

// relic-service: ビルドシミュレーターの遺物の効果のマスタ(サービスの共通構成 5.1)
var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// DTO の注釈による入力の検証(API の共通仕様 4.5)。
// 検証の情報はソースジェネレーターがこの呼び出しに差し込むので、エンドポイントを持つ Api のプロジェクトで呼ぶ
builder.Services.AddValidation();
builder.Services.AddApplication();
builder.Services.AddInfrastructure();

var app = builder.Build();

app.UseServiceDefaults();

// 機能のエンドポイントは /api/v1 のグループに、機能ごとの Map<機能>Endpoints で足す(サービスの共通構成 5.4)
var api = app.MapGroup("/api/v1");
api.MapRelicEffectEndpoints();

return await app.RunServiceAsync();

/// <summary>
/// テストの WebApplicationFactory から参照するための宣言。
/// </summary>
public partial class Program;
