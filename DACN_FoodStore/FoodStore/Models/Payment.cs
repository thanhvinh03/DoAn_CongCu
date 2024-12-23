using System.ComponentModel.DataAnnotations;

namespace FoodStore.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        public List<Invoice> Invoices { get; set; } //Hóa đơn
    }
}
