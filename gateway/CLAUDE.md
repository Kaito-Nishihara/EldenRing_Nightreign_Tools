# gateway(ASP.NET Core + YARP)

画面からの要求をただ 1 つの入口で受け、パスの接頭辞で 3 つのサービスへ振り分ける。
データベースに触れない。

## 決まり

- **Minimal API で書き、ルーティングは `Program.cs` と `appsettings.json` に集める。**
- **振り分けは YARP で行う。** 振り分けの表は [API の共通仕様](../docs/design/common-api.md) の 4.6 にある。広い規則(`/api/v1/{**catch-all}` から `master-service`)に `Order: 100` を付け、狭い規則は小さい `Order` で足す。
- **`/internal/v1` は振り分けない。** Gateway が 404 を返す。
- **画面から来た `X-Service-Key` を消し、Gateway の値を付け直して渡す。**
- **管理用の API(`/api/v1/admin`)は、`X-Admin-Key` を確かめてから渡す(段階 2)。** 管理者の判定は Gateway だけで行う。
- 振り分け先は環境変数(`MASTER_SERVICE_URL` など)、許すオリジンは `CORS_ORIGINS` で受け取る。
- C# のドキュメントコメントは、[backend の決まり](../backend/CLAUDE.md) の「コメントの決まり」に従う。
- Gateway が自分で返すエラー(404、502、504、401、429)も、`{ status, error, message, timestamp }` の形にする。

## 確かめ方

Gateway にはテストのプロジェクトがない。
振り分けとキーの付け直しは、`docker compose up --build` で一式を動かし、8080 から呼んで確かめる。
