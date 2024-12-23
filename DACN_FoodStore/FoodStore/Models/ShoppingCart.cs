namespace FoodStore.Models
{
    public class ShoppingCart
    {
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public void AddItem(CartItem item)
        {
            var existingItem = Items.FirstOrDefault(i => i.FoodId ==
            item.FoodId);
            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                Items.Add(item);
            }
        }
        public void RemoveItem(int foodId)
        {
            Items.RemoveAll(i => i.FoodId == foodId);
        }
        public void Clear()
        {
            Items.Clear();
        }
        // Các phương thức khác...
    }
}
