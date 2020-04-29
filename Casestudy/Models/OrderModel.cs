using Casestudy.Utils;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Casestudy.Models
{
    public class OrderModel
    {
        private AppDbContext _db;
        public OrderModel(AppDbContext ctx)
        {
            _db = ctx;
        }
        public List<Order> GetAllForUser(ApplicationUser user)
        {
            return _db.Orders.Where(order => order.UserId == user.Id).ToList<Order>();
        }
        public KeyValuePair<int, bool> AddOrder(Dictionary<string, object> items, ApplicationUser user)
        {
            int orderId = -1;
            bool goodsBackordered = false;
            using (_db)
            {
                // we need a transaction as multiple entities involved
                using (var _trans = _db.Database.BeginTransaction())
                {
                    try
                    {
                        Order order = new Order();
                        order.UserId = user.Id;
                        order.OrderDate = System.DateTime.Now;

                        // calculate the totals and then add the order row to the table
                        foreach (var key in items.Keys)
                        {
                            ProductViewModel item =
                            JsonConvert.DeserializeObject<ProductViewModel>(Convert.ToString(items[key]));
                            if (item.Qty > 0)
                            {
                                order.OrderAmount += item.Qty * item.MSRP;
                            }
                        }
                        order.OrderAmount *= 1.13M;
                        _db.Orders.Add(order);
                        _db.SaveChanges();

                        //variable for keeping track of real order total - i.e. the goods sold.
                        decimal goodsSoldTotal = 0;
                        // add each product to the orderitems table
                        foreach (var key in items.Keys)
                        {
                            ProductViewModel item =
                            JsonConvert.DeserializeObject<ProductViewModel>(Convert.ToString(items[key]));
                            Product product = _db.Products.Find(item.Id);
                            if (item.Qty > 0)
                            {
                                //Get how many of the item is currently in the database
                                int currentQtyStock = item.QtyOnHand;
                                OrderLineItem oItem = new OrderLineItem();
                                oItem.ProductId = item.Id;
                                oItem.OrderId = order.Id;
                                oItem.SellingPrice = item.MSRP;
                                oItem.QtyOrdered = item.Qty;
                                //Check if the qty ordered of this product is bigger or smaller than the current qty on hand
                                if (item.Qty < currentQtyStock)
                                {
                                    //Decrease the QtyOnHand in the products table by Qty
                                    product.QtyOnHand = currentQtyStock - item.Qty;
                                    //Order Item Quantity Sold is Qty
                                    oItem.QtySold = item.Qty;
                                    //Order Item Qty Back Ordered is 0
                                    oItem.QtyBackOrdered = 0;
                                    product.QtyOnBackOrder = item.QtyOnBackOrder;
                                }
                                else
                                {
                                    goodsBackordered = true;
        
                                    //QtyOnHand in the products table is 0
                                    product.QtyOnHand = 0;
                                    //QtyOnBackOrder for the Products table is QtyOnBackOrder + 
                                    // Qty - currentQtyStock
                                    product.QtyOnBackOrder = item.QtyOnBackOrder + (item.Qty - currentQtyStock);
                                    //Order item Quantity Sold is currentQtyStock 
                                    oItem.QtySold = currentQtyStock;
                                    //Order item QtyBackOrdered is Qty - currentQtyStock
                                    oItem.QtyBackOrdered = item.Qty - currentQtyStock;                               
                                }
                                _db.OrderLineItems.Add(oItem);
                                //update products table
                                _db.Products.Update(product);
                                _db.SaveChanges();

                                //increase goods sold amount
                                goodsSoldTotal += oItem.QtySold * oItem.SellingPrice; 
                            }
                        }
                        //if the goodsSoldTotal is different than the original order total, update this
                        if(goodsSoldTotal != order.OrderAmount)
                        {
                            order.OrderAmount = goodsSoldTotal * 1.13M;
                            _db.Orders.Update(order);
                            _db.SaveChanges();
                        }
                        _trans.Commit();
                        orderId = order.Id;
                    }
                    catch (Exception ex)
                    {
                        orderId = -1;
                        Console.WriteLine(ex.Message);
                        _trans.Rollback();
                    }
                }
            }
            return new KeyValuePair<int, bool>(orderId, goodsBackordered);
        }

        public List<OrderViewModel> GetOrderDetails(int oid, string uid)
        {
            List<OrderViewModel> allDetails = new List<OrderViewModel>();
            // LINQ way of doing INNER JOINS
            var results = from o in _db.Set<Order>()
                          join oli in _db.Set<OrderLineItem>() on o.Id equals oli.OrderId
                          join p in _db.Set<Product>() on oli.ProductId equals p.Id
                          where (o.UserId == uid && o.Id == oid)
                          select new OrderViewModel
                          {
                              OrderId = o.Id,
                              UserId = uid,
                              Name = p.ProductName,
                              ProductId = oli.ProductId,
                              MSRP = oli.SellingPrice,
                              QtyOrdered = oli.QtyOrdered,
                              QtySold = oli.QtySold,
                              QtyBackOrdered = oli.QtyBackOrdered,
                              Extended = oli.SellingPrice * oli.QtySold,
                              Total = o.OrderAmount,
                              Subtotal = 0,
                              Tax = 0,
                              DateCreated = o.OrderDate.ToString("yyyy/MM/dd - hh:mm tt")
                          };

            allDetails = results.ToList<OrderViewModel>();
            return allDetails;
        }
    }
}