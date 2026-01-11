using Microsoft.AspNetCore.Mvc;
using UserManage.Application.Interface.Service;
using UserManage.Domain.Constants;
using UserManage.Domain.Dtos;

namespace UserManage.API.Controllers;

/// <summary>
/// Controlador REST para la gestión de datos de regiones geográficas.
/// </summary>
/// <remarks>
/// Proporciona endpoints para consultar países, departamentos y municipios.
/// Implementa estructura jerárquica: País → Departamento → Municipio.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
public class RegionController(IRegionService service, ILogger<RegionController> logger) : ControllerBase
{
    private readonly IRegionService _service = service;
    private readonly ILogger<RegionController> _logger = logger;

    /// <summary>
    /// Obtiene la lista completa de países disponibles.
    /// </summary>
    /// <returns>Lista de países activos del sistema</returns>
    /// <response code="200">Lista obtenida exitosamente</response>
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
                _logger.LogWarning(LogMessages.ErrorObtenerPaises, result.Message);

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
            _logger.LogError(ex, LogMessages.ErrorInternoObtenerPaises, ex.GetType().Name);

            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                success = false,
                message = AppMessages.ErrorInterno,
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// Obtiene los departamentos de un país específico.
    /// </summary>
    /// <param name="paisId">Identificador del país</param>
    /// <returns>Lista de departamentos del país</returns>
    /// <response code="200">Lista obtenida exitosamente</response>
    /// <response code="400">ID inválido o país no encontrado</response>
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
                _logger.LogWarning(LogMessages.PaisIdInvalidoLog, paisId);

                return BadRequest(new
                {
                    success = false,
                    message = AppMessages.IdPaisInvalido
                });
            }

            var result = await _service.ObtenerDepartamento(paisId);

            if (!result.Success)
            {
                _logger.LogWarning(LogMessages.ErrorObtenerDepartamentos, paisId, result.Message);

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
            _logger.LogError(ex, LogMessages.ErrorInternoDepartamentos, paisId, ex.GetType().Name);

            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                success = false,
                message = AppMessages.ErrorInterno,
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// Obtiene los municipios de un departamento específico.
    /// </summary>
    /// <param name="departamentoId">Identificador del departamento</param>
    /// <returns>Lista de municipios del departamento</returns>
    /// <response code="200">Lista obtenida exitosamente</response>
    /// <response code="400">ID inválido o departamento no encontrado</response>
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
                _logger.LogWarning(LogMessages.DepartamentoIdInvalidoLog, departamentoId);

                return BadRequest(new
                {
                    success = false,
                    message = AppMessages.IdDepartamentoInvalido
                });
            }

            var result = await _service.ObtenerMunicipio(departamentoId);

            if (!result.Success)
            {
                _logger.LogWarning(LogMessages.ErrorObtenerMunicipios, departamentoId, result.Message);

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
            _logger.LogError(ex, LogMessages.ErrorInternoMunicipios, departamentoId, ex.GetType().Name);

            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                success = false,
                message = AppMessages.ErrorInterno,
                error = ex.Message
            });
        }
    }
}