using System.ComponentModel.DataAnnotations;

namespace FoodStore.Models
{
    public class Food : DeletableEntity
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }
        public string Image { get; set; }
        public string Type { get; set; } //Đơn vị tính
        public decimal Price { get; set; }
        public int Status { get; set; } = 0;
        public int FoodCategoryId { get; set; }
        public int SoldCount { get; set; } //Moi Thêm số lượng đã bán
        //public int TypeFood { get; set; } // 0 luôn tính tiền, 1 free trong mọi buffet, 2 free buffet 2 nhưng không buffet 1
        public FoodCategory FoodCategorys { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
        public ICollection<BuffetDetail> BuffetDetails { get; set; }
    }
}
