
using UserManage.Application.Interface.Repository;
using UserManage.Application.Interface.Service;
using UserManage.Application.Vlidators;
using UserManage.Domain.Common;
using UserManage.Domain.Constants;
using UserManage.Domain.Dtos;
using UserManage.Domain.Entities;
namespace UserManage.Application.Services;

/// <summary>
/// Servicio para la gestión de usuarios.
/// Proporciona operaciones de creación, consulta y mantenimiento de usuarios del sistema.
/// </summary>
/// <remarks>
/// Este servicio implementa la lógica de negocio relacionada con usuarios,
/// incluyendo validaciones, transformaciones de datos y coordinación con el repositorio.
/// </remarks>
public class UsuarioService(IUsuarioRepository repository) : IUsuarioService
{
    private readonly IUsuarioRepository _repository = repository;

    /// <summary>
    /// Crea un nuevo usuario en el sistema.
    /// </summary>
    /// <param name="req">DTO con los datos del usuario a crear</param>
    /// <returns>
    /// Resultado de la operación conteniendo el UsuarioResponseDto si fue exitoso,
    /// o los errores de validación/procesamiento si falló
    /// </returns>
    public async Task<Result<UsuarioResponseDto>> CrearUsuario(ReqUsuarioDto req)
    {
        var validationErrors = RegistrarUsuarioValidator.Validate(req);
        if (validationErrors.Count != 0)
        {
            return Result<UsuarioResponseDto>.FailureResult(
                AppMessages.ErrorValidacion,
                validationErrors
            );
        }

        try
        {
            var (id, mensaje, exitoso) = await _repository.CrearUsuario(req);

            if (!exitoso)
            {
                return Result<UsuarioResponseDto>.FailureResult(mensaje);
            }

            var usuario = await _repository.ObtenerUsuarioById(id);

            if (usuario == null)
            {
                return Result<UsuarioResponseDto>.FailureResult(AppMessages.UsuarioRegistradoNoRecuperado);
            }

            var response = MapToResponseDto(usuario);
            return Result<UsuarioResponseDto>.SuccessResult(response, mensaje);
        }
        catch (Exception ex)
        {
            return Result<UsuarioResponseDto>.FailureResult(
                string.Format(AppMessages.ErrorInternoPrefijo, ex.Message)
            );
        }
    }

    /// <summary>
    /// Obtiene un usuario por su identificador único.
    /// </summary>
    /// <param name="id">Identificador único del usuario</param>
    /// <returns>
    /// Resultado de la operación conteniendo el UsuarioResponseDto si se encontró el usuario,
    /// o un mensaje de error si no existe o el ID es inválido
    /// </returns>
    public async Task<Result<UsuarioResponseDto>> ObtenerUsuarioById(long id)
    {
        if (id <= 0)
        {
            return Result<UsuarioResponseDto>.FailureResult(AppMessages.IdMayorCero);
        }

        try
        {
            var usuario = await _repository.ObtenerUsuarioById(id);

            if (usuario == null)
            {
                return Result<UsuarioResponseDto>.FailureResult(AppMessages.UsuarioNoEncontrado);
            }

            var response = MapToResponseDto(usuario);
            return Result<UsuarioResponseDto>.SuccessResult(response);
        }
        catch (Exception ex)
        {
            return Result<UsuarioResponseDto>.FailureResult(
                string.Format(AppMessages.ErrorInternoPrefijo, ex.Message)
            );
        }
    }

    /// <summary>
    /// Mapea una entidad Usuario a su DTO de respuesta.
    /// </summary>
    /// <param name="usuario">Entidad Usuario a mapear</param>
    /// <returns>DTO de respuesta con los datos del usuario</returns>
    private static UsuarioResponseDto MapToResponseDto(Usuario usuario)
    {
        return new UsuarioResponseDto(
            usuario.Id,
            usuario.Nombre,
            usuario.Telefono,
            usuario.Pais ?? string.Empty,
            usuario.Departamento ?? string.Empty,
            usuario.Municipio ?? string.Empty,
            usuario.Direccion,
            usuario.FechaCreacion
        );
    }
}