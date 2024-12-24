using Microsoft.AspNetCore.Http;
using Nagiyu.Common.Auth.Service.Models;
using Nagiyu.Common.Auth.Service.Services;
using System.Threading.Tasks;

namespace Nagiyu.Auth.Web.Middlewares
{
    /// <summary>
    /// Google 認証ミドルウェア
    /// </summary>
    public class GoogleAuthMiddleware
    {
        /// <summary>
        /// 次のミドルウェアを呼び出すためのリクエストデリゲート
        /// </summary>
        private readonly RequestDelegate _next;

        /// <summary>
        /// 認証サービス
        /// </summary>
        private readonly AuthService authService;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="next">デリゲート</param>
        /// <param name="authService">認証サービス</param>
        public GoogleAuthMiddleware(RequestDelegate next, AuthService authService)
        {
            _next = next;

            this.authService = authService;
        }

        /// <summary>
        /// ミドルウェアの実行
        /// </summary>
        /// <param name="context">HTTP コンテキスト</param>
        public async Task InvokeAsync(HttpContext context)
        {
            // ヘルスチェックのリクエストなら認証スキップ
            if (context.Request.Path.StartsWithSegments("/health"))
            {
                await _next(context); // 認証をスキップ
                return;
            }

            // 認証状態の確認
            if (context.User.Identity?.IsAuthenticated ?? false)
            {
                var user = await authService.GetUser<UserAuthBase>();

                if (user == null)
                {
                    await authService.AddUser(string.Empty);
                }
            }

            await _next(context);
        }
    }
}
