using FoodStore.Models;
using FoodStore.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodStore.Services.Admin
{
    public class FoodService : IFoodService
    {
        private readonly ApplicationDbContext _context;
        private readonly IFoodIngredientRepository _foodIngredientRepository;

        public FoodService(ApplicationDbContext context, IFoodIngredientRepository foodIngredientRepository)
        {
            _context = context;
            _foodIngredientRepository = foodIngredientRepository;
        }

        public async Task<bool> SaveRecipeAsync(int foodId, List<FoodIngredient> ingredients)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var existingIngredients = await _foodIngredientRepository.GetIngredientsByFoodIdAsync(foodId);
                foreach (var ingredient in existingIngredients)
                {
                    _context.FoodIngredient.Remove(ingredient);
                }
                await _context.SaveChangesAsync();

                foreach (var ingredient in ingredients)
                {
                    var newIngredient = new FoodIngredient
                    {
                        FoodId = foodId,
                        IngredientId = ingredient.IngredientId,
                        QuantityRequired = ingredient.QuantityRequired
                    };
                    await _foodIngredientRepository.AddAsync(newIngredient);
                }

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }
    }
}
