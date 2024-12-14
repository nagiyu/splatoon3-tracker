#!/bin/bash

# 必要なS3バケットを作成
awslocal s3api create-bucket --bucket health

# 必要なDynamoDBテーブルを作成する
awslocal dynamodb create-table \
  --table-name my-table \
  --attribute-definitions AttributeName=id,AttributeType=S \
  --key-schema AttributeName=id,KeyType=HASH \
  --provisioned-throughput ReadCapacityUnits=5,WriteCapacityUnits=5
