// テストは 1 つのテスト用のデータベースを共有し、起動のたびにマイグレーションを確かめるので、並べて走らせない
[assembly: CollectionBehavior(DisableTestParallelization = true)]
