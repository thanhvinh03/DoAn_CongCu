namespace FoodStore.DTO
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public int TableId { get; set; }
        public bool Status { get; set; }
        public double TotalPrice { get; set; }
        public bool StatusPay { get; set; }

        public List<OrderDetailDTO> listOrderDetail { get; set; }
    }
}
