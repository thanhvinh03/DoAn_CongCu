namespace FoodStore.Models
{
    public class BuffetDetail
    {
        public int Id { get; set; }
        public int BuffetId { get; set; }
        public Buffet Buffet { get; set; }

        public int FoodId { get; set; }
        public Food Food { get; set; }

    }
}
