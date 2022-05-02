using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OBApp { 
    public class Order {
        [Key]
        public int OrderId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public decimal OrderPrice { get; set; }
        public IList<Item> Items { get; set; } = new List<Item>();
    }
}
