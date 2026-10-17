-- 拠点候補地にマップ識別カラムを追加（地変ごとに出し分ける）
ALTER TABLE game.base_points ADD COLUMN map_id VARCHAR(20) NOT NULL DEFAULT 'normal';

-- 通常マップの拠点候補地 初期マスタデータ（15ヶ所）
-- 実ゲームで確認した座標をピクセル座標で登録
DELETE FROM game.base_points;

INSERT INTO game.base_points (map_id, name_ja, x, y, created_at) VALUES
  ('normal', NULL, 404.00, 798.00, NOW()),
  ('normal', NULL, 615.00, 698.00, NOW()),
  ('normal', NULL, 279.00, 667.00, NOW()),
  ('normal', NULL, 317.00, 562.00, NOW()),
  ('normal', NULL, 278.00, 459.00, NOW()),
  ('normal', NULL, 225.00, 272.00, NOW()),
  ('normal', NULL, 410.00, 197.00, NOW()),
  ('normal', NULL, 434.00, 316.00, NOW()),
  ('normal', NULL, 638.00, 301.00, NOW()),
  ('normal', NULL, 742.00, 232.00, NOW()),
  ('normal', NULL, 801.00, 438.00, NOW()),
  ('normal', NULL, 575.00, 451.00, NOW()),
  ('normal', NULL, 677.00, 501.00, NOW()),
  ('normal', NULL, 766.00, 763.00, NOW()),
  ('normal', NULL, 777.00, 636.00, NOW());
