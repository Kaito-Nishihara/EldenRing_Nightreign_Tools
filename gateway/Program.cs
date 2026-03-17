using System.Text;
using Gateway.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// サービス登録
// ==========================================

// JWT サービスをシングルトンとして登録
builder.Services.AddSingleton<JwtService>();

// CORS 設定（フロントエンドからのアクセスを許可）
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// JWT Bearer 認証ミドルウェア
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // JwtService を構築して署名キーを取得
        var jwtService = new JwtService(builder.Configuration);

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtService.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtService.Audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = jwtService.GetSigningKey(),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization();

// ヘルスチェック
builder.Services.AddHealthChecks();

// YARP リバースプロキシ設定（appsettings.json から読み込み）
var reverseProxyConfig = builder.Configuration.GetSection("ReverseProxy");

// 環境変数 BACKEND_URL でバックエンドURLをオーバーライド
var backendUrl = Environment.GetEnvironmentVariable("BACKEND_URL");
if (!string.IsNullOrEmpty(backendUrl))
{
    // appsettings.json のクラスター設定を環境変数の値で上書き
    builder.Configuration["ReverseProxy:Clusters:backend-cluster:Destinations:backend:Address"] = backendUrl;
}

// サービス間認証キーを取得
var serviceKey = Environment.GetEnvironmentVariable("SERVICE_KEY")
    ?? builder.Configuration["ServiceKey"]
    ?? throw new InvalidOperationException("SERVICE_KEY が設定されていません。環境変数または appsettings.json で設定してください。");

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms(transformContext =>
    {
        // プロキシ時に X-Service-Key ヘッダーを自動付与
        transformContext.AddRequestTransform(context =>
        {
            context.ProxyRequest.Headers.Remove("X-Service-Key");
            context.ProxyRequest.Headers.Add("X-Service-Key", serviceKey);
            return ValueTask.CompletedTask;
        });
    });

var app = builder.Build();

// ==========================================
// ミドルウェアパイプライン
// ==========================================

// CORS を最初に適用
app.UseCors("FrontendPolicy");

// 認証・認可ミドルウェア
app.UseAuthentication();
app.UseAuthorization();

// ==========================================
// エンドポイント定義（Minimal API）
// ==========================================

// ヘルスチェックエンドポイント
app.MapHealthChecks("/health");

// 匿名トークン発行エンドポイント（v1ではユーザー認証なし）
app.MapPost("/auth/token", (JwtService jwtService) =>
{
    var token = jwtService.GenerateAnonymousToken();
    return Results.Ok(new
    {
        token,
        tokenType = "Bearer",
        expiresIn = 3600 // 秒（デフォルト60分）
    });
});

// YARP リバースプロキシをマッピング
// v1では全APIエンドポイントは認証不要（将来のv2でPOST系に認証必須を追加予定）
app.MapReverseProxy();

app.Run();
