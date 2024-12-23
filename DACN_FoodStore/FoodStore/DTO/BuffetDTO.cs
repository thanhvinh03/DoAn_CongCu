using FoodStore.Models;
using System.ComponentModel.DataAnnotations;

namespace FoodStore.DTO
{
    public class BuffetDTO
    { 
        public int Id
        {
            get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Description { get; set; } //Đơn vị tính
        public decimal Price { get; set; }
        public int Status { get; set; } = 0;

        
    }
}
