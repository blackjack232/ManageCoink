using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManage.Domain.Constants
{
    /// <summary>
    /// Constantes de mensajes de log.
    /// </summary>
    public static class LogMessages
    {
        // Logs de  ususario
        public const string ValidacionFallida = "Validacion de modelo fallida al crear usuario. Errores: {Errors}";
        public const string ErrorArgumento = "Error de argumento al crear usuario. Nombre: {Nombre}";
        public const string ErrorCrearUsuario = "Error interno al crear usuario. Nombre: {Nombre}, Excepción: {ExceptionType}";
        public const string UsuarioNoEncontradoLog = "Usuario no encontrado. ID: {UserId}, Mensaje: {Message}";
        public const string ErrorObtenerUsuario = "Error interno al obtener usuario. ID: {UserId}, Excepcion: {ExceptionType}";

        // Logs de Regin
        public const string ErrorObtenerPaises = "Error al obtener países: {Message}";
        public const string ErrorInternoObtenerPaises = "Error interno al obtener países. Excepción: {ExceptionType}";
        public const string PaisIdInvalidoLog = "Intento de consultar departamentos con PaisId invalido: {PaisId}";
        public const string ErrorObtenerDepartamentos = "Error al obtener departamentos. PaisId: {PaisId}, Mensaje: {Message}";
        public const string ErrorInternoDepartamentos = "Error interno al obtener departamentos. PaisId: {PaisId}, Excepción: {ExceptionType}";
        public const string DepartamentoIdInvalidoLog = "Intento de consultar municipios con DepartamentoId inválido: {DepartamentoId}";
        public const string ErrorObtenerMunicipios = "Error al obtener municipios. DepartamentoId: {DepartamentoId}, Mensaje: {Message}";
        public const string ErrorInternoMunicipios = "Error interno al obtener municipios. DepartamentoId: {DepartamentoId}, Excepción: {ExceptionType}";
    }
}
