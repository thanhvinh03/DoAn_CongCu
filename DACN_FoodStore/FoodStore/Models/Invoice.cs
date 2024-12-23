using System.ComponentModel.DataAnnotations;

namespace FoodStore.Models
{
    public class Invoice: DeletableEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime FinishedAt { get; set; } = DateTime.UtcNow;
        public bool Status { get; set; } = false; // mới thêm
        public double Charge { get; set; } //mới thêm
        public double Price { get; set; }
        public int PaymentId { get; set; }
        public Payment Payments { get; set; }
    }
}
