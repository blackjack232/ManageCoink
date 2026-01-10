using Microsoft.AspNetCore.Mvc;
using UserManage.Application.Interface.Service;

namespace UserManage.API.Controllers;

/// <summary>
/// Controlador para la gestión de datos paramétricos del sistema.
/// Proporciona endpoints para consultar países, departamentos y municipios.
/// </summary>
/// <remarks>
/// Constructor del controlador de paramétricas.
/// </remarks>
/// <param name="service">Servicio de gestión de paramétricas</param>
/// <param name="logger">Servicio de logging</param>
[ApiController]
[Route("api/[controller]")]
public class ParametricasController(IParametricaService service, ILogger<ParametricasController> logger) : ControllerBase
{
    private readonly IParametricaService _service = service;
    private readonly ILogger<ParametricasController> _logger = logger;

    /// <summary>
    /// Obtiene la lista completa de países disponibles en el sistema.
    /// </summary>
    /// <returns>Lista de países con código 200, o error 500</returns>
    /// <response code="200">Lista de países obtenida exitosamente</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("pais")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> ObtenerPais()
    {
        try
        {
            var result = await _service.ObtenerPais();

            if (!result.Success)
            {
                _logger.LogWarning("Error al obtener países: {Message}", result.Message);

                return StatusCode(StatusCodes.Status500InternalServerError, new
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
            _logger.LogError(ex, "Error interno al obtener países. Excepción: {ExceptionType}", ex.GetType().Name);

            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                success = false,
                message = "Ha ocurrido un error interno en el servidor",
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// Obtiene los departamentos pertenecientes a un país específico.
    /// </summary>
    /// <param name="paisId">Identificador del país</param>
    /// <returns>Lista de departamentos con código 200, error 400 si el ID es inválido, o error 500</returns>
    /// <response code="200">Lista de departamentos obtenida exitosamente</response>
    /// <response code="400">ID de país inválido o no encontrado</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("pais/{paisId}/departamento")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> ObtenerDepartamento(long paisId)
    {
        try
        {
            if (paisId <= 0)
            {
                _logger.LogWarning("Intento de consultar departamentos con PaisId invalido: {PaisId}", paisId);

                return BadRequest(new
                {
                    success = false,
                    message = "El ID del país debe ser un numero mayor a 0"
                });
            }

            var result = await _service.ObtenerDepartamento(paisId);

            if (!result.Success)
            {
                _logger.LogWarning("Error al obtener departamentos. PaisId: {PaisId}, Mensaje: {Message}", paisId, result.Message);

                return BadRequest(new
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
            _logger.LogError(ex, "Error interno al obtener departamentos. PaisId: {PaisId}, Excepción: {ExceptionType}", paisId, ex.GetType().Name);

            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                success = false,
                message = "Ha ocurrido un error interno en el servidor",
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// Obtiene los municipios pertenecientes a un departamento específico.
    /// </summary>
    /// <param name="departamentoId">Identificador del departamento</param>
    /// <returns>Lista de municipios con código 200, error 400 si el ID es inválido, o error 500</returns>
    /// <response code="200">Lista de municipios obtenida exitosamente</response>
    /// <response code="400">ID de departamento inválido o no encontrado</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("departamento/{departamentoId}/municipio")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> ObtenerMunicipios(long departamentoId)
    {
        try
        {
            if (departamentoId <= 0)
            {
                _logger.LogWarning("Intento de consultar municipios con DepartamentoId inválido: {DepartamentoId}", departamentoId);

                return BadRequest(new
                {
                    success = false,
                    message = "El ID del departamento debe ser un número mayor a 0"
                });
            }

            var result = await _service.ObtenerMunicipio(departamentoId);

            if (!result.Success)
            {
                _logger.LogWarning("Error al obtener municipios. DepartamentoId: {DepartamentoId}, Mensaje: {Message}", departamentoId, result.Message);

                return BadRequest(new
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
            _logger.LogError(ex, "Error interno al obtener municipios. DepartamentoId: {DepartamentoId}, Excepción: {ExceptionType}", departamentoId, ex.GetType().Name);

            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                success = false,
                message = "Ha ocurrido un error interno en el servidor",
                error = ex.Message
            });
        }
    }
}