using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using System.Web;
namespace OBApp {
    public interface ICartService {
        List<Order> GetAllOrders();
        Order GetOrder(int id);
        string AddOrder(Order order);
        string AddItems(List<Item> items, int id);
        List<Item> GetItems(int id);
        List<Order> GetOrderItems(int id);
        string UpdateOrder (Order order, int id);
        string DeleteOrder(int id);
        string PatchOrder(JsonPatchDocument<Order> obj, int id);
        string PatchItem(JsonPatchDocument<Item> obj, int id, int oid);
    }

    public class CartService : ICartService {
        private readonly ICartRepository cartRepo;

        public CartService(ICartRepository iCartRepo) {
            cartRepo = iCartRepo;
            System.Diagnostics.Debug.WriteLine("service Construct");
        }

        public string AddOrder(Order order) {
            List<Item> items = (List<Item>)order.Items;
            decimal price = 0;
            foreach (var item in items) {
                price += item.Total;
            }
            order.OrderPrice = price;
            return cartRepo.AddOrder(order);
        }
        public string AddItems(List<Item> items, int id) {
            foreach (Item item in items) {
                item.OrderId = id;
                cartRepo.AddItem(item);
            }
            //update order price after item add
            List<Item> totalItems = cartRepo.GetItems(id);
            decimal price = 0;
            foreach (var item in totalItems) {
                price += item.Total;
            }
            Order order = new Order();
            order.OrderPrice=price;
            order.OrderId = id;
            return cartRepo.UpdateOrder(order);
        }
        public List<Order> GetAllOrders() => cartRepo.GetOrders(); 
        public Order GetOrder(int id) => cartRepo.GetOrder(id);
        public List<Item> GetItems(int id) => cartRepo.GetItems(id);
        public List<Order> GetOrderItems(int id) {
            List<Order> order = cartRepo.GetOrderItems(id);
            return order;
        }
        public string UpdateOrder(Order order, int id) {
            IList<Item> items = order.Items;
            decimal price = 0;
            foreach (Item item in items) {
                price += item.Total;
            }
            order.OrderPrice = price;
            return cartRepo.UpdateOrder(order);
        }
        public string DeleteOrder(int id) {
            return cartRepo.DeleteOrder(id);
        }

        public string PatchOrder(JsonPatchDocument<Order> obj, int id) {
            Order order = cartRepo.GetOrder(id);
            if (order != null) {
                obj.ApplyTo(order);
                return cartRepo.UpdateOrder(order);
            } else {
                return "Order not found";
            }
        }
        public string PatchItem(JsonPatchDocument<Item> obj, int id, int oid) {
           Item dbItem = cartRepo.GetItem(id, oid);
            if (dbItem != null) {
                obj.ApplyTo(dbItem);
                cartRepo.UpdateItem(dbItem, id, oid);
                //update order price after item add
                List<Item> totalItems = cartRepo.GetItems(oid);
                decimal price = 0;
                foreach (var item in totalItems) {
                    price += item.Total;
                }
                Order order = new Order();
                order.OrderPrice = price;
                order.OrderId = oid;
                return cartRepo.UpdateOrder(order);
            }
            else {
                return "Item not found";
            }
        }
    }
}
