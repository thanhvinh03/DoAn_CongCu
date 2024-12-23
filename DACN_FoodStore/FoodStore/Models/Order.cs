using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace FoodStore.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int TableId { get; set; }
        public Table Tables { get; set; }
        public bool Status { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;

        public double TotalPrice { get; set; }
        public bool StatusPay { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
