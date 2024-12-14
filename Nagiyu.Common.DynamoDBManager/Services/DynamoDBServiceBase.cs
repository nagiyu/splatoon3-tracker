using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nagiyu.Common.DynamoDBManager.Services
{
    /// <summary>
    /// DynamoDB サービスの基底クラス
    /// </summary>
    public class DynamoDBServiceBase
    {
        /// <summary>
        /// DynamoDB クライアント
        /// </summary>
        protected AmazonDynamoDBClient client;

        /// <summary>
        /// DynamoDB コンテキスト
        /// </summary>
        protected DynamoDBContext context;

        /// <summary>
        /// クライアントとコンテキストを初期化する
        /// </summary>
        /// <param name="accessKey">アクセスキー</param>
        /// <param name="secretKey">シークレットキー</param>
        /// <param name="region">リージョン</param>
        /// <param name="serviceUrl">サービス URL</param>
        protected void InitializeClient(string accessKey, string secretKey, string region, string serviceUrl = null)
        {
            AmazonDynamoDBConfig config;

            if (!string.IsNullOrEmpty(serviceUrl))
            {
                // ServiceURLが指定された場合
                config = new AmazonDynamoDBConfig
                {
                    ServiceURL = serviceUrl,                    // LocalStackやカスタムエンドポイントを指定
                    RegionEndpoint = RegionEndpoint.GetBySystemName(region) // 必要ならリージョンも設定
                };
            }
            else
            {
                // ServiceURLが指定されていない場合
                config = new AmazonDynamoDBConfig
                {
                    RegionEndpoint = RegionEndpoint.GetBySystemName(region) // AWS本番環境向け
                };
            }

            // DynamoDBクライアントを初期化
            client = new AmazonDynamoDBClient(accessKey, secretKey, config);

            // DynamoDBContextを初期化
            context = new DynamoDBContext(client);
        }

        /// <summary>
        /// アイテムを取得する
        /// </summary>
        /// <param name="tableName">テーブル名</param>
        /// <param name="indexName">インデックス名</param>
        /// <param name="keyName">キー名</param>
        /// <param name="keyValue">キー値</param>
        /// <returns>アイテムのリスト</returns>
        protected async Task<List<Dictionary<string, AttributeValue>>> GetItems(string tableName, string indexName, string keyName, string keyValue)
        {
            var queryRequest = new QueryRequest
            {
                TableName = tableName,
                IndexName = indexName,
                KeyConditionExpression = $"{keyName} = :keyValue",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":keyValue", new AttributeValue { S = keyValue } },
                }
            };

            var response = await client.QueryAsync(queryRequest);

            return response.Items;
        }

        /// <summary>
        /// テーブルにアイテムを追加する
        /// </summary>
        /// <param name="tableName">テーブル名</param>
        /// <param name="item">アイテム</param>
        protected async Task Add<T>(string tableName, T item)
        {
            var config = new DynamoDBOperationConfig
            {
                OverrideTableName = tableName
            };

            await context.SaveAsync(item, config);
        }

        /// <summary>
        /// 指定されたプロパティを更新する
        /// </summary>
        /// <param name="tableName">テーブル名</param>
        /// <param name="keyName">キー名</param>
        /// <param name="keyValue">キー値</param>
        /// <param name="updates">更新するプロパティとその値の辞書</param>
        protected async Task UpdateProperties(string tableName, string keyName, string keyValue, Dictionary<string, AttributeValueUpdate> updates)
        {
            var updateRequest = new UpdateItemRequest
            {
                TableName = tableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    { keyName, new AttributeValue { S = keyValue } }
                },
                AttributeUpdates = updates
            };

            await client.UpdateItemAsync(updateRequest);
        }
    }
}
