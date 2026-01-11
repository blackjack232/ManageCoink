using Dapper;
using Npgsql;
using NpgsqlTypes;
using System.Data;
using UserManage.Application.Interface.Repository;
using UserManage.Domain.Constants;
using UserManage.Domain.Entities;
using UserManage.Infrastructure.Interface;

namespace UserManage.Infrastructure.Repositories;

/// <summary>
/// Repositorio para la gestión de datos paramétricos en la base de datos.
/// Proporciona acceso a información de países, departamentos y municipios mediante stored procedures.
/// </summary>
/// <remarks>
/// Este repositorio utiliza stored procedures de PostgreSQL con cursores (REFCURSOR)
/// para retornar información geográfica parametrizada del sistema.
/// Todas las operaciones son de solo lectura (consultas).
/// </remarks>
public class RegionRepository(IDbConnectionFactory connectionFactory) : IRegionRepository
{
    private readonly IDbConnectionFactory _connectionFactory = connectionFactory;

    /// <summary>
    /// Obtiene la lista completa de países disponibles en el sistema.
    /// </summary>
    /// <returns>
    /// Colección enumerable de objetos <see cref="Pais"/> con todos los países activos registrados.
    /// Retorna una colección vacía si no hay países registrados.
    /// </returns>
    /// <exception cref="Exception">
    /// Lanza excepción si ocurre un error al conectar con la base de datos
    /// o al ejecutar el stored procedure.
    /// </exception>
    /// <remarks>
    /// Este método ejecuta el stored procedure 'sp_obtener_pais'
    /// que retorna todos los registros activos de la tabla de países,
    /// ordenados alfabéticamente por nombre.
    /// </remarks>
    public async Task<IEnumerable<Pais>> ObtenerPais()
    {
        try
        {
            return await ExecuteStoredProcedureWithCursor<Pais>(
                procedureName: "coink.sp_obtener_pais",
                cursorName: "paises_cursor",
                parameters: null,
                mapFunction: reader => new Pais
                {
                    Id = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    Codigo = reader.GetString(2),
                    Estado = reader.GetBoolean(3),
                    Fecha_Creacion = reader.GetDateTime(4)
                }
            );
        }
        catch (Exception ex)
        {
            throw new Exception(string.Format(RepositoryMessages.ErrorObtenerPaises, ex.Message), ex);
        }
    }

    /// <summary>
    /// Obtiene la lista de departamentos asociados a un país específico.
    /// </summary>
    /// <param name="paisId">Identificador único del país</param>
    /// <returns>
    /// Colección enumerable de objetos <see cref="Departamento"/> con todos los departamentos activos
    /// del país especificado. Retorna una colección vacía si el país no tiene departamentos.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Lanza excepción si el paisId es menor o igual a cero.
    /// </exception>
    /// <exception cref="Exception">
    /// Lanza excepción si ocurre un error al conectar con la base de datos
    /// o al ejecutar el stored procedure.
    /// </exception>
    /// <remarks>
    /// Este método ejecuta el stored procedure 'sp_obtener_departamentos_by_pais'
    /// que retorna todos los departamentos activos asociados al país especificado,
    /// ordenados alfabéticamente por nombre.
    /// </remarks>
    public async Task<IEnumerable<Departamento>> ObtenerDepartamentosByPais(long paisId)
    {
        try
        {
            var parameters = new Dictionary<string, object>
            {
                { "p_pais_id", paisId }
            };

            return await ExecuteStoredProcedureWithCursor<Departamento>(
                procedureName: "coink.sp_obtener_departamentos_by_pais",
                cursorName: "departamentos_cursor",
                parameters: parameters,
                mapFunction: reader => new Departamento
                {
                    Id = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    Pais_id = reader.GetInt32(2)
                }
            );
        }
        catch (NpgsqlException ex) when (ex.Message.Contains("no existe"))
        {
            throw new Exception(string.Format(RepositoryMessages.PaisNoExiste, paisId), ex);
        }
        catch (NpgsqlException ex) when (ex.Message.Contains("inactivo"))
        {
            throw new Exception(string.Format(RepositoryMessages.PaisInactivo, paisId), ex);
        }
        catch (Exception ex) when (ex is not ArgumentException)
        {
            throw new Exception(string.Format(RepositoryMessages.ErrorObtenerDepartamentos, paisId, ex.Message), ex);
        }
    }

