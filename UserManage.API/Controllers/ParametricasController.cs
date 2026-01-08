using Microsoft.AspNetCore.Mvc;
using UserManage.Application.Interface.Service;
namespace UserManage.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ParametricasController(IParametricaService parametricaService) : ControllerBase
{
    private readonly IParametricaService _parametricaService = parametricaService;

    /// <summary>
    /// Obtiene la lista de países
    /// </summary>
    [HttpGet("paises")]
    public async Task<IActionResult> ObtenerPaises()
    {
        var result = await _parametricaService.ObtenerPaisesAsync();
        return Ok(new { success = result.Success, data = result.Data });
    }

    /// <summary>
    /// Obtiene los departamentos de un país
    /// </summary>
    [HttpGet("paises/{paisId}/departamentos")]
    public async Task<IActionResult> ObtenerDepartamentos(int paisId)
    {
        var result = await _parametricaService.ObtenerDepartamentosAsync(paisId);

        if (!result.Success)
            return BadRequest(new { success = false, message = result.Message });

        return Ok(new { success = result.Success, data = result.Data });
    }

    /// <summary>
    /// Obtiene los municipios de un departamento
    /// </summary>
    [HttpGet("departamentos/{departamentoId}/municipios")]
    public async Task<IActionResult> ObtenerMunicipios(int departamentoId)
    {
        var result = await _parametricaService.ObtenerMunicipiosAsync(departamentoId);

        if (!result.Success)
            return BadRequest(new { success = false, message = result.Message });

        return Ok(new { success = result.Success, data = result.Data });
    }
}