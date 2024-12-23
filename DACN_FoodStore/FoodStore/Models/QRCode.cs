using System.ComponentModel.DataAnnotations;

namespace FoodStore.Models
{
    public class QRCode
    {

        [Key]
        public int Id { get; set; }
        [Required]
        public string Url { get; set; }      
        public string ImagePath { get; set; } 

        public int IdTable { get; set; }
        public string Key { get; set; }     
        public DateTime CreatedAt { get; set; }
        public bool Status { get; set; }

    }
}
