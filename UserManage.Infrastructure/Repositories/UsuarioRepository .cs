using Dapper;
using Npgsql;
using NpgsqlTypes;
using System.Data;
using UserManage.Application.Interface.Repository;
using UserManage.Domain.Dtos;
using UserManage.Domain.Entities;
using UserManage.Infrastructure.Interface;

namespace UserManage.Infrastructure.Repositories
{
    public class UsuarioRepository(IDbConnectionFactory connectionFactory) : IUsuarioRepository
    {
        private readonly IDbConnectionFactory _connectionFactory = connectionFactory;

        public async Task<(long id, string mensaje, bool exitoso)> CrearUsuario(ReqUsuarioDto req)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();

            using var cmd = new NpgsqlCommand("CALL coink.sp_crear_usuario(@p_nombre, @p_telefono, @p_pais_id, @p_departamento_id, @p_municipio_id, @p_direccion, @p_id, @p_mensaje, @p_estado)", (NpgsqlConnection?)connection);
            cmd.Parameters.AddWithValue("p_nombre", req.Nombre);
            cmd.Parameters.AddWithValue("p_telefono", req.Telefono);
            cmd.Parameters.AddWithValue("p_pais_id", req.PaisId);
            cmd.Parameters.AddWithValue("p_departamento_id", req.DepartamentoId);
            cmd.Parameters.AddWithValue("p_municipio_id", req.MunicipioId);
            cmd.Parameters.AddWithValue("p_direccion", req.Direccion);
            var paramId = new NpgsqlParameter("p_id", NpgsqlDbType.Integer)
            {
                Direction = ParameterDirection.InputOutput,
                Value = DBNull.Value
            };
            var paramMensaje = new NpgsqlParameter("p_mensaje", NpgsqlDbType.Varchar)
            {
                Direction = ParameterDirection.InputOutput,
                Value = DBNull.Value
            };
            var paramEstado = new NpgsqlParameter("p_estado", NpgsqlDbType.Boolean)
            {
                Direction = ParameterDirection.InputOutput,
                Value = DBNull.Value
            };

            cmd.Parameters.Add(paramId);
            cmd.Parameters.Add(paramMensaje);
            cmd.Parameters.Add(paramEstado);

            await cmd.ExecuteNonQueryAsync();

            var id = paramId.Value != DBNull.Value ? Convert.ToInt64(paramId.Value) : 0;
            var mensaje = paramMensaje.Value?.ToString() ?? string.Empty;
            var exitoso = paramEstado.Value != DBNull.Value && Convert.ToBoolean(paramEstado.Value);

            return (id, mensaje, exitoso);
        }

        public async Task<Usuario?> ObtenerUsuarioById(long id)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                using (var cmd = new NpgsqlCommand("CALL coink.sp_obtener_usuario_by_id(@p_usuario_id, @resultado)", (NpgsqlConnection?)connection))
                {
                    cmd.Transaction = (NpgsqlTransaction?)transaction;

                    var cursorParam = new NpgsqlParameter("resultado", NpgsqlDbType.Refcursor)
                    {
                        Direction = ParameterDirection.InputOutput,
                        Value = "usuario_cursor"
                    };

                    cmd.Parameters.AddWithValue("p_usuario_id", id);
                    cmd.Parameters.Add(cursorParam);

                    await cmd.ExecuteNonQueryAsync();
                }
                Usuario? usuario = null;
                using (var fetchCmd = new NpgsqlCommand("FETCH ALL IN \"usuario_cursor\"", (NpgsqlConnection?)connection))
                {
                    fetchCmd.Transaction = (NpgsqlTransaction?)transaction;

                    using var reader = await fetchCmd.ExecuteReaderAsync();
                    if (await reader.ReadAsync())
                    {
                        usuario = new Usuario
                        {
                            Id = reader.GetInt32(0),
                            Nombre = reader.GetString(1),
                            Telefono = reader.GetString(2),
                            Pais = reader.GetString(3),
                            Departamento = reader.GetString(4),
                            Municipio = reader.GetString(5),
                            Direccion = reader.GetString(6),
                            FechaCreacion = reader.GetDateTime(7)
                        };
                    }
                }
                using (var closeCmd = new NpgsqlCommand("CLOSE \"usuario_cursor\"", (NpgsqlConnection?)connection))
                {
                    closeCmd.Transaction = (NpgsqlTransaction?)transaction;
                    await closeCmd.ExecuteNonQueryAsync();
                }

                transaction.Commit();

                return usuario;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        private class SpRegistrarUsuarioResult
        {
            public int Id { get; set; }
            public string Mensaje { get; set; } = string.Empty;
            public bool Exitoso { get; set; }
        }
    }
}