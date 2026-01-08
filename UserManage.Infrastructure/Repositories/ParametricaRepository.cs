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
    public class ParametricaRepository(IDbConnectionFactory connectionFactory) : IParametricaRepository
    {
        private readonly IDbConnectionFactory _connectionFactory = connectionFactory;

        public async Task<IEnumerable<Pais>> ObtenerPaisesAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<Pais>("SELECT * FROM sp_obtener_paises()");
        }

        public async Task<IEnumerable<Departamento>> ObtenerDepartamentosPorPaisAsync(int paisId)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<Departamento>(
                "SELECT * FROM sp_obtener_departamentos_por_pais(@p_pais_id)",
                new { p_pais_id = paisId }
            );
        }

        public async Task<IEnumerable<Municipio>> ObtenerMunicipiosPorDepartamentoAsync(int departamentoId)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<Municipio>(
                "SELECT * FROM sp_obtener_municipios_por_departamento(@p_departamento_id)",
                new { p_departamento_id = departamentoId }
            );
        }
    }
}