-- 開始地点の拠点タイプを正しい値に修正
-- campfire → church（教会）、ruins → camp（野営地）
UPDATE game.spawn_points SET nearby_base_type = 'church' WHERE nearby_base_type = 'campfire';
UPDATE game.spawn_points SET nearby_base_type = 'camp' WHERE nearby_base_type = 'ruins';
