namespace FoodStore.DTO
{
    public class OrderDetailDTO
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int FoodId { get; set; }

        public int Quantity { get; set; }
    }
}
