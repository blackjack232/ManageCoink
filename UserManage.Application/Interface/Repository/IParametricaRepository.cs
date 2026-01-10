using UserManage.Domain.Entities;

namespace UserManage.Application.Interface.Repository
{
    public interface IParametricaRepository
    {
        Task<IEnumerable<Pais>> ObtenerPais();
        Task<IEnumerable<Departamento>> ObtenerDepartamentosByPais(long paisId);
        Task<IEnumerable<Municipio>> ObtenerMunicipioByDepartamento(long departamentoId);
    }
}
