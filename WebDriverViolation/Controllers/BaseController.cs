using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebDriverViolation.Controllers
{
    public class BaseController : Controller
    {
        public ActionResult ERROR404()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
