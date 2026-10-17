using NightreignTools.Backend.Common;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// サービスの登録
// ==========================================

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddServiceKey();

// 例外は ApiExceptionHandler が ErrorResponse の形に変える
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();

builder.Services.AddHealthChecks();
builder.Services.AddOpenApi();

var app = builder.Build();

// ==========================================
// ミドルウェア
// ==========================================

app.UseExceptionHandler();
// 本文のないまま終わった 4xx(ルートがないなど)にも、ErrorResponse の本文を付ける
app.UseStatusCodePages(ErrorResponseWriter.WriteStatusCodePageAsync);
app.UseMiddleware<ServiceKeyMiddleware>();

// ==========================================
// エンドポイント
// ==========================================

app.MapHealthChecks(ServiceKeyMiddleware.HealthPath);

// OpenAPI の文書は開発の環境でだけ出す(API の共通仕様 4.7)
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// 機能のエンドポイントは /api/v1 のグループに、機能ごとの Map<機能>Endpoints で足す(サービスの共通構成 5.3)

app.Run();

/// <summary>テストの WebApplicationFactory から参照するための宣言。</summary>
public partial class Program;
