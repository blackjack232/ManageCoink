using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManage.Domain.Common;
using UserManage.Domain.Entities;

namespace UserManage.Application.Interface.Service
{
    public interface IParametricaService
    {
        Task<Result<IEnumerable<Pais>>> ObtenerPaisesAsync();
        Task<Result<IEnumerable<Departamento>>> ObtenerDepartamentosAsync(int paisId);
        Task<Result<IEnumerable<Municipio>>> ObtenerMunicipiosAsync(int departamentoId);
    }
}