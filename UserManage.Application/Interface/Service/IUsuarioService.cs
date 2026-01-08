using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManage.Domain.Common;
using UserManage.Domain.Dtos;

namespace UserManage.Application.Interface.Service
{
    public interface IUsuarioService
    {
        Task<Result<UsuarioResponseDto>> RegistrarUsuarioAsync(ReqUsuarioDto dto);
        Task<Result<UsuarioResponseDto>> ObtenerUsuarioPorIdAsync(int id);
    }
}
