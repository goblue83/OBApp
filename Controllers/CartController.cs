using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

using Microsoft.AspNetCore.JsonPatch;

namespace OBApp.Controllers { 
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase {
        private ICartService cartService;
        public CartController(ICartService iCartService) {
            cartService = iCartService;
        }
        [HttpGet]
        public List<Order> Get() {
            return this.cartService.GetAllOrders();
        }
        [HttpGet("OrderItems/{id}")]
        public List<Order> GetOrderItems(int id) {
            return this.cartService.GetOrderItems(id);
        }
        [HttpGet("{id}")]
        public Order Get(int id) {
            return this.cartService.GetOrder(id);
        }
        [HttpPost]
        public string Post(Order order) {
            this.cartService.AddOrder(order);
            return "Success!";
        }
        [HttpPost("{id}")]
        public string Post(List<Item> items, int id) {
            this.cartService.AddItems(items, id);
            return "Success!";
        }
        [HttpPut("{id}")]
        public string UpdateOrder(int id, Order order) {
            this.cartService.UpdateOrder(order, id);
            return "Order Updated";
        }
        [HttpPatch("Order/id")]
        public IActionResult Patch([FromBody] JsonPatchDocument<Order> patch, int id) {
            Order order = new Order();
            patch.ApplyTo(order, ModelState);
            if (!ModelState.IsValid) {
                return new BadRequestObjectResult(ModelState);
            } else {
                return new ObjectResult(this.cartService.PatchOrder(patch, id));
            }
        }
        [HttpPatch("/{orderid}/Item/{id}")] 
        public IActionResult Patch([FromBody] JsonPatchDocument<Item> patch, int id, int orderid) {
            Item item = new Item();
            patch.ApplyTo(item, ModelState);
            if (!ModelState.IsValid) {
                return new BadRequestObjectResult(ModelState);
            } else {
                return new ObjectResult(this.cartService.PatchItem(patch, id,orderid));
            }
        }
        [HttpDelete("{id}")]
        public string DeleteOrder(int id) {
            this.cartService.DeleteOrder(id);
            return "Order Deleted";
        }
    }
}

