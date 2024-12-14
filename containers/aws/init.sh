#!/bin/bash

# DynamoDB テーブル「Auth」を作成
awslocal dynamodb create-table \
  --table-name Auth \
  --attribute-definitions \
    AttributeName=UserId,AttributeType=S \
  --key-schema \
    AttributeName=UserId,KeyType=HASH \
  --global-secondary-indexes \
    '[
      {
        "IndexName": "GoogleUserId-index",
        "KeySchema": [
          { "AttributeName": "GoogleUserId", "KeyType": "HASH" }
        ],
        "Projection": {
          "ProjectionType": "INCLUDE",
          "NonKeyAttributes": ["UserId", "SampleRole"]
        }
      }
    ]' \
  --billing-mode PAY_PER_REQUEST \
  --table-class STANDARD \
  --sse-specification Enabled=true,SSEType=AES256
