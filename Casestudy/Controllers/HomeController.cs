using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Casestudy.Models;
using Casestudy.Utils;
using Microsoft.AspNetCore.Authorization;

namespace Casestudy.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString(SessionVariables.LoginStatus) == null)
            {
                HttpContext.Session.SetString(SessionVariables.LoginStatus, "not logged in");
            }
            if (HttpContext.Session.GetString(SessionVariables.LoginStatus) == "not logged in")
            {
                if (HttpContext.Session.GetString(SessionVariables.Message) == null)
                {
                    HttpContext.Session.SetString(SessionVariables.Message, "Please login!");
                }
            }
            ViewBag.Status = HttpContext.Session.GetString(SessionVariables.LoginStatus);
            ViewBag.Message = HttpContext.Session.GetString(SessionVariables.Message);
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
