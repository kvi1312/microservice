using Microservice.IDP.Infrastructure.Repositories;
using Microservices.IDP.Infrastructure.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Microservices.IDP.Presentation.Controllers
{
    [ApiController]
    [Route("api/permissions/roles/{roleId}/[controller]")]
    public class PermissionsController : ControllerBase
    {
        private readonly IRepositoryManager _repositoryManager;

        public PermissionsController(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetPermission(string roleId)
        {
            var result = await _repositoryManager.PermissionRepository.GetPermissionsByRole(roleId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePermission(string roleId, [FromBody] PermissionAddModel model)
        {
            var result = await _repositoryManager.PermissionRepository.CreatePermission(roleId, model);
            return result != null ? Ok(result) : NoContent();
        }

        [HttpDelete("function/{function}/command/{command}")]
        [ProducesResponseType(typeof(PermissionViewModel), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeletePermission(string roleId, [Required] string function, [Required] string command)
        {
            await _repositoryManager.PermissionRepository.DeletePermission(roleId, function, command);
            return NoContent();
        }

        [HttpPost("update-permissions")]
        [ProducesResponseType(typeof(NoContentResult), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdatePermission(string roleId, [FromBody]IEnumerable<PermissionAddModel> permissions)
        {
            await _repositoryManager.PermissionRepository.UpdatePermissionByRoleId(roleId, permissions);
            return NoContent();
        }
    }
}
