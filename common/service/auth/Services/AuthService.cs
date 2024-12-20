using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Nagiyu.Common.Auth.Service.Models;
using Nagiyu.Common.DynamoDBManager.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Nagiyu.Common.Auth.Service.Services
{
    /// <summary>
    /// 認証サービス
    /// </summary>
    public class AuthService : DynamoDBServiceBase
    {
        /// <summary>
        /// HTTP コンテキストアクセサ
        /// </summary>
        private readonly IHttpContextAccessor httpContextAccessor;

        /// <summary>
        /// テーブル名
        /// </summary>
        private readonly string tableName;

        /// <summary>
        /// インデックス名
        /// </summary>
        private readonly string indexName;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="configuration">設定情報</param>
        public AuthService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;

            var accessKey = configuration["Auth:Credentials:AWS:AccessKey"];
            var secretKey = configuration["Auth:Credentials:AWS:SecretKey"];
            var region = configuration["Auth:Credentials:AWS:Region"];
            var serviceUrl = configuration["Auth:Credentials:AWS:ServiceUrl"];

            InitializeClient(accessKey, secretKey, region, serviceUrl);

            tableName = configuration["Auth:DynamoDB:TableName"];
            indexName = configuration["Auth:DynamoDB:IndexName"];
        }

        /// <summary>
        /// ユーザー情報を取得する
        /// </summary>
        /// <returns>ユーザー情報</returns>
        /// <remarks>未認証: null, 認証済: ユーザー情報</remarks>
        public async Task<T> GetUser<T>() where T : UserAuthBase
        {
            var googleUserId = GetGoogleUserId();

            if (string.IsNullOrEmpty(googleUserId))
            {
                return null;
            }

            var items = await GetItems(tableName, indexName, nameof(UserAuthBase.GoogleUserId), googleUserId);

            if (items.Count == 0)
            {
                return null;
            }

            var item = items.FirstOrDefault();

            return (T)Activator.CreateInstance(typeof(T), item);
        }

        /// <summary>
        /// ユーザーを追加する
        /// </summary>
        /// <param name="userName">ユーザー名</param>
        /// <returns>ユーザー ID</returns>
        public async Task<Guid> AddUser(string userName)
        {
            var userId = Guid.NewGuid();
            var googleUserId = GetGoogleUserId();

            var user = new UserAuthBase
            {
                UserId = userId,
                UserName = userName,
                GoogleUserId = googleUserId
            };

            await Add(tableName, user);

            return userId;
        }

        /// <summary>
        /// ユーザー情報を更新する
        /// </summary>
        /// <param name="user">ユーザー情報</param>
        public async Task UpdateUser<T>(T user) where T : UserAuthBase
        {
            // user の全要素を properties に変換
            var properties = new Dictionary<string, AttributeValueUpdate>();

            // 全キーをループして properties に追加
            foreach (var property in user.GetType().GetProperties())
            {
                // DynamoDB には UserId は含めない
                if (property.Name == nameof(UserAuthBase.UserId))
                {
                    continue;
                }

                // プロパティの値を取得
                var value = property.GetValue(user);

                // プロパティの値が null または空文字列の場合はスキップ
                if (value == null || string.IsNullOrEmpty(value.ToString()))
                {
                    continue;
                }

                // プロパティの値を AttributeValueUpdate に変換
                properties.Add(property.Name, new AttributeValueUpdate
                {
                    Action = AttributeAction.PUT,
                    Value = new AttributeValue { S = value.ToString() }
                });
            }

            await UpdateProperties(tableName, nameof(UserAuthBase.UserId), user.UserId.ToString(), properties);
        }

        /// <summary>
        /// Google ユーザー ID を取得する
        /// </summary>
        /// <returns>Google ユーザー ID</returns>
        private string GetGoogleUserId()
        {
            return httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
