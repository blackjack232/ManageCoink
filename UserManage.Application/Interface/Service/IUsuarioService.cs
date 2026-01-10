using UserManage.Domain.Common;
using UserManage.Domain.Dtos;

namespace UserManage.Application.Interface.Service
{
    public interface IUsuarioService
    {
        Task<Result<UsuarioResponseDto>> CrearUsuario(ReqUsuarioDto dto);
        Task<Result<UsuarioResponseDto>> ObtenerUsuarioById(long id);
    }
}
