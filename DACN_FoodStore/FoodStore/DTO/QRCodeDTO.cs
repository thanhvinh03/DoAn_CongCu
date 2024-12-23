using System.ComponentModel.DataAnnotations;

namespace FoodStore.DTO
{
    public class QRCodeDTO
    {
     
        public int Id { get; set; }
      
        public string Url { get; set; }
        public string ImagePath { get; set; }
        public string Key { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Status { get; set; }
    }
}
