using NightreignTools.Master.Api.Endpoints;
using NightreignTools.Master.Application;
using NightreignTools.Master.Infrastructure;
using NightreignTools.ServiceDefaults;

// master-service: マスタの取得と管理(サービスの共通構成 5.1)
var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddApplication();
builder.Services.AddInfrastructure();

var app = builder.Build();

app.UseServiceDefaults();

// 機能のエンドポイントは /api/v1 のグループに、機能ごとの Map<機能>Endpoints で足す(サービスの共通構成 5.4)
var api = app.MapGroup("/api/v1");
api.MapMasterEndpoints();

return await app.RunServiceAsync();

/// <summary>テストの WebApplicationFactory から参照するための宣言。</summary>
public partial class Program;
