using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Gateway.Auth;

/// <summary>
/// JWT トークンの発行・検証を行うサービス
/// </summary>
public class JwtService
{
    private readonly string _secret;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expirationMinutes;

    public JwtService(IConfiguration configuration)
    {
        // 環境変数からJWT署名キーを取得（未設定の場合はエラー）
        _secret = Environment.GetEnvironmentVariable("JWT_SECRET")
            ?? configuration["Jwt:Secret"]
            ?? throw new InvalidOperationException("JWT_SECRET が設定されていません。環境変数または appsettings.json で設定してください。");

        _issuer = configuration["Jwt:Issuer"] ?? "nightreign-gateway";
        _audience = configuration["Jwt:Audience"] ?? "nightreign-app";
        _expirationMinutes = int.TryParse(configuration["Jwt:ExpirationMinutes"], out var minutes) ? minutes : 60;
    }

    /// <summary>
    /// 署名キーを取得する（認証ミドルウェアの設定用）
    /// </summary>
    public SymmetricSecurityKey GetSigningKey()
    {
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
    }

    /// <summary>
    /// 発行者を取得する
    /// </summary>
    public string Issuer => _issuer;

    /// <summary>
    /// オーディエンスを取得する
    /// </summary>
    public string Audience => _audience;

    /// <summary>
    /// 匿名トークンを発行する（v1ではユーザー認証なし）
    /// 将来のv2でユーザー情報を含むトークンに拡張予定
    /// </summary>
    public string GenerateAnonymousToken()
    {
        var claims = new List<Claim>
        {
            // 匿名ユーザー用のクレーム
            new(JwtRegisteredClaimNames.Sub, "anonymous"),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            new("role", "anonymous")
        };

        var credentials = new SigningCredentials(GetSigningKey(), SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
