using Microsoft.AspNetCore.Mvc;
using UserManage.Application.Interface.Service;
using UserManage.Domain.Constants;
using UserManage.Domain.Dtos;

namespace UserManage.API.Controllers;

/// <summary>
/// Controlador REST para la gestión de usuarios.
/// </summary>
/// <remarks>
/// Proporciona endpoints para crear y consultar usuarios del sistema.
/// Implementa validaciones, logging y manejo estandarizado de respuestas.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
public class UsuarioController(IUsuarioService service, ILogger<UsuarioController> log) : ControllerBase
{
    private readonly IUsuarioService _service = service;
    private readonly ILogger<UsuarioController> _log = log;

    /// <summary>
    /// Registra un nuevo usuario en el sistema.
    /// </summary>
    /// <param name="req">Datos del usuario a registrar</param>
    /// <returns>Usuario creado con código HTTP 201 o error con código 400/500</returns>
    /// <response code="201">Usuario creado exitosamente</response>
    /// <response code="400">Datos de entrada inválidos o validación fallida</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> CrearUsuario([FromBody] ReqUsuarioDto req)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _log.LogWarning(LogMessages.ValidacionFallida, string.Join(", ", ModelState.Values));
                return BadRequest(new
                {
                    success = false,
                    message = AppMessages.DatosInvalidos,
                    errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                });
            }

            if (req == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = AppMessages.CuerpoVacio
                });
            }

            var result = await _service.CrearUsuario(req);

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
                nameof(ObtenerUsuarioById),
                new { id = result.Data!.Id },
                new
                {
                    success = true,
                    message = result.Message,
                    data = result.Data
                }
            );
        }
        catch (ArgumentException ex)
        {
            _log.LogError(ex, LogMessages.ErrorArgumento, req?.Nombre ?? "N/A");

            return BadRequest(new
            {
                success = false,
                message = AppMessages.ArgumentosInvalidos,
                error = ex.Message
            });
        }
        catch (Exception ex)
        {
            _log.LogError(ex, LogMessages.ErrorCrearUsuario, req?.Nombre ?? "N/A", ex.GetType().Name);
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                success = false,
                message = AppMessages.ErrorInterno,
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// Obtiene un usuario por su identificador único.
    /// </summary>
    /// <param name="id">ID del usuario a consultar</param>
    /// <returns>Datos del usuario o error si no existe</returns>
    /// <response code="200">Usuario encontrado exitosamente</response>
    /// <response code="400">ID inválido (menor o igual a 0)</response>
    /// <response code="404">Usuario no encontrado</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> ObtenerUsuarioById(long id)
    {
        try
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message = AppMessages.IdInvalido
                });
            }

            var result = await _service.ObtenerUsuarioById(id);

            if (!result.Success)
            {
                _log.LogWarning(LogMessages.UsuarioNoEncontradoLog, id, result.Message);
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
        catch (Exception ex)
        {
            _log.LogError(ex, LogMessages.ErrorObtenerUsuario, id, ex.GetType().Name);

            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                success = false,
                message = AppMessages.ErrorInterno,
                error = ex.Message
            });
        }
    }
}