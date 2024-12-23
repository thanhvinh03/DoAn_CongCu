namespace FoodStore.Models
{
    public class OrderDetail
    {
        public int OrderId { get; set; }
        public Order Order { get; set; }

        public int FoodId { get; set; }
        public Food Food { get; set; }

        public int Quantity { get; set; }

        public int Status { get; set; } 
    }
}
