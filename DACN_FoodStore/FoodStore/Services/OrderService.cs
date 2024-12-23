using FoodStore.Repositories;

namespace FoodStore.Services
{
    public class OrderService
    {
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IFoodIngredientRepository _foodIngredientRepository;

        public OrderService(
            IOrderDetailRepository orderDetailRepository,
            IIngredientRepository ingredientRepository,
            IFoodIngredientRepository foodIngredientRepository)
        {
            _orderDetailRepository = orderDetailRepository;
            _ingredientRepository = ingredientRepository;
            _foodIngredientRepository = foodIngredientRepository;
        }

        public async Task<bool> CanAcceptOrderAsync(int orderId)  // TODO move code from controller to here
        {
            // // Lấy danh sách món ăn từ chi tiết đơn hàng
            // var orderDetails = await _orderDetailRepository.GetOrderDetailsByOrderIdAsync(orderId);
            // var totalIngredientsNeeded = new Dictionary<int, int>();

            // // Tính tổng số lượng nguyên liệu cần thiết cho mỗi món ăn trong đơn hàng
            // foreach (var orderDetail in orderDetails)
            // {
            //     var foodIngredients = await _foodIngredientRepository.GetFoodIngredientsByFoodIdAsync(orderDetail.FoodId);

            //     foreach (var foodIngredient in foodIngredients)
            //     {
            //         if (totalIngredientsNeeded.ContainsKey(foodIngredient.IngredientId))
            //         {
            //             totalIngredientsNeeded[foodIngredient.IngredientId] += foodIngredient.QuantityRequired;
            //         }
            //         else
            //         {
            //             totalIngredientsNeeded[foodIngredient.IngredientId] = foodIngredient.QuantityRequired;
            //         }
            //     }
            // }

            // // Kiểm tra nếu đủ số lượng nguyên liệu trong kho
            // foreach (var ingredientId in totalIngredientsNeeded.Keys)
            // {
            //     var ingredient = await _ingredientRepository.GetByIdAsync(ingredientId);
            //     if (ingredient == null || ingredient.Quantity < totalIngredientsNeeded[ingredientId])
            //     {
            //         Console.WriteLine($"Not enough ingredient ID {ingredientId}. Required: {totalIngredientsNeeded[ingredientId]}, Available: {ingredient?.Quantity ?? 0}");
            //         return false; // Thiếu nguyên liệu
            //     }
            // }

            // // Nếu đủ nguyên liệu, trừ nguyên liệu trong kho
            // foreach (var ingredientId in totalIngredientsNeeded.Keys)
            // {
            //     var ingredient = await _ingredientRepository.GetByIdAsync(ingredientId);
            //     ingredient.Quantity -= totalIngredientsNeeded[ingredientId];
            //     await _ingredientRepository.UpdateAsync(ingredient);
            // }

            return true; // Có đủ nguyên liệu
        }
    }

}
