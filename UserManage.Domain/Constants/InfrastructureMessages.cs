namespace UserManage.Domain.Constants;

public static class InfrastructureMessages
{
    public const string ConnectionStringNullOrEmpty = "La cadena de conexión no puede ser nula o vacía";
    public const string ConnectionStringDecodedSuccess = "✓ Cadena de conexión decodificada correctamente desde Base64";
    public const string ConnectionStringInvalidDecoded = "⚠ La decodificación no produjo una cadena de conexión válida, usando texto plano";
    public const string ConnectionStringPlainText = "ℹ La cadena de conexión no está en Base64, usando texto plano";
    public const string ConnectionStringNotConfigured = "La conexión a la base de datos '{0}' no fue establecida.";
}