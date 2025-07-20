using System.ComponentModel.DataAnnotations;

namespace Microservices.IDP.Infrastructure.ViewModels;

public class PermissionAddModel
{
    [Required]
    public string Function { get; set; }
    [Required]
    public string Command { get; set; }
   
}