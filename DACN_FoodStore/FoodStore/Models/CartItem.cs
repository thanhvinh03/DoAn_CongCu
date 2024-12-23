namespace FoodStore.Models
{
    public class CartItem
    {
        public int FoodId { get; set; }
        public string Name { get; set; }

        public string Picture { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
