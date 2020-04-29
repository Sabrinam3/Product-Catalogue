using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Casestudy.Models
{
    public class UtilityModel
    {
        private AppDbContext _db;
        public UtilityModel(AppDbContext context)//will be sent by controller
        {
            _db = context;
        }

        public bool loadProductTables(string stringJson)
        {
            bool brandsLoaded = false;
            bool productsLoaded = false;
            try
            {
                dynamic objectJson = Newtonsoft.Json.JsonConvert.DeserializeObject(stringJson);
                brandsLoaded = loadBrands(objectJson);
                productsLoaded = loadProducts(objectJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return brandsLoaded && productsLoaded;
        }

        private bool loadBrands(dynamic objectJson)
        {
            bool loadedBrands = false;
            try
            {
                // clear out the old rows
                _db.Brands.RemoveRange(_db.Brands);
                _db.SaveChanges();
                List<String> allBrands = new List<String>();
                foreach (var node in objectJson)
                {
                    allBrands.Add(Convert.ToString(node["BRAND"]));
                }
                // distinct will remove duplicates before we insert them into the db
                IEnumerable<String> brands = allBrands.Distinct<String>();
                foreach (string brname in brands)
                {
                    Brand br = new Brand();
                    br.Name = brname;
                    _db.Brands.Add(br);
                    _db.SaveChanges();
                }
                loadedBrands = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - " + ex.Message);
            }
            return loadedBrands;
        }

        private bool loadProducts(dynamic objectJson)
        {
            bool loadedProducts = false;
            try
            {
                List<Brand> brands = _db.Brands.ToList();
                // clear out the old
                _db.Products.RemoveRange(_db.Products);
                _db.SaveChanges();
                foreach (var node in objectJson)
                {
                    Product item = new Product();
                    item.Id = Convert.ToString(node["ID"]);
                    item.ProductName = Convert.ToString(node["PNAME"]);
                    item.GraphicName = Convert.ToString(node["GNAME"]);
                    item.CostPrice = Convert.ToDecimal(node["PRICE"]);
                    item.MSRP = Convert.ToDecimal(node["MSRP"]);
                    item.QtyOnHand = Convert.ToInt32(node["QTYHAND"]);
                    item.QtyOnBackOrder = Convert.ToInt32(node["QTYORDER"]);
                    item.Description = Convert.ToString(node["DESCRIPTION"]);
                    string br = Convert.ToString(node["BRAND"].Value);
                    // add the FK here
                    foreach (Brand brnd in brands)
                    {
                        if (brnd.Name == br)
                            item.Brand = brnd;
                    }
                    _db.Products.Add(item);
                    _db.SaveChanges();
                }
                loadedProducts = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - " + ex.Message);
            }
            return loadedProducts;
        }
    }
}
