using System.ComponentModel.DataAnnotations;

namespace FoodStore.Models
{
    public class Buffet
    {

        [Key]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }
        public string Image { get; set; }
        public string Description { get; set; } //Đơn vị tính
        public decimal Price { get; set; }
        public int Status { get; set; } = 0;
        public ICollection<BuffetDetail> BuffetDetails { get; set; }
    }
}
