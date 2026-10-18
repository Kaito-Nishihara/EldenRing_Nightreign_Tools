# docs-site

仕様書(`../docs/` の Markdown)をブラウザで読むためのサイト。
アプリ本体とは別のもので、アプリの起動、ビルド、配置には関わらない。

| 置き場 | 中身 |
|---|---|
| `../docs/` | 文書の中身。Markdown、図、画面の試作だけを置く |
| `mkdocs.yml` | サイトの設定と、文書の並び(`nav`) |
| `overrides/assets/` | 見た目(`docs.css`)と、試作を埋め込む枠(`screen-frame.js`) |
| `hooks/` | 表示のときに Markdown へかける変換と、試作が使う画像(`../frontend/public`)をサイトに写す処理 |
| `compose.yaml` | サイトの起動 |

## 使い方

```bash
docker compose -f docs-site/compose.yaml up -d      # 起動。http://localhost:8000
docker compose -f docs-site/compose.yaml logs docs  # 警告を確かめる
docker compose -f docs-site/compose.yaml down       # 停止
```

文書、見た目、変換の処理のどれを保存しても、表示が自動で更新される。

## 公開

`main` に `docs/`、`docs-site/`、`frontend/public/` の変更が入ると、`.github/workflows/docs.yml` がサイトを作り、GitHub Pages に置く。
公開先は https://kaito-nishihara.github.io/EldenRing_Nightreign_Tools/ である。
PR では、警告なしに作れること(`--strict`)だけを確かめ、公開しない。
