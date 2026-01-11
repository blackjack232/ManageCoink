using UserManage.Application.Interface.Repository;
using UserManage.Application.Interface.Service;
using UserManage.Domain.Common;
using UserManage.Domain.Constants;
using UserManage.Domain.Entities;

namespace UserManage.Application.Services
{
    /// <summary>
    /// Servicio de aplicación para la gestión de información geográfica regional.
    /// Proporciona operaciones de consulta para países, departamentos y municipios.
    /// </summary>
    public class RegionService(IRegionRepository repository) : IRegionService
    {
        private readonly IRegionRepository _repository = repository;

        /// <summary>
        /// Obtiene la lista completa de países disponibles en el sistema.
        /// </summary>
        /// <returns>
        /// Un objeto <see cref="Result{T}"/> que contiene:
        /// <list type="bullet">
        /// <item><description><b>Éxito:</b> Colección de objetos <see cref="Pais"/> con todos los países activos, 
        /// incluyendo ID, nombre, código, estado y fecha de creación.</description></item>
        /// <item><description><b>Fallo:</b> Mensaje de error descriptivo si ocurre algún problema 
        /// al consultar la base de datos.</description></item>
        /// </list>
        /// </returns>
        public async Task<Result<IEnumerable<Pais>>> ObtenerPais()
        {
            try
            {
                var paises = await _repository.ObtenerPais();
                return Result<IEnumerable<Pais>>.SuccessResult(paises);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Pais>>.FailureResult(string.Format(AppMessages.ErrorPrefijo, ex.Message));
            }
        }

        /// <summary>
        /// Obtiene la lista de departamentos asociados a un país específico.
        /// </summary>
        /// <param name="paisId">
        /// Identificador único del país del cual se desean obtener los departamentos.
        /// Debe ser un valor mayor a cero.
        /// </param>
        /// <returns>
        /// Un objeto <see cref="Result{T}"/> que contiene:
        /// <list type="bullet">
        /// <item><description><b>Éxito:</b> Colección de objetos <see cref="Departamento"/> con todos 
        /// los departamentos activos del país especificado, ordenados alfabéticamente por nombre.</description></item>
        /// <item><description><b>Fallo:</b> Mensaje de error descriptivo si el ID es inválido, 
        /// el país no existe, está inactivo, o si ocurre un error de base de datos.</description></item>
        /// </list>
        /// </returns>
        public async Task<Result<IEnumerable<Departamento>>> ObtenerDepartamento(long paisId)
        {
            if (paisId <= 0)
            {
                return Result<IEnumerable<Departamento>>.FailureResult(AppMessages.PaisIdMayorCero);
            }

            try
            {
                var departamentos = await _repository.ObtenerDepartamentosByPais(paisId);
                return Result<IEnumerable<Departamento>>.SuccessResult(departamentos);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Departamento>>.FailureResult(string.Format(AppMessages.ErrorPrefijo, ex.Message));
            }
        }

        /// <summary>
        /// Obtiene la lista de municipios asociados a un departamento específico.
        /// </summary>
        /// <param name="departamentoId">
        /// Identificador único del departamento del cual se desean obtener los municipios.
        /// Debe ser un valor mayor a cero.
        /// </param>
        /// <returns>
        /// Un objeto <see cref="Result{T}"/> que contiene:
        /// <list type="bullet">
        /// <item><description><b>Éxito:</b> Colección de objetos <see cref="Municipio"/> con todos 
        /// los municipios activos del departamento especificado, ordenados alfabéticamente por nombre.</description></item>
        /// <item><description><b>Fallo:</b> Mensaje de error descriptivo si el ID es inválido, 
        /// el departamento no existe, está inactivo, o si ocurre un error de base de datos.</description></item>
        /// </list>
        /// </returns>
        public async Task<Result<IEnumerable<Municipio>>> ObtenerMunicipio(long departamentoId)
        {
            if (departamentoId <= 0)
            {
                return Result<IEnumerable<Municipio>>.FailureResult(AppMessages.DepartamentoIdMayorCero);
            }

            try
            {
                var municipios = await _repository.ObtenerMunicipioByDepartamento(departamentoId);
                return Result<IEnumerable<Municipio>>.SuccessResult(municipios);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Municipio>>.FailureResult(string.Format(AppMessages.ErrorPrefijo, ex.Message));
            }
        }
    }
}