using System.ComponentModel.DataAnnotations;

namespace Shared.DTOS;

public class CreateProductDto : CreateOrUpdateProductDto
{
    [Required]
    public string No { get; set; }
}