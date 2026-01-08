using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManage.Domain.Entities;

namespace UserManage.Application.Interface.Repository
{

    public interface IUsuarioRepository
    {
        Task<(int Id, string Message, bool Success)> RegistrarUsuarioAsync(
            string nombre,
            string telefono,
            long paisId,
            long departamentoId,
            long municipioId,
            string direccion
        );

        Task<Usuario?> ObtenerUsuarioPorIdAsync(int id);
    }
}
