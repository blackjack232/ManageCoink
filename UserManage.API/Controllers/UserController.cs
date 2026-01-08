using Microsoft.AspNetCore.Mvc;
using UserManage.Application.Interface.Service;
using UserManage.Domain.Dtos;
namespace UserManage.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController(IUsuarioService usuarioService) : ControllerBase
{
    private readonly IUsuarioService _usuarioService = usuarioService;

    /// <summary>
    /// Registra un nuevo usuario
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegistrarUsuario([FromBody] ReqUsuarioDto dto)
    {
        var result = await _usuarioService.RegistrarUsuarioAsync(dto);

        if (!result.Success)
        {
            return BadRequest(new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        return CreatedAtAction(
            nameof(ObtenerUsuarioPorId),
            new { id = result.Data!.Id },
            new
            {
                success = true,
                message = result.Message,
                data = result.Data
            }
        );
    }

    /// <summary>
    /// Obtiene un usuario por su ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerUsuarioPorId(int id)
    {
        var result = await _usuarioService.ObtenerUsuarioPorIdAsync(id);

        if (!result.Success)
        {
            return NotFound(new
            {
                success = false,
                message = result.Message
            });
        }

        return Ok(new
        {
            success = true,
            data = result.Data
        });
    }
}