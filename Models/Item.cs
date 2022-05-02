using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OBApp {
    public class Item {
        [Key]
        public int id { get; set; }
        public string Name { get; set; } 
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Total {
            get {
                return (Price * Quantity); 
            }
        }
        [ForeignKey("OrderId")]
        public int OrderId { get; set; }
    }
}
