using NightreignTools.Record.Api.Endpoints;
using NightreignTools.Record.Application;
using NightreignTools.Record.Infrastructure;
using NightreignTools.ServiceDefaults;

// record-service: プレイ記録とパターン検索(サービスの共通構成 5.1)
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
api.MapPlayRecordEndpoints();
api.MapPatternEndpoints();

return await app.RunServiceAsync();

/// <summary>
/// テストの WebApplicationFactory から参照するための宣言。
/// </summary>
public partial class Program;
