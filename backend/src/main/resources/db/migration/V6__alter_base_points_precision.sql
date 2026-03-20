-- 拠点候補地の座標をピクセル座標に対応させる
-- DECIMAL(5,4)では最大9.9999のためピクセル座標が格納できない
ALTER TABLE game.base_points ALTER COLUMN x TYPE DECIMAL(7, 2);
ALTER TABLE game.base_points ALTER COLUMN y TYPE DECIMAL(7, 2);
