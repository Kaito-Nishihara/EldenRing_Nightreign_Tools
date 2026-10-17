# frontend(Next.js)

Next.js 15(App Router)、React 19、TypeScript、Tailwind CSS 4、react-konva。
画面は Gateway(8080)だけを呼ぶ。

## ディレクトリの役割

```
src/
├── app/          # ページ(/、/record、/build、/admin)。データの取得と部品の組み立てだけを書く
├── components/   # 部品。機能ごと(map、pattern、record、build、admin)に分ける。共通の部品は ui/
├── hooks/        # 画面の状態と、API の取得をまとめる処理
├── lib/          # API のクライアント(api.ts)、計算(buildCalc.ts)、対応表(mapImage.ts)
├── types/        # API の応答の型
└── __tests__/    # Vitest
public/           # マップ画像、アイコン(仕様書の試作もここの画像を使う)
```

## 決まり

- **App Router を使う。** Pages Router を使わない。
- **API は `src/lib/api.ts` の axios のクライアントだけから呼ぶ。** 部品から直接呼ばない。エラーの形(`{ status, error, message, timestamp }`)の扱いを 1 か所にまとめるためである。
- **API の取得は `src/hooks/` に切り出す。** 部品はフックの返す `data`、`loading`、`error` を使う。
- **ビルドシミュレーターの計算は `src/lib/buildCalc.ts` に書く。** サービスに同じ計算を書かない(決定の記録 0006)。
- **管理の入口を隠すことは、権限の判定にならない。** 管理用の API は、Gateway が鍵を確かめる。
- **マップの座標は 1024 四方の座標空間で描く。** マップ画像(1000 px)は座標空間いっぱいに引き伸ばす。
- 色、書体、余白は `globals.css` の `@theme` の名前で指定し、値を直接書かない。見た目と部品の決まりは [画面の共通仕様](../docs/design/common-frontend.md) に従う。
- 見た目は [画面の試作](../docs/mockups/) に合わせる。
- `any` を使わない。型が定まらないときは `unknown` から絞り込む。

## テスト

- 計算と対応表の関数には Vitest の単体テストを書く。
- テストの名前かコメントに、受け入れ条件の識別子を書く。
- 画面を変えたときは、Playwright でその画面を開き、スクリーンショットを見て確かめる。

```bash
docker compose run --rm frontend-dev npx tsc --noEmit
docker compose run --rm frontend-dev npm run lint
docker compose run --rm frontend-dev npm run test
```