    /// <summary>
    /// Obtiene la lista de municipios asociados a un departamento específico.
    /// </summary>
    /// <param name="departamentoId">Identificador único del departamento</param>
    /// <returns>
    /// Colección enumerable de objetos <see cref="Municipio"/> con todos los municipios activos
    /// del departamento especificado. Retorna una colección vacía si el departamento no tiene municipios.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Lanza excepción si el departamentoId es menor o igual a cero.
    /// </exception>
    /// <exception cref="Exception">
    /// Lanza excepción si ocurre un error al conectar con la base de datos
    /// o al ejecutar el stored procedure.
    /// </exception>
    /// <remarks>
    /// Este método ejecuta el stored procedure 'sp_obtener_municipios_by_departamento'
    /// que retorna todos los municipios activos asociados al departamento especificado,
    /// ordenados alfabéticamente por nombre.
    /// </remarks>
    public async Task<IEnumerable<Municipio>> ObtenerMunicipioByDepartamento(long departamentoId)
    {
        try
        {
            var parameters = new Dictionary<string, object>
            {
                { "p_departamento_id", departamentoId }
            };

            return await ExecuteStoredProcedureWithCursor<Municipio>(
                procedureName: "coink.sp_obtener_municipios_by_departamento",
                cursorName: "municipios_cursor",
                parameters: parameters,
                mapFunction: reader => new Municipio
                {
                    Id = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    Departamento_Id = reader.GetInt32(2)
                }
            );
        }
        catch (NpgsqlException ex) when (ex.Message.Contains("no existe"))
        {
            throw new Exception(string.Format(RepositoryMessages.DepartamentoNoExiste, departamentoId), ex);
        }
        catch (NpgsqlException ex) when (ex.Message.Contains("inactivo"))
        {
            throw new Exception(string.Format(RepositoryMessages.DepartamentoInactivo, departamentoId), ex);
        }
        catch (Exception ex) when (ex is not ArgumentException)
        {
            throw new Exception(string.Format(RepositoryMessages.ErrorObtenerMunicipios, departamentoId, ex.Message), ex);
        }
    }

    #region Private Helper Methods

    /// <summary>
    /// Método genérico para ejecutar stored procedures de PostgreSQL que retornan cursores.
    /// </summary>
    /// <typeparam name="T">Tipo de entidad a retornar</typeparam>
    /// <param name="procedureName">Nombre completo del stored procedure (ej: coink.sp_nombre)</param>
    /// <param name="cursorName">Nombre del cursor a usar</param>
    /// <param name="parameters">Diccionario con los parámetros de entrada (puede ser null)</param>
    /// <param name="mapFunction">Función para mapear el DataReader a la entidad T</param>
    /// <returns>Colección de entidades del tipo T</returns>
    private async Task<IEnumerable<T>> ExecuteStoredProcedureWithCursor<T>(
        string procedureName,
        string cursorName,
        Dictionary<string, object>? parameters,
        Func<NpgsqlDataReader, T> mapFunction)
    {
        var results = new List<T>();

        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        using var transaction = connection.BeginTransaction();

        try
        {
            await CallStoredProcedure(
                (NpgsqlConnection)connection,
                (NpgsqlTransaction)transaction,
                procedureName,
                cursorName,
                parameters
            );

            results = await FetchDataFromCursor(
                (NpgsqlConnection)connection,
                (NpgsqlTransaction)transaction,
                cursorName,
                mapFunction
            );

            await CloseCursor(
                (NpgsqlConnection)connection,
                (NpgsqlTransaction)transaction,
                cursorName
            );

            transaction.Commit();

            return results;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    /// <summary>
    /// Ejecuta el stored procedure y configura el cursor.
    /// </summary>
    private static async Task CallStoredProcedure(
        NpgsqlConnection connection,
        NpgsqlTransaction transaction,
        string procedureName,
        string cursorName,
        Dictionary<string, object>? parameters)
    {
        var parameterNames = parameters?.Keys.Select(k => $"@{k}").ToList() ?? [];
        parameterNames.Add("@resultado");
        var callStatement = $"CALL {procedureName}({string.Join(", ", parameterNames)})";

        using var cmd = new NpgsqlCommand(callStatement, connection)
        {
            Transaction = transaction
        };

        if (parameters != null)
        {
            foreach (var param in parameters)
            {
                cmd.Parameters.AddWithValue(param.Key, param.Value);
            }
        }

        var cursorParam = new NpgsqlParameter("resultado", NpgsqlDbType.Refcursor)
        {
            Direction = ParameterDirection.InputOutput,
            Value = cursorName
        };
        cmd.Parameters.Add(cursorParam);

        await cmd.ExecuteNonQueryAsync();
    }

    /// <summary>
    /// Lee los datos del cursor y los mapea a entidades.
    /// </summary>
    private static async Task<List<T>> FetchDataFromCursor<T>(
        NpgsqlConnection connection,
        NpgsqlTransaction transaction,
        string cursorName,
        Func<NpgsqlDataReader, T> mapFunction)
    {
        var results = new List<T>();

        using var fetchCmd = new NpgsqlCommand($"FETCH ALL IN \"{cursorName}\"", connection)
        {
            Transaction = transaction
        };

        using var reader = await fetchCmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            results.Add(mapFunction(reader));
        }

        return results;
    }

    /// <summary>
    /// Cierra el cursor después de leer los datos.
    /// </summary>
    private static async Task CloseCursor(
        NpgsqlConnection connection,
        NpgsqlTransaction transaction,
        string cursorName)
    {
        using var closeCmd = new NpgsqlCommand($"CLOSE \"{cursorName}\"", connection)
        {
            Transaction = transaction
        };
        await closeCmd.ExecuteNonQueryAsync();
    }

    #endregion
}