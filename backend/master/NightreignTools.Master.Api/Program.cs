using NightreignTools.Master.Api.Endpoints;
using NightreignTools.Master.Application;
using NightreignTools.Master.Infrastructure;
using NightreignTools.ServiceDefaults;

// master-service: マスタの取得と管理(サービスの共通構成 5.1)
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
api.MapMasterEndpoints();
api.MapAdminEndpoints();

// サービスの間だけで使う API。Gateway は振り分けない(API の共通仕様 4.8)
var internalApi = app.MapGroup("/internal/v1");
internalApi.MapReferenceEndpoints();

return await app.RunServiceAsync();

/// <summary>
/// テストの WebApplicationFactory から参照するための宣言。
/// </summary>
public partial class Program;
