using System.ComponentModel.DataAnnotations;

namespace Shared.DTOS
{
    public class CreateOrUpdateProductDto
    {
        [Required]
        [MaxLength(255, ErrorMessage ="Maximum lenght for Product.Name is 255 characters")]
        public string Name { get; set; }

        [MaxLength(255, ErrorMessage = "Maximum lenght for Product.Summary is 255 characters")]
        public string Summary { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }
}
