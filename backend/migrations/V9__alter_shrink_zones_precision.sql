-- 収縮候補地の座標・半径をピクセル座標に対応させる
ALTER TABLE game.shrink_zones ALTER COLUMN center_x TYPE DECIMAL(7, 2);
ALTER TABLE game.shrink_zones ALTER COLUMN center_y TYPE DECIMAL(7, 2);
ALTER TABLE game.shrink_zones ALTER COLUMN radius TYPE DECIMAL(7, 2);

UPDATE game.shrink_zones
SET
    center_x = ROUND(center_x * 1024, 2),
    center_y = ROUND(center_y * 1024, 2)
WHERE center_x <= 1.00
  AND center_y <= 1.00;

UPDATE game.shrink_zones
SET radius = 350.00;
