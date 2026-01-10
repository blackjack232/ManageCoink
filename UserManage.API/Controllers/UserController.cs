using Microsoft.AspNetCore.Mvc;
using UserManage.Application.Interface.Service;
using UserManage.Domain.Dtos;

namespace UserManage.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController(IUsuarioService service, ILogger<UsuariosController> log) : ControllerBase
{
    private readonly IUsuarioService _service = service;
    private readonly ILogger<UsuariosController> _log = log;

    /// <summary>
    /// Registra un nuevo usuario
    /// </summary>
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
                _log.LogWarning("Validacion de modelo fallida al crear usuario. Errores: {Errors}", string.Join(", ", ModelState.Values));
                return BadRequest(new
                {
                    success = false,
                    message = "Datos de entrada invalidos",
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
                    message = "El cuerpo de la solicitud no puede estar vacio"
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
            _log.LogError(ex, "Error de argumento al crear usuario. Nombre: {Nombre}", req?.Nombre ?? "N/A");

            return BadRequest(new
            {
                success = false,
                message = "Error en los argumentos proporcionados",
                error = ex.Message
            });
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Error interno al crear usuario. Nombre: {Nombre}, Excepción: {ExceptionType}", req?.Nombre ?? "N/A", ex.GetType().Name);
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                success = false,
                message = "Ha ocurrido un error interno en el servidor",
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// Obtiene un usuario por su ID
    /// </summary>
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
                    message = "El ID debe ser un numero mayor a 0"
                });
            }

            var result = await _service.ObtenerUsuarioById(id);

            if (!result.Success)
            {
                _log.LogWarning("Usuario no encontrado. ID: {UserId}, Mensaje: {Message}", id, result.Message);
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
            _log.LogError(ex, "Error interno al obtener usuario. ID: {UserId}, Excepcion: {ExceptionType}", id, ex.GetType().Name);

            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                success = false,
                message = "Ha ocurrido un error interno en el servidor",
                error = ex.Message
            });
        }
    }
}