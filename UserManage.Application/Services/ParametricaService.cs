using UserManage.Application.Interface.Repository;
using UserManage.Application.Interface.Service;
using UserManage.Domain.Common;
using UserManage.Domain.Entities;

namespace UserManage.Application.Services
{
    public class ParametricaService(IParametricaRepository repository) : IParametricaService
    {
        private readonly IParametricaRepository _repository = repository;

        public async Task<Result<IEnumerable<Pais>>> ObtenerPais()
        {
            try
            {
                var paises = await _repository.ObtenerPais();
                return Result<IEnumerable<Pais>>.SuccessResult(paises);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Pais>>.FailureResult($"Error: {ex.Message}");
            }
        }

        public async Task<Result<IEnumerable<Departamento>>> ObtenerDepartamento(long paisId)
        {
            if (paisId <= 0)
            {
                return Result<IEnumerable<Departamento>>.FailureResult("El ID del pais debe ser mayor a 0");
            }

            try
            {
                var departamentos = await _repository.ObtenerDepartamentosByPais(paisId);
                return Result<IEnumerable<Departamento>>.SuccessResult(departamentos);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Departamento>>.FailureResult($"Error: {ex.Message}");
            }
        }

        public async Task<Result<IEnumerable<Municipio>>> ObtenerMunicipio(long departamentoId)
        {
            if (departamentoId <= 0)
            {
                return Result<IEnumerable<Municipio>>.FailureResult("El ID del departamento debe ser mayor a 0");
            }

            try
            {
                var municipios = await _repository.ObtenerMunicipioByDepartamento(departamentoId);
                return Result<IEnumerable<Municipio>>.SuccessResult(municipios);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Municipio>>.FailureResult($"Error: {ex.Message}");
            }
        }
    }
}