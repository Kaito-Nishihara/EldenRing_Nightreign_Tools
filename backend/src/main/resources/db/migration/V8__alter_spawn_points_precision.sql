-- nearby_base_typeカラムを削除（名前に含める運用に変更）
ALTER TABLE game.spawn_points DROP COLUMN nearby_base_type;

-- 既存データを削除して初期マスタデータを投入
DELETE FROM game.spawn_points;

INSERT INTO game.spawn_points (name_ja, x, y, created_at) VALUES
  ('北東の教会',   351.00, 387.00, NOW()),
  ('西の教会',     216.00, 578.00, NOW()),
  ('南西の教会',   218.00, 763.00, NOW()),
  ('南の教会',     577.00, 830.00, NOW()),
  ('南東の教会',   561.00, 679.00, NOW()),
  ('北の教会',     547.00, 215.00, NOW()),
  ('北西の教会',   218.00, 360.00, NOW()),
  ('東の教会',     697.00, 398.00, NOW()),
  ('東の野営地',   808.00, 569.00, NOW());
