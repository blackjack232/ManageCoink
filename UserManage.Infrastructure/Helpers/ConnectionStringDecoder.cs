using System.Text;
using UserManage.Domain.Constants;

namespace UserManage.Infrastructure.Helpers;

/// <summary>
/// Helper para decodificar cadenas de conexión que pueden estar en Base64.
/// </summary>
public static class ConnectionStringDecoder
{
    /// <summary>
    /// Decodifica una cadena de conexión desde Base64 si está codificada,
    /// o la retorna tal cual si está en texto plano.
    /// </summary>
    /// <param name="encodedConnectionString">Cadena de conexión potencialmente codificada en Base64</param>
    /// <param name="logAction">Acción opcional para logging (ej: Console.WriteLine o ILogger)</param>
    /// <returns>Cadena de conexión decodificada lista para usar</returns>
    /// <exception cref="ArgumentNullException">Si la cadena de conexión es null o vacía</exception>
    public static string Decode(string encodedConnectionString, Action<string>? logAction = null)
    {
        if (string.IsNullOrWhiteSpace(encodedConnectionString))
        {
            throw new ArgumentNullException(nameof(encodedConnectionString),
                InfrastructureMessages.ConnectionStringNullOrEmpty);
        }

        try
        {
            var base64Bytes = Convert.FromBase64String(encodedConnectionString);
            var decodedString = Encoding.UTF8.GetString(base64Bytes);

            if (IsValidConnectionString(decodedString))
            {
                logAction?.Invoke(InfrastructureMessages.ConnectionStringDecodedSuccess);
                return decodedString;
            }
            logAction?.Invoke(InfrastructureMessages.ConnectionStringInvalidDecoded);
            return encodedConnectionString;
        }
        catch (FormatException)
        {
            logAction?.Invoke(InfrastructureMessages.ConnectionStringPlainText);
            return encodedConnectionString;
        }
    }

    /// <summary>
    /// Codifica una cadena de conexión a Base64.
    /// </summary>
    /// <param name="connectionString">Cadena de conexión en texto plano</param>
    /// <returns>Cadena de conexión codificada en Base64</returns>
    public static string Encode(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentNullException(nameof(connectionString),
                InfrastructureMessages.ConnectionStringNullOrEmpty);
        }

        var bytes = Encoding.UTF8.GetBytes(connectionString);
        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// Valida si una cadena parece ser una cadena de conexión válida.
    /// </summary>
    private static bool IsValidConnectionString(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            return false;
        var keywords = new[] { "Host=", "Server=", "Database=", "Data Source=", "User", "Password" };

        return keywords.Any(keyword =>
            connectionString.Contains(keyword, StringComparison.OrdinalIgnoreCase));
    }
}
