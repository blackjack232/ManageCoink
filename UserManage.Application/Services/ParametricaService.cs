using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManage.Application.Interface.Repository;
using UserManage.Application.Interface.Service;
using UserManage.Domain.Common;
using UserManage.Domain.Entities;

namespace UserManage.Application.Services
{
    public class ParametricaService(IParametricaRepository parametricaRepository) : IParametricaService
    {
        private readonly IParametricaRepository _parametricaRepository = parametricaRepository;

        public async Task<Result<IEnumerable<Pais>>> ObtenerPaisesAsync()
        {
            try
            {
                var paises = await _parametricaRepository.ObtenerPaisesAsync();
                return Result<IEnumerable<Pais>>.SuccessResult(paises);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Pais>>.FailureResult($"Error: {ex.Message}");
            }
        }

        public async Task<Result<IEnumerable<Departamento>>> ObtenerDepartamentosAsync(int paisId)
        {
            if (paisId <= 0)
            {
                return Result<IEnumerable<Departamento>>.FailureResult("El ID del país debe ser mayor a 0");
            }

            try
            {
                var departamentos = await _parametricaRepository.ObtenerDepartamentosPorPaisAsync(paisId);
                return Result<IEnumerable<Departamento>>.SuccessResult(departamentos);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Departamento>>.FailureResult($"Error: {ex.Message}");
            }
        }

        public async Task<Result<IEnumerable<Municipio>>> ObtenerMunicipiosAsync(int departamentoId)
        {
            if (departamentoId <= 0)
            {
                return Result<IEnumerable<Municipio>>.FailureResult("El ID del departamento debe ser mayor a 0");
            }

            try
            {
                var municipios = await _parametricaRepository.ObtenerMunicipiosPorDepartamentoAsync(departamentoId);
                return Result<IEnumerable<Municipio>>.SuccessResult(municipios);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Municipio>>.FailureResult($"Error: {ex.Message}");
            }
        }
    }
}