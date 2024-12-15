FROM localstack/localstack:4.0.3

# 初期化スクリプトをコンテナ内にコピー
COPY init.sh /etc/localstack/init/ready.d/init.sh

# 実行権限を付与
RUN chmod +x /etc/localstack/init/ready.d/init.sh
