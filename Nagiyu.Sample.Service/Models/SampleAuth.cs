using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Nagiyu.Common.Auth.Service.Models;
using Nagiyu.Sample.Service.Enums;
using System;
using System.Collections.Generic;

namespace Nagiyu.Sample.Service.Models
{
    public class SampleAuth : UserAuthBase
    {
        /// <summary>
        /// Sample のロール
        /// </summary>
        [DynamoDBProperty]
        public SampleRole SampleRole { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SampleAuth()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="keyValuePairs">キーと値のペア</param>
        public SampleAuth(Dictionary<string, AttributeValue> keyValuePairs) : base(keyValuePairs)
        {
            if (keyValuePairs.TryGetValue(nameof(SampleRole), out var sampleRoleValue))
            {
                if (Enum.TryParse(sampleRoleValue.S, out SampleRole role))
                {
                    SampleRole = role;
                }
                else
                {
                    SampleRole = SampleRole.User; // デフォルト値を設定
                }
            }
            else
            {
                SampleRole = SampleRole.User;
            }
        }
    }
}
