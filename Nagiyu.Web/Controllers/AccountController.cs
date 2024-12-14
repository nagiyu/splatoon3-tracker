using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;

namespace Nagiyu.Web.Controllers
{
    /// <summary>
    /// アカウントコントローラー
    /// </summary>
    public class AccountController : Controller
    {
        /// <summary>
        /// ログイン
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
        public IActionResult Login()
        {
            var redirectUrl = Url.Action("Index", "Home");
            return Challenge(new AuthenticationProperties { RedirectUri = redirectUrl }, GoogleDefaults.AuthenticationScheme);
        }
    }
}
