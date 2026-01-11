using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManage.Domain.Constants
{
    /// <summary>
    /// Constantes de mensajes utilizados en la aplicación.
    /// </summary>
    public static class AppMessages
    {
        public const string DatosInvalidos = "Datos de entrada invalidos";
        public const string CuerpoVacio = "El cuerpo de la solicitud no puede estar vacio";
        public const string IdInvalido = "El ID debe ser un numero mayor a 0";
        public const string IdMayorCero = "El ID debe ser mayor a 0";
        public const string ArgumentosInvalidos = "Error en los argumentos proporcionados";
        public const string ErrorInterno = "Ha ocurrido un error interno en el servidor";
        public const string ErrorValidacion = "Error de validacion";
        public const string ErrorInternoPrefijo = "Error interno: {0}";
        public const string OperacionExitosa = "Operación realizada exitosamente";

        public const string IdPaisInvalido = "El ID del país debe ser un numero mayor a 0";
        public const string IdDepartamentoInvalido = "El ID del departamento debe ser un número mayor a 0";
        public const string PaisIdMayorCero = "El ID del pais debe ser mayor a 0";
        public const string DepartamentoIdMayorCero = "El ID del departamento debe ser mayor a 0";

        public const string UsuarioNoEncontrado = "Usuario no encontrado";
        public const string UsuarioCreado = "Usuario creado exitosamente";
        public const string UsuarioRegistradoNoRecuperado = "Usuario registrado pero no se pudo recuperar";


        public const string PaisesObtenidos = "Lista de países obtenida exitosamente";
        public const string DepartamentosObtenidos = "Lista de departamentos obtenida exitosamente";
        public const string MunicipiosObtenidos = "Lista de municipios obtenida exitosamente";
   
        public const string ErrorPrefijo = "Error: {0}";

    }
}
