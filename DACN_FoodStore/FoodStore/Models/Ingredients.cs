using System.ComponentModel.DataAnnotations;

namespace FoodStore.Models
{
    public class Ingredients
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên nguyên liệu là bắt buộc.")]
        [StringLength(255, ErrorMessage = "Tên nguyên liệu không được vượt quá 255 ký tự.")]
        public string Name { get; set; }

        public string? Image { get; set; }

        [Required(ErrorMessage = "Số lượng còn lại là bắt buộc.")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Đơn vị là bắt buộc.")]
        [StringLength(50, ErrorMessage = "Đơn vị không được vượt quá 50 ký tự.")]
        public string Unit { get; set; }

        [Required(ErrorMessage = "Ngày nhập hàng là bắt buộc.")]
        [DataType(DataType.Date)]
        public DateTime ImportDate { get; set; }

        [Required(ErrorMessage = "Ngày hết hạn là bắt buộc.")]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(Ingredients), "ValidateExpiryDate")]
        public DateTime ExpiryDate { get; set; }

        public bool IsDeleted { get; set; } = false;

        // Custom validation method
        public static ValidationResult? ValidateExpiryDate(DateTime expiryDate, ValidationContext context)
        {
            var instance = context.ObjectInstance as Ingredients;
            if (instance != null && expiryDate < instance.ImportDate)
            {
                return new ValidationResult("Ngày hết hạn phải lớn hơn hoặc bằng ngày nhập hàng.");
            }

            return ValidationResult.Success;
        }
    }
}
