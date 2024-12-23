using System.ComponentModel.DataAnnotations;

namespace FoodStore.Models
{
    public class Table
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int Status { get; set; } = 0;
        public List<Order> Orders { get; set; }
    }
}
    