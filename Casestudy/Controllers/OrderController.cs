using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using Casestudy.Utils;
using Casestudy.Models;
using System.Collections.Generic;

namespace Casestudy.Controllers
{
    public class OrderController : Controller
    {
        AppDbContext _db;
        public OrderController(AppDbContext context)
        {
            _db = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public ActionResult ClearCart() // clear out current cart
        {
            HttpContext.Session.Remove(SessionVariables.Cart);
            HttpContext.Session.Set<String>(SessionVariables.Message, "Cart Cleared");
            return Redirect("/Home");
        }

        // Add the order, pass the session variable info to the db
        public ActionResult AddOrder()
        {
            OrderModel model = new OrderModel(_db);
            int retVal = -1;
            bool goodsBackOrdered = false;
            KeyValuePair<int, bool> modelReturn;
            string retMessage = "";
            try
            {
                Dictionary<string, object> orderItems = HttpContext.Session.Get<Dictionary<string,
                object>>(SessionVariables.Cart);
                modelReturn = model.AddOrder(orderItems,
                HttpContext.Session.Get<ApplicationUser>(SessionVariables.User));
                retVal = modelReturn.Key;
                goodsBackOrdered = modelReturn.Value;

                if (retVal > 0 && !goodsBackOrdered) // Order Added, no goods back ordered
                {
                    retMessage = "Order " + retVal + " Created!";
                }
                else if(retVal > 0 && goodsBackOrdered) //Order Added, goods backordered
                {
                    retMessage = "Order " + retVal + " Created! Some goods were backordered!";
                }
                else // problem
                {
                    retMessage = "Order not added, try again later";
                }
            }
            catch (Exception ex) // big problem
            {
                retMessage = "Order was not created, try again later! - " + ex.Message;
            }
            HttpContext.Session.Remove(SessionVariables.Cart); // clear out current order once persisted
            HttpContext.Session.SetString(SessionVariables.Message, retMessage);
            return Redirect("/Home");
        }

        [Route("[action]")] //whatever the method is called is the route
        public IActionResult GetOrders()
        {
            OrderModel model = new OrderModel(_db);
            ApplicationUser user = HttpContext.Session.Get<ApplicationUser>(SessionVariables.User);
            return Ok(model.GetAllForUser(user));
        }

        [Route("[action]/{oid:int}")]
        public IActionResult GetOrderDetails(int oid)
        {
            OrderModel model = new OrderModel(_db);
            ApplicationUser user = HttpContext.Session.Get<ApplicationUser>(SessionVariables.User);
            return Ok(model.GetOrderDetails(oid, user.Id));
        }
        public IActionResult List()
        {
            // they can't list Orders if they're not logged on
            if (HttpContext.Session.Get<ApplicationUser>(SessionVariables.User) == null)
            {
                return Redirect("/Login");
            }
            return View("List");
        }

        }
    }