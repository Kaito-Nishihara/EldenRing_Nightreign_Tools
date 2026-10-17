#!/bin/sh
# サービスごとのデータベースと利用者を作る(データベースの共通仕様 5.1)。
# postgres のイメージは、ボリュームが空のときにだけ、この置き場の手順を走らせる。
# 各利用者は自分のデータベースだけを持ち、ほかの利用者は PUBLIC の CONNECT を外してつなげなくする。
set -eu

create_service_database() {
  name="$1"
  password="$2"
  if [ -z "$password" ]; then
    echo "${name} の合言葉がありません。.env に書いてください" >&2
    exit 1
  fi
  # 名前と合言葉は psql の変数で渡し、SQL の中で引用する(合言葉に引用符があっても壊れないようにする)
  psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname postgres \
    -v name="$name" -v password="$password" <<'SQL'
CREATE ROLE :"name" LOGIN PASSWORD :'password';
CREATE DATABASE :"name" OWNER :"name";
REVOKE ALL ON DATABASE :"name" FROM PUBLIC;
SQL
}

create_service_database nightreign_master "${MASTER_DB_PASSWORD:-}"
create_service_database nightreign_record "${RECORD_DB_PASSWORD:-}"
create_service_database nightreign_relic "${RELIC_DB_PASSWORD:-}"
