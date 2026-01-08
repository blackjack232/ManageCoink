using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManage.Application.Interface.Repository;
using UserManage.Domain.Entities;
using UserManage.Infrastructure.Interface;

namespace UserManage.Infrastructure.Repositories
{
    public class UsuarioRepository(IDbConnectionFactory connectionFactory) : IUsuarioRepository
    {
        private readonly IDbConnectionFactory _connectionFactory = connectionFactory;

        public async Task<(int Id, string Message, bool Success)> RegistrarUsuarioAsync(string nombre, string telefono, long paisId, long departamentoId, long municipioId, string direccion)
        {
            using var connection = _connectionFactory.CreateConnection();

            var result = await connection.QueryFirstAsync<SpRegistrarUsuarioResult>(
                "SELECT * FROM sp_registrar_usuario(@p_nombre, @p_telefono, @p_pais_id, @p_departamento_id, @p_municipio_id, @p_direccion)",
                new
                {
                    p_nombre = nombre,
                    p_telefono = telefono,
                    p_pais_id = paisId,
                    p_departamento_id = departamentoId,
                    p_municipio_id = municipioId,
                    p_direccion = direccion
                }
            );

            return (result.Id, result.Mensaje, result.Exitoso);
        }

        public async Task<Usuario?> ObtenerUsuarioPorIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();

            var usuario = await connection.QueryFirstOrDefaultAsync<Usuario>(
                "SELECT * FROM sp_obtener_usuario_por_id(@p_usuario_id)",
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