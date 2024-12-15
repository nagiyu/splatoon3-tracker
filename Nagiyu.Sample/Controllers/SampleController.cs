using Microsoft.AspNetCore.Mvc;

namespace Nagiyu.Sample.Controllers
{
    /// <summary>
    /// サンプルコントローラー
    /// </summary>
    public class SampleController : Controller
    {
        /// <summary>
        /// インデックス
        /// </summary>
        public IActionResult Index()
        {
            return View();
        }
    }
}
