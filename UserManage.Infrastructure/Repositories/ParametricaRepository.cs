using Dapper;
using System.Data;
using UserManage.Application.Interface.Repository;
using UserManage.Domain.Entities;
using UserManage.Infrastructure.Interface;

namespace UserManage.Infrastructure.Repositories;

/// <summary>
/// Repositorio para la gestión de datos paramétricos en la base de datos.
/// Proporciona acceso a información de países, departamentos y municipios mediante stored procedures.
/// </summary>
/// <remarks>
/// Este repositorio utiliza Dapper como micro-ORM para ejecutar stored procedures
/// que retornan información geográfica parametrizada del sistema.
/// Todas las operaciones son de solo lectura (consultas).
/// 
/// IMPORTANTE: Para PostgreSQL, los stored procedures se ejecutan con CALL o usando
/// funciones que retornan tablas con SELECT FROM. Este repositorio está configurado
/// para usar stored procedures tradicionales.
/// </remarks>
public class ParametricaRepository(IDbConnectionFactory connectionFactory) : IParametricaRepository
{
    private readonly IDbConnectionFactory _connectionFactory = connectionFactory;

    /// <summary>
    /// Obtiene la lista completa de países disponibles en el sistema.
    /// </summary>
    /// <returns>
    /// Colección enumerable de objetos <see cref="Pais"/> con todos los países registrados.
    /// Retorna una colección vacía si no hay países registrados.
    /// </returns>
    /// <exception cref="Exception">
    /// Lanza excepción si ocurre un error al conectar con la base de datos
    /// o al ejecutar el stored procedure.
    /// </exception>
    /// <remarks>
    /// Este método ejecuta el stored procedure 'sp_obtener_pais'
    /// que retorna todos los registros activos de la tabla de países.
    /// No requiere parámetros de entrada.
    /// </remarks>
    public async Task<IEnumerable<Pais>> ObtenerPais()
    {
        using var connection = _connectionFactory.CreateConnection();

        return await connection.QueryAsync<Pais>(
            "sp_obtener_pais",
            commandType: CommandType.StoredProcedure
        );
    }

    /// <summary>
    /// Obtiene la lista de departamentos (estados/provincias) que pertenecen a un país específico.
    /// </summary>
    /// <param name="paisId">Identificador único del país del cual se desean obtener los departamentos</param>
    /// <returns>
    /// Colección enumerable de objetos <see cref="Departamento"/> pertenecientes al país especificado.
    /// Retorna una colección vacía si el país no tiene departamentos o no existe.
    /// </returns>
    /// <exception cref="Exception">
    /// Lanza excepción si ocurre un error al conectar con la base de datos,
    /// al ejecutar el stored procedure, o si el paisId no es válido.
    /// </exception>
    /// <remarks>
    /// Este método ejecuta el stored procedure 'sp_obtener_departamentos_by_pais'
    /// que retorna todos los departamentos activos asociados al país proporcionado.
    /// La relación entre país y departamento se valida en el stored procedure.
    /// </remarks>
    public async Task<IEnumerable<Departamento>> ObtenerDepartamentosByPais(long paisId)
    {
        using var connection = _connectionFactory.CreateConnection();

        return await connection.QueryAsync<Departamento>(
            "sp_obtener_departamentos_by_pais",
            new { p_pais_id = paisId },
            commandType: CommandType.StoredProcedure
        );
    }

    /// <summary>
    /// Obtiene la lista de municipios (ciudades/localidades) que pertenecen a un departamento específico.
    /// </summary>
    /// <param name="departamentoId">Identificador único del departamento del cual se desean obtener los municipios</param>
    /// <returns>
    /// Colección enumerable de objetos <see cref="Municipio"/> pertenecientes al departamento especificado.
    /// Retorna una colección vacía si el departamento no tiene municipios o no existe.
    /// </returns>
    /// <exception cref="Exception">
    /// Lanza excepción si ocurre un error al conectar con la base de datos,
    /// al ejecutar el stored procedure, o si el departamentoId no es válido.
    /// </exception>
    /// <remarks>
    /// Este método ejecuta el stored procedure 'sp_obtener_municipios_by_departamento'
    /// que retorna todos los municipios activos asociados al departamento proporcionado.
    /// La relación entre departamento y municipio se valida en el stored procedure.
    /// Este es el nivel más granular de la estructura geográfica: País → Departamento → Municipio.
    /// </remarks>
    public async Task<IEnumerable<Municipio>> ObtenerMunicipioByDepartamento(long departamentoId)
    {
        using var connection = _connectionFactory.CreateConnection();

        return await connection.QueryAsync<Municipio>(
            "sp_obtener_municipios_by_departamento",
            new { p_departamento_id = departamentoId },
            commandType: CommandType.StoredProcedure
        );
    }
}