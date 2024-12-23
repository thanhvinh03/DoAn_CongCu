using System.ComponentModel.DataAnnotations;

namespace FoodStore.DTO
{
    public class FoodDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Type { get; set; } //Đơn vị tính
        public decimal Price { get; set; }
        public int Status { get; set; } = 0;
        public int FoodCategoryId { get; set; }
        public int SoldCount { get; set; }
    }
}
