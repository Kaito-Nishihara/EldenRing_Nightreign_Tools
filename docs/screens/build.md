---
hide:
  - toc
---

# 画面: ビルドシミュレーター

遺物の効果を選び、効果の合計を見る画面である。

| 項目 | 内容 |
|---|---|
| パス | `/build` |
| 使う人 | プレイヤー |
| 骨組みと遷移 | [画面設計: 画面の一覧と遷移](../design/screens.md) の 3.6 |

## 対応する設計

| 文書 | この画面について決めていること |
|---|---|
| [機能設計: ビルドシミュレーター](../design/build-simulator.md) | 枠の数、ランクの選び方、計算の種類ごとの合計の仕方、表示の桁 |

## 見た目の試作

枠の中は、押したり切り替えたりできる。
保存はされない。

### 広い画面

通常効果、深層効果、合計の 3 列に並べる。
深層効果の枠は青紫を帯びた地にし、デメリットは `ember` の色で出す。

<div class="screen-frame"><iframe src="../../mockups/build.html" title="ビルドシミュレーターの試作。広い画面" loading="lazy"></iframe></div>

[別のタブで開く](../mockups/build.html)

### 狭い画面

3 列を縦に並べる。

<div class="screen-frame screen-frame--phone" data-width="390" data-height="844"><iframe src="../../mockups/build.html" title="ビルドシミュレーターの試作。狭い画面" loading="lazy"></iframe></div>

[別のタブで開く](../mockups/build.html)
