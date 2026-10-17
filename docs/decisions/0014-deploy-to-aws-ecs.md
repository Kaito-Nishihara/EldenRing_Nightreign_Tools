# 0014 AWS の ECS on Fargate に配置し、GitHub Actions で配る

## 状況

本番の配置の先と、配る仕組みを決めていなかった(実行環境の共通仕様 2 章)。
利用者から、AWS に配置する前提で CI/CD を設計するよう求められた。
サービスは 5 つのコンテナ(frontend、gateway、master、record、relic)と PostgreSQL で、手元では compose で動かしている。

## 決めたこと

コンテナを ECS on Fargate で動かし、データベースを RDS for PostgreSQL にし、GitHub Actions から OIDC で配る。
AWS の資源は Terraform で作る。

## 理由

- 手元の compose と同じイメージを、そのまま動かせる。サーバーの OS を管理しなくてよい。
- ECS Service Connect で、サービスの間の呼び先を名前で引ける。今の `MASTER_SERVICE_URL` の形を変えずに済む。
- ECS の配置のサーキットブレーカーで、ヘルスチェックを通らない版から自動で戻せる。
- RDS は 1 台に 3 つのデータベースと利用者を置けるので、手元の分け方(決定の記録 0012)を変えずに済む。スナップショットと指定時刻への復元を持つ。
- GitHub Actions は今の CI と同じ場所で動く。OIDC なら期限のない鍵を GitHub に置かずに済む。
- Terraform は、ステージングと本番を同じ定義から大きさだけ変えて作れる。

## 却下した案

- **EKS**: 5 つのコンテナには大きすぎる。クラスターの更新の手間が増える。
- **App Runner**: サービスの間を非公開のまま呼ぶ形が組みにくい。配置の前に単発のタスクを走らせる仕組みがない。
- **EC2 に compose**: 手軽だが、OS の更新、戻し方、台を増やすことを自分で持つことになる。
- **AWS CDK**: 型のある言葉で書ける利点はあるが、このリポジトリに足す言葉と道具が増える。Terraform との選択は利用者の承認を待つ。
- **AWS の鍵を GitHub の秘密の値に置く**: 鍵が漏れたときに期限なく使われる。

## 影響

- 実行環境の共通仕様の「本番の配置」と「CI」を、[CI/CD と AWS への配置](../design/common-delivery.md) で決める。
- マイグレーションの適用の仕方を、ステージングと本番では変える(決定の記録 [0015](0015-migrate-before-deploy.md))。
- 費用が見込みより大きければ、frontend を S3 と CloudFront の配信に移すことを考える。
