using Microsoft.AspNetCore.Http;
using Nagiyu.Common.Auth.Models;
using Nagiyu.Common.Auth.Services;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Nagiyu.Web.Middlewares
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
            // 認証状態の確認
            if (context.User.Identity?.IsAuthenticated ?? false)
            {
                var claims = context.User.Claims;
                var googleUserId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                var userId = await GetUserId(googleUserId);

                var identity = new ClaimsIdentity(context.User.Identity);
                identity.AddClaim(new Claim(nameof(UserAuthBase.UserId), userId.ToString()));

                context.User = new ClaimsPrincipal(identity);
            }

            await _next(context);
        }

        /// <summary>
        /// ユーザー ID を取得する
        /// </summary>
        /// <param name="googleUserId">Google ユーザー ID</param>
        /// <returns>ユーザー ID</returns>
        private async Task<Guid> GetUserId(string googleUserId)
        {
            if (!await authService.IsExistUserByGoogle(googleUserId))
            {
                return await authService.AddUserByGoogle(googleUserId);
            }

            return Guid.Parse(await authService.GetUserIdByGoogle(googleUserId));
        }
    }
}
