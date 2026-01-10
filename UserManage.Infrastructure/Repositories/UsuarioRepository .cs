using Dapper;
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

            var result = await connection.QueryFirstAsync<SpRegistrarUsuarioResult>(
                "SELECT * FROM sp_crear_usuario(@p_nombre, @p_telefono, @p_pais_id, @p_departamento_id, @p_municipio_id, @p_direccion)",
                new
                {
                    p_nombre = req.Nombre,
                    p_telefono = req.Telefono,
                    p_pais_id = req.PaisId,
                    p_departamento_id = req.DepartamentoId,
                    p_municipio_id = req.MunicipioId,
                    p_direccion = req.Direccion
                }
            );

            return (result.Id, result.Mensaje, result.Exitoso);
        }

        public async Task<Usuario?> ObtenerUsuarioById(long id)
        {
            using var connection = _connectionFactory.CreateConnection();

            var usuario = await connection.QueryFirstOrDefaultAsync<Usuario>(
                "SELECT * FROM sp_obtener_usuario_by_id(@p_usuario_id)",
                new { p_usuario_id = id }
            );

            return usuario;
        }

        private class SpRegistrarUsuarioResult
        {
            public int Id { get; set; }
            public string Mensaje { get; set; } = string.Empty;
            public bool Exitoso { get; set; }
        }
    }
}