using Microservice.IDP.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Microservices.IDP.Presentation.Controllers
{
    [ApiController]
    [Route("api/roles/{roleId}/[controller]")]
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
    }
}
