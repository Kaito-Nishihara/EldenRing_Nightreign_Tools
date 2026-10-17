"""画面の試作が使う画像(frontend/public)を、サイトの frontend-public/ に写す。

試作(docs/mockups/)は、アプリと同じマップ画像とアイコンを使う。
画像を docs/ に複製すると 2 か所で持つことになるので、サイトを作るときにだけ写す。
compose.yaml で、リポジトリの frontend/public を ../frontend/public に渡している。
"""

import logging
import shutil
from pathlib import Path

log = logging.getLogger("mkdocs.hooks.copy_frontend_public")

# サイトの中の置き場。試作の mock.js は、サイトから開いたとき ../frontend-public/ を見る
SITE_DIR_NAME = "frontend-public"


def on_post_build(config, **kwargs):
    source = Path(config.config_file_path).parent.parent / "frontend" / "public"
    if not source.is_dir():
        log.warning("frontend/public がないので、試作の画像を写さない: %s", source)
        return
    target = Path(config.site_dir) / SITE_DIR_NAME
    shutil.copytree(source, target, dirs_exist_ok=True, ignore=shutil.ignore_patterns(".gitkeep"))
