using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManage.Domain.Constants
{
    /// <summary>
    /// Constantes de mensajes de error para repositorios.
    /// </summary>
    public static class RepositoryMessages
    {
        public const string ErrorObtenerPaises = "Error al obtener la lista de países: {0}";

        public const string PaisNoExiste = "El pais con ID {0} no existe";
        public const string PaisInactivo = "El pais con ID {0} esta inactivo";
        public const string ErrorObtenerDepartamentos = "Error al obtener departamentos del pais {0}: {1}";

        public const string DepartamentoNoExiste = "El departamento con ID {0} no existe";
        public const string DepartamentoInactivo = "El departamento con ID {0} esta inactivo";
        public const string ErrorObtenerMunicipios = "Error al obtener municipios del departamento {0}: {1}";
    }
}
