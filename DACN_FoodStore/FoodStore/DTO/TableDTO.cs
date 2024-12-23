using FoodStore.Models;
using System.ComponentModel.DataAnnotations;

namespace FoodStore.DTO
{
    public class TableDTO
    {
        public int Id { get; set; }
       
        public int Status { get; set; } = 0;
      
    }
}
