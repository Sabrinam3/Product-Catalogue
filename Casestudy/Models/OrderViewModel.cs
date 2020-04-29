using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Casestudy.Models
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }
        public string ProductId { get; set; }
        public decimal MSRP { get; set; }
        public int QtyOrdered { get; set; }
        public int QtySold { get; set; }
        public int QtyBackOrdered { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        public string DateCreated { get; set; }
        public decimal Extended { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
    }
}
