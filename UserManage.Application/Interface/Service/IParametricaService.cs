using UserManage.Domain.Common;
using UserManage.Domain.Entities;

namespace UserManage.Application.Interface.Service
{
    public interface IParametricaService
    {
        Task<Result<IEnumerable<Pais>>> ObtenerPais();
        Task<Result<IEnumerable<Departamento>>> ObtenerDepartamento(long paisId);
        Task<Result<IEnumerable<Municipio>>> ObtenerMunicipio(long departamentoId);
    }
}