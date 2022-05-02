using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
namespace OBApp {
    public interface ICartRepository {
        List<Order> GetOrders();
        Order GetOrder(int id);
        List<Order> GetOrderItems(int id);
        string AddOrder(Order order);
        string AddItem(Item items);
        List<Item> GetItems(int id);
        string UpdateOrder(Order order);
        string UpdateItem(Item item, int id, int oid);
        string DeleteOrder(int id);
        Item GetItem(int id, int oid);
    }
    public class CartRepository : ICartRepository {
        private CartContext cartContext;

        public CartRepository(CartContext CartContext) {
            this.cartContext = CartContext;
        }
        public List<Order> GetOrders() {
            return cartContext.Orders.ToList();
        }
        public List<Item> GetItems(int id) {
            return cartContext.Items.Where(i => i.OrderId == id).ToList();
        }
        public Item GetItem(int id, int oid) {
            return cartContext.Items.Where(i => i.OrderId == oid && i.id == id).FirstOrDefault();
        }
        public Order GetOrder(int id) {
            return cartContext.Orders.Where(o => o.OrderId == id).FirstOrDefault();
        }
        public List<Order> GetOrderItems(int id) {
            var order = cartContext.Orders
                .Where(o => o.OrderId == id)
                .Include(m => m.Items)
                .ToList();
            return order;
        }
        public string AddOrder(Order order) {
            cartContext.Orders.Add(order);
            try {
                cartContext.SaveChanges();
                return "success";
            }
            catch (Exception ex) {
                Log.Error("Error Adding Order: exception = {}", ex);
                return "Error Adding Order";
            }
        }

        public string AddItem(Item item) {
            cartContext.Items.Add(item);
            try {
                cartContext.SaveChanges();
                return "success";
            }
            catch (Exception ex) {
                Log.Error("Error Adding Item: exception = {}", ex);
                return "Error Adding Item";
            }
        }
        public string UpdateOrder(Order order) {
            Order dbOrder = cartContext.Orders.Where(o => o.OrderId == order.OrderId).FirstOrDefault();
            if (dbOrder != null) {
                foreach (PropertyInfo info in dbOrder.GetType().GetProperties()) {
                    if (info.GetValue(order, null) == null) {
                        info.SetValue(order, info.GetValue(dbOrder, null), null);
                    }
                }
                cartContext.Entry(dbOrder).CurrentValues.SetValues(order);
                // update items if included
                if (order.Items != null) {
                    if (order.Items.Count > 0) {
                        List<Item> updatedItems = order.Items.ToList();
                        foreach (Item item in updatedItems) {
                            List<Item> dbItemList = cartContext.Items.Where(i => i.OrderId == order.OrderId && i.id == item.id).ToList();
                            if (dbItemList.Count > 0) {
                                Item dbItem = dbItemList[0];
                                foreach (PropertyInfo info in dbItem.GetType().GetProperties()) {
                                    if (info.GetValue(item, null) == null) {
                                        info.SetValue(item, info.GetValue(dbItem, null), null);
                                    }
                                }
                                cartContext.Entry(dbItem).CurrentValues.SetValues(item);
                            }
                        }
                    }
                }
                try {
                    cartContext.SaveChanges();
                    return "Success";
                }
                catch (Exception ex) {
                    Log.Error("Error Updating Order: exception = {}", ex);
                    return "Error Updating Order";
                }
            } else {
                return "Order not found";
            }
        }
        public string DeleteOrder(int id) {
            Order order = cartContext.Orders.Where(o => o.OrderId == id).FirstOrDefault();
            if (order != null) {
                cartContext.Remove(order);
                try {
                    cartContext.SaveChanges();
                    return "Order Removed";
                }
                catch (Exception ex) {
                    Log.Error("Error Removing Order: exception = {}", ex);
                    return "Error Removing Order";
                }
            } else {
                return "Order not found";
            }
        }
        public string UpdateItem(Item item, int id, int oid) {
            Item dbItem = cartContext.Items.Where(i => i.OrderId == oid && i.id == id).FirstOrDefault();
            if (dbItem != null) {
                foreach (PropertyInfo info in dbItem.GetType().GetProperties()) {
                    if (info.GetValue(item, null) == null) {
                        info.SetValue(item, info.GetValue(dbItem, null), null);
                    }
                }
                cartContext.Entry(dbItem).CurrentValues.SetValues(item);
                try {
                    cartContext.SaveChanges();
                    return "Success";
                }
                catch (Exception ex) {
                    Log.Error("Error Updating Order: exception = {}", ex);
                    return "Error Updating Order";
                }
            } else {
                return "Item not found";
            }
        }
    }
}
