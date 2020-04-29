using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Casestudy.Models;

namespace Casestudy.Controllers
{
    public class DataController : Controller
    {
        AppDbContext _ctx;
        public DataController(AppDbContext context)
        {
            _ctx = context;
        }
        public IActionResult Index()
        {
            UtilityModel util = new UtilityModel(_ctx);
            var json =  getProductJson();
            try
            {
                util.loadProductTables(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return View();
        }

        public String getProductJson()
        {
            var json = System.IO.File.ReadAllText("products.json");
            return json;
        }
    }
}