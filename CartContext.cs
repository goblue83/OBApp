using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
namespace OBApp {
    public class CartContext : DbContext {
        public CartContext(DbContextOptions<CartContext> options) : base(options) { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder
                .UseSqlServer(@"Data Source=(localdb)\mssqllocaldb;Initial Catalog=aspnet-53bc9b9d-9d6a-45d4-8429-2a2761773502;");
        }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Item> Items { get; set; }        
    }
}
