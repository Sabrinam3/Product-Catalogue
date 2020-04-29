using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Casestudy.Models
{
    public class BrandViewModel
    {
        public BrandViewModel() { }
        public string BrandName { get; set; }
        public string ProductId { get; set; } //this represents the productId
        public List<Brand> _brands { get; set; }
        public int BrandId { get; set; }
        public int Qty { get; set; }
        
        //Host the child items directly
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<SelectListItem> GetBrands()
        {
            return _brands.Select(brand => new SelectListItem
            {
                Text = brand.Name,
                Value
            = brand.Id.ToString()
            });
        }
        
        public void SetBrands(List<Brand> brnds)
        {
            _brands = brnds;
        }
    }
}
