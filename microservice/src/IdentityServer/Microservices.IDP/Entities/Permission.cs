using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IdentityServer.Common.Domain;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Entities;

public class Permission : EntityBase<long>
{
    [Key]
    [MaxLength(50)]
    [Column(TypeName = "varchar(50)")]
    public string Function { get; set; }
    [Key]
    [MaxLength(50)]
    [Column(TypeName = "varchar(50)")]
    public string Command { get; set; }
    [ForeignKey("RoleId")]
    public string RoleId { get; set; }
    public virtual IdentityRole Role { get; set; }

    public Permission(string function, string command, string roleId)
    {
        Function = function;
        Command = command;
        RoleId = roleId;
    }
}