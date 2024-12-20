using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Nagiyu.Common.Auth.Service.Models;
using Nagiyu.Common.Auth.Service.Services;
using System.Threading.Tasks;

namespace Nagiyu.Auth.Web.Controllers
{
    /// <summary>
    /// アカウントコントローラー
    /// </summary>
    public class AccountController : Controller
    {
        /// <summary>
        /// 認証サービス
        /// </summary>
        private readonly AuthService authService;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="authService">認証サービス</param>
        public AccountController(AuthService authService)
        {
            this.authService = authService;
        }

        /// <summary>
        /// ログイン
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
        public async Task<IActionResult> Login()
        {
            var user = await authService.GetUser<UserAuthBase>();

            if (user == null)
            {
                var redirectUrl = Url.Action("Login");
                return Challenge(new AuthenticationProperties { RedirectUri = redirectUrl }, GoogleDefaults.AuthenticationScheme);
            }

            if (string.IsNullOrEmpty(user.UserName))
            {
                return RedirectToAction("Register");
            }

            return Redirect("/");
        }

        /// <summary>
        /// 登録
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            var user = await authService.GetUser<UserAuthBase>();

            if (user == null)
            {
                var redirectUrl = Url.Action("Register");
                return Challenge(new AuthenticationProperties { RedirectUri = redirectUrl }, GoogleDefaults.AuthenticationScheme);
            }

            if (string.IsNullOrEmpty(user.UserName))
            {
                return View();
            }

            return Redirect("/");
        }

        /// <summary>
        /// 登録
        /// </summary>
        /// <param name="userName">ユーザー名</param>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public async Task<IActionResult> Register(string userName)
        {
            var user = await authService.GetUser<UserAuthBase>();

            user.UserName = userName;

            await authService.UpdateUser(user);

            return Redirect("/");
        }

        [HttpGet]
        [Route("api/account/user")]
        public async Task<IActionResult> GetUser()
        {
            var user = await authService.GetUser<UserAuthBase>();

            return Json(user);
        }

        /// <summary>
        /// アクセス拒否
        /// </summary>
        /// <param name="returnUrl">URL</param>
        /// <returns>ActionResult</returns>
        [HttpGet]
        public IActionResult AccessDenied(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
    }
}
