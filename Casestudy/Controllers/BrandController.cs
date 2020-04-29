using Microsoft.AspNetCore.Mvc;
using Casestudy.Models;
using System.Collections.Generic;
using Casestudy.Utils;
using System;

namespace Casestudy.Controllers
{
    public class BrandController : Controller
    {
        AppDbContext _db;
        public BrandController(AppDbContext context)
        {
            _db = context;
        }
        public IActionResult Index(BrandViewModel vm)
        {
            // only build the catalogue once
            if (HttpContext.Session.Get<List<Brand>>(SessionVariables.Brands) == null)
            {
                // no session information so let's go to the database
                try
                {
                    BrandModel brModel = new BrandModel(_db);
                    // now load the brands
                    List<Brand> brands = brModel.GetAll();
                    HttpContext.Session.Set<List<Brand>>(SessionVariables.Brands, brands);
                    vm.SetBrands(brands);
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "Catalogue Problem - " + ex.Message;
                }
            }
            else
            {
                // no need to go back to the database as information is already in the session
                vm.SetBrands(HttpContext.Session.Get<List<Brand>>(SessionVariables.Brands));
            }
            return View(vm);
        }

        public IActionResult SelectBrand(BrandViewModel vm)
        {
            BrandModel bmModel = new BrandModel(_db);
            ProductModel pModel = new ProductModel(_db);
            List<Product> items = pModel.GetAllByBrand(vm.BrandId);
            List<ProductViewModel> vms = new List<ProductViewModel>();

            if(items.Count > 0)
            {
                foreach(Product pr in items)
                {
                    ProductViewModel pvm = new ProductViewModel();
                    pvm.Qty = 0;
                    pvm.BrandId = pr.BrandId;
                    pvm.Id = pr.Id; // Id is a string
                    pvm.BrandName = bmModel.GetName(pr.BrandId);
                    pvm.ProductName = pr.ProductName;
                    pvm.GraphicName = pr.GraphicName;
                    pvm.CostPrice = pr.CostPrice;
                    pvm.MSRP = pr.MSRP;
                    pvm.QtyOnHand = pr.QtyOnHand;
                    pvm.QtyOnBackOrder = pr.QtyOnBackOrder;
                    pvm.Description = pr.Description;
                    vms.Add(pvm);
                }
                ProductViewModel[] myProducts = vms.ToArray();
                HttpContext.Session.Set<ProductViewModel[]>(SessionVariables.Catalogue, myProducts);
            }
            vm.SetBrands(HttpContext.Session.Get<List<Brand>>(SessionVariables.Brands));
            return View("Index", vm); 

        }
        public ActionResult SelectProduct(BrandViewModel vm)
        {
            Dictionary<string, object> shoppingCart;
            if (HttpContext.Session.Get<Dictionary<string, Object>>(SessionVariables.Cart) == null)
            {
                shoppingCart = new Dictionary<string, object>();
            }
            else
            {
                shoppingCart = HttpContext.Session.Get<Dictionary<string, object>>(SessionVariables.Cart);
            }
            ProductViewModel[] cart = HttpContext.Session.Get<ProductViewModel[]>(SessionVariables.Catalogue);
            String retMsg = "";
            foreach (ProductViewModel item in cart)
            {
                if (item.Id == vm.ProductId)
                {
                    if (vm.Qty > 0) // update only selected item
                    {
                        item.Qty = vm.Qty;
                        retMsg = vm.Qty + " - item(s) Added!";
                        shoppingCart[item.Id] = item;
                    }
                    else
                    {
                        item.Qty = 0;
                        shoppingCart.Remove(item.Id);
                        retMsg = "item(s) Removed!";
                    }
                    vm.BrandId = item.BrandId;
                    break;
                }
            }
            ViewBag.AddMessage = retMsg;
            HttpContext.Session.Set<Dictionary<string, Object>>(SessionVariables.Cart, shoppingCart);
            vm.SetBrands(HttpContext.Session.Get<List<Brand>>(SessionVariables.Brands));
            return View("Index", vm);
        }

    }
}