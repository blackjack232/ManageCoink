using UserManage.Domain.Dtos;
using UserManage.Domain.Entities;

namespace UserManage.Application.Interface.Repository
{

    public interface IUsuarioRepository
    {
        Task<(long id, string mensaje, bool exitoso)> CrearUsuario(ReqUsuarioDto req);

        Task<Usuario?> ObtenerUsuarioById(long id);
    }
}
