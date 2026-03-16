# API設計書

**バージョン**: 1.0
**作成日**: 2026-03-16

---

## 1. 共通仕様

- **Base URL**: `http://localhost:8080/api/v1`
- **形式**: RESTful JSON API
- **文字コード**: UTF-8
- **タイムゾーン**: UTC（表示はフロント側でJSTに変換）

---

## 2. エンドポイント一覧

| メソッド | パス | 概要 | サービス |
|---------|------|------|---------|
| GET | `/bosses` | ボス一覧取得 | マスタサービス |
| GET | `/jihen` | 地変一覧取得 | マスタサービス |
| GET | `/shrink-zones` | 収縮候補地一覧取得 | マスタサービス |
| GET | `/spawn-points` | 開始地点一覧取得 | マスタサービス |
| GET | `/patterns` | 過去パターン検索 | パターンサービス |
| POST | `/play-records` | プレイ記録保存 | 記録サービス |

---

## 3. マスタAPI

### GET `/bosses`

```json
// Response 200
[
  {
    "id": "gladius",
    "nameJa": "三つ首の獣（グラディウス）",
    "nameEn": "Gladius, Beast Regent",
    "weaknesses": ["holy"],
    "isDlc": false,
    "unlockCondition": "最初から選択可能"
  }
]
```

### GET `/jihen`

```json
// Response 200
[
  {
    "id": "crater",
    "nameJa": "火口",
    "area": "north",
    "centerX": 0.50,
    "centerY": 0.20,
    "isDlc": false,
    "unlockCondition": "夜の王を1体撃破後"
  }
]
```

### GET `/shrink-zones`

```json
// Response 200
[
  {
    "id": 1,
    "nameJa": "北砦付近",
    "centerX": 0.45,
    "centerY": 0.20,
    "radius": 0.08
  }
]
```

### GET `/spawn-points`

```json
// Response 200
[
  {
    "id": 1,
    "nameJa": "北東・野営地付近",
    "x": 0.75,
    "y": 0.25,
    "nearbyBaseType": "camp"
  }
]
```

---

## 4. パターン検索API

### GET `/patterns`

**クエリパラメータ**

| パラメータ | 型 | 必須 | 説明 |
|-----------|---|------|------|
| `bossId` | string | ✓ | ボスID |
| `spawnPointId` | integer | ✓ | 開始地点ID |
| `jihenId` | string | — | 地変ID（省略時は地変なしで検索） |

**リクエスト例**

```
GET /patterns?bossId=gladius&spawnPointId=3&jihenId=crater
```

**レスポンス例**

```json
// Response 200
{
  "totalRecords": 4,
  "patterns": [
    {
      "patternIndex": 1,
      "recordCount": 3,
      "lastRecordedAt": "2026-03-15T20:30:00Z",
      "entries": [
        {
          "day": 1,
          "phase": 1,
          "zone": { "id": 1, "nameJa": "北砦付近", "centerX": 0.45, "centerY": 0.20, "radius": 0.08 }
        },
        {
          "day": 1,
          "phase": 2,
          "zone": { "id": 1, "nameJa": "北砦付近", "centerX": 0.45, "centerY": 0.20, "radius": 0.05 }
        },
        {
          "day": 2,
          "phase": 1,
          "zone": { "id": 7, "nameJa": "南の教会跡", "centerX": 0.55, "centerY": 0.75, "radius": 0.08 }
        },
        {
          "day": 2,
          "phase": 2,
          "zone": { "id": 7, "nameJa": "南の教会跡", "centerX": 0.55, "centerY": 0.75, "radius": 0.05 }
        }
      ]
    },
    {
      "patternIndex": 2,
      "recordCount": 1,
      "lastRecordedAt": "2026-03-10T18:00:00Z",
      "entries": [
        {
          "day": 1,
          "phase": 1,
          "zone": { "id": 5, "nameJa": "中央湖畔", "centerX": 0.50, "centerY": 0.50, "radius": 0.08 }
        },
        {
          "day": 2,
          "phase": 1,
          "zone": { "id": 9, "nameJa": "東の遺跡", "centerX": 0.80, "centerY": 0.40, "radius": 0.08 }
        }
      ]
    }
  ]
}
```

---

## 5. プレイ記録保存API

### POST `/play-records`

**リクエスト**

```json
{
  "bossId": "gladius",
  "jihenId": "crater",        // nullの場合は地変なし
  "spawnPointId": 3,
  "note": "2日目は地変方向に引き寄せられた",
  "entries": [
    { "day": 1, "phase": 1, "zoneId": 1 },
    { "day": 1, "phase": 2, "zoneId": 1 },
    { "day": 2, "phase": 1, "zoneId": 7 },
    { "day": 2, "phase": 2, "zoneId": 7 }
  ]
}
```

**レスポンス**

```json
// Response 201
{
  "id": 42,
  "createdAt": "2026-03-16T10:00:00Z"
}
```

**バリデーション**

| フィールド | ルール |
|-----------|--------|
| `bossId` | 必須・存在するIDであること |
| `spawnPointId` | 必須・存在するIDであること |
| `entries` | 必須・1件以上 |
| `entries[].day` | 1 or 2 |
| `entries[].phase` | 1 or 2 |
| `entries[].zoneId` | 存在するIDであること |
| 同一レコード内の `(day, phase)` | 重複不可 |

---

## 6. エラーレスポンス共通形式

```json
// Response 400
{
  "status": 400,
  "error": "Bad Request",
  "message": "bossId は必須です",
  "timestamp": "2026-03-16T10:00:00Z"
}

// Response 404
{
  "status": 404,
  "error": "Not Found",
  "message": "bossId 'unknown' は存在しません",
  "timestamp": "2026-03-16T10:00:00Z"
}
```
