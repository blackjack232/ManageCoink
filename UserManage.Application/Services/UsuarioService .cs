using UserManage.Application.Interface.Repository;
using UserManage.Application.Interface.Service;
using UserManage.Application.Vlidators;
using UserManage.Domain.Common;
using UserManage.Domain.Dtos;


namespace UserManage.Application.Services;

public class UsuarioService(IUsuarioRepository usuarioRepository) : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository = usuarioRepository;

    public async Task<Result<UsuarioResponseDto>> RegistrarUsuarioAsync(ReqUsuarioDto dto)
    {
        // Validación de entrada (SOLID: Single Responsibility)
        var validationErrors = RegistrarUsuarioValidator.Validate(dto);
        if (validationErrors.Count != 0)
        {
            return Result<UsuarioResponseDto>.FailureResult(
                "Error de validación",
                validationErrors
            );
        }

        try
        {
            // Llamar al stored procedure
            var (id, message, success) = await _usuarioRepository.RegistrarUsuarioAsync(
                dto.Nombre,
                dto.Telefono,
                dto.PaisId,
                dto.DepartamentoId,
                dto.MunicipioId,
                dto.Direccion
            );

            if (!success)
            {
                return Result<UsuarioResponseDto>.FailureResult(message);
            }

            // Obtener el usuario completo
            var usuario = await _usuarioRepository.ObtenerUsuarioPorIdAsync(id);
            if (usuario == null)
            {
                return Result<UsuarioResponseDto>.FailureResult("Usuario registrado pero no se pudo recuperar");
            }

            var response = new UsuarioResponseDto(
                usuario.Id,
                usuario.Nombre,
                usuario.Telefono,
                usuario.Pais ?? "",
                usuario.Departamento ?? "",
                usuario.Municipio ?? "",
                usuario.Direccion,
                usuario.FechaCreacion
            );

            return Result<UsuarioResponseDto>.SuccessResult(response, message);
        }
        catch (Exception ex)
        {
            return Result<UsuarioResponseDto>.FailureResult($"Error interno: {ex.Message}");
        }
    }

    public async Task<Result<UsuarioResponseDto>> ObtenerUsuarioPorIdAsync(int id)
    {
        if (id <= 0)
        {
            return Result<UsuarioResponseDto>.FailureResult("El ID debe ser mayor a 0");
        }

        try
        {
            var usuario = await _usuarioRepository.ObtenerUsuarioPorIdAsync(id);

            if (usuario == null)
            {
                return Result<UsuarioResponseDto>.FailureResult("Usuario no encontrado");
            }

            var response = new UsuarioResponseDto(
                usuario.Id,
                usuario.Nombre,
                usuario.Telefono,
                usuario.Pais ?? "",
                usuario.Departamento ?? "",
                usuario.Municipio ?? "",
                usuario.Direccion,
                usuario.FechaCreacion
            );

            return Result<UsuarioResponseDto>.SuccessResult(response);
        }
        catch (Exception ex)
        {
            return Result<UsuarioResponseDto>.FailureResult($"Error interno: {ex.Message}");
        }
    }

}