using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManage.Domain.Entities;

namespace UserManage.Application.Interface.Repository
{
    public interface IParametricaRepository
    {
        Task<IEnumerable<Pais>> ObtenerPaisesAsync();
        Task<IEnumerable<Departamento>> ObtenerDepartamentosPorPaisAsync(int paisId);
        Task<IEnumerable<Municipio>> ObtenerMunicipiosPorDepartamentoAsync(int departamentoId);
    }
}
