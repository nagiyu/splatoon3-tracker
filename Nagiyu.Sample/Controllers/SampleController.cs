using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Nagiyu.Sample.Web.Controllers
{
    /// <summary>
    /// サンプルコントローラー
    /// </summary>
    public class SampleController : Controller
    {
        /// <summary>
        /// インデックス
        /// </summary>
        [Authorize(Policy = "SamplePolicy")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
