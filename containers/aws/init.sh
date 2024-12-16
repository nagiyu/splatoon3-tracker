#!/bin/bash

# DynamoDB テーブル「Auth」を作成
awslocal dynamodb create-table \
  --table-name Auth \
  --attribute-definitions \
    AttributeName=UserId,AttributeType=S \
    AttributeName=GoogleUserId,AttributeType=S \
  --key-schema \
    AttributeName=UserId,KeyType=HASH \
  --global-secondary-indexes \
    '[
      {
        "IndexName": "GoogleUserId-index-20241215",
        "KeySchema": [
          { "AttributeName": "GoogleUserId", "KeyType": "HASH" }
        ],
        "Projection": {
          "ProjectionType": "INCLUDE",
          "NonKeyAttributes": ["UserId", "UserName", "SampleRole"]
        }
      }
    ]' \
  --billing-mode PAY_PER_REQUEST \
  --table-class STANDARD \
  --sse-specification Enabled=true,SSEType=AES256

# DynamoDB の Auth にデータ追加
awslocal dynamodb put-item \
  --table-name Auth \
  --item \
    '{
      "UserId": {"S": "acaa64d6-3da6-49a7-9c52-d57cfe8bdb34"},
      "GoogleUserId": {"S": "105305940240519833154"},
      "UserName": {"S": "なぎゆー"},
      "SampleRole": {"S": "Admin"}
    }'
