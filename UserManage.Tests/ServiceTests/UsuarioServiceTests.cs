using Moq;
using UserManage.Application.Interface.Repository;
using UserManage.Application.Services;
using UserManage.Domain.Constants;
using UserManage.Domain.Dtos;
using UserManage.Domain.Entities;
using UserManage.Tests.ControllerTests;

namespace UserManage.Tests.ServiceTests;

/// <summary>
/// Tests unitarios para UsuarioService.
/// </summary>
public class UsuarioServiceTests
{
    private readonly Mock<IUsuarioRepository> _mockRepository;
    private readonly UsuarioService _service;

    public UsuarioServiceTests()
    {
        _mockRepository = new Mock<IUsuarioRepository>();
        _service = new UsuarioService(_mockRepository.Object);
    }

    #region CrearUsuario Tests - Casos Exitosos

    [Fact]
    public async Task CrearUsuario_ConDatosValidos_RetornaExito()
    {
        // Arrange
        var request = UsuarioTestHelper.CreateValidRequest();

        var usuario = new Usuario
        {
            Id = 1,
            Nombre = "Juan Pérez",
            Telefono = "3001234567",
            Pais = "Colombia",
            Departamento = "Antioquia",
            Municipio = "Medellín",
            Direccion = "Calle 123 #45-67",
            FechaCreacion = DateTime.Now
        };

        _mockRepository.Setup(r => r.CrearUsuario(It.IsAny<ReqUsuarioDto>()))
            .ReturnsAsync((1L, "Usuario creado exitosamente", true));

        _mockRepository.Setup(r => r.ObtenerUsuarioById(1))
            .ReturnsAsync(usuario);

        // Act
        var result = await _service.CrearUsuario(request);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(1, result.Data.Id);
        Assert.Equal("Juan Pérez", result.Data.Nombre);
        Assert.Equal("Colombia", result.Data.Pais);
        Assert.Equal("Usuario creado exitosamente", result.Message);
    }

    [Fact]
    public async Task CrearUsuario_VerificaLlamadasAlRepositorio()
    {
        // Arrange
        var request = UsuarioTestHelper.CreateValidRequest();
        var usuario = CreateTestUsuario();

        _mockRepository.Setup(r => r.CrearUsuario(It.IsAny<ReqUsuarioDto>()))
            .ReturnsAsync((1L, "Éxito", true));
        _mockRepository.Setup(r => r.ObtenerUsuarioById(1))
            .ReturnsAsync(usuario);

        // Act
        await _service.CrearUsuario(request);

        // Assert
        _mockRepository.Verify(r => r.CrearUsuario(It.Is<ReqUsuarioDto>(
            dto => dto.Nombre == request.Nombre && dto.Telefono == request.Telefono
        )), Times.Once);

        _mockRepository.Verify(r => r.ObtenerUsuarioById(1), Times.Once);
    }

    [Fact]
    public async Task CrearUsuario_ConCamposNullEnUsuario_RetornaStringsVacios()
    {
        // Arrange
        var request = UsuarioTestHelper.CreateValidRequest();

        var usuario = new Usuario
        {
            Id = 1,
            Nombre = "Test",
            Telefono = "3001234567",
            Pais = null, // Campos null
            Departamento = null,
            Municipio = null,
            Direccion = "Test",
            FechaCreacion = DateTime.Now
        };

        _mockRepository.Setup(r => r.CrearUsuario(It.IsAny<ReqUsuarioDto>()))
            .ReturnsAsync((1L, "Éxito", true));
        _mockRepository.Setup(r => r.ObtenerUsuarioById(1))
            .ReturnsAsync(usuario);

        // Act
        var result = await _service.CrearUsuario(request);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(string.Empty, result.Data!.Pais);
        Assert.Equal(string.Empty, result.Data.Departamento);
        Assert.Equal(string.Empty, result.Data.Municipio);
    }

    #endregion

    #region CrearUsuario Tests - Validaciones

    [Fact]
    public async Task CrearUsuario_ConNombreVacio_RetornaErrorValidacion()
    {
        // Arrange
        var request = new ReqUsuarioDto
        {
            Nombre = "", // Nombre vacío
            Telefono = "3001234567",
            PaisId = 1,
            DepartamentoId = 1,
            MunicipioId = 1,
            Direccion = "Calle 123"
        };

        // Act
        var result = await _service.CrearUsuario(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(AppMessages.ErrorValidacion, result.Message);
        Assert.NotNull(result.Errors);
        Assert.NotEmpty(result.Errors);

        // Verificar que NO se llamó al repositorio
        _mockRepository.Verify(r => r.CrearUsuario(It.IsAny<ReqUsuarioDto>()), Times.Never);
    }

    [Fact]
    public async Task CrearUsuario_ConTelefonoInvalido_RetornaErrorValidacion()
    {
        // Arrange
        var request = new ReqUsuarioDto
        {
            Nombre = "Juan Pérez",
            Telefono = "123", // Teléfono muy corto
            PaisId = 1,
            DepartamentoId = 1,
            MunicipioId = 1,
            Direccion = "Calle 123"
        };

        // Act
        var result = await _service.CrearUsuario(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(AppMessages.ErrorValidacion, result.Message);
        Assert.NotNull(result.Errors);
        Assert.Contains(result.Errors, e => e.Contains("teléfono") || e.Contains("telefono"));
    }

    [Fact]
    public async Task CrearUsuario_ConDireccionVacia_RetornaErrorValidacion()
    {
        // Arrange
        var request = new ReqUsuarioDto
        {
            Nombre = "Juan Pérez",
            Telefono = "3001234567",
            PaisId = 1,
            DepartamentoId = 1,
            MunicipioId = 1,
            Direccion = "" // Dirección vacía
        };

        // Act
        var result = await _service.CrearUsuario(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(AppMessages.ErrorValidacion, result.Message);
        Assert.NotNull(result.Errors);
    }

    [Theory]
    [InlineData(0, 1, 1)]
    [InlineData(1, 0, 1)]
    [InlineData(1, 1, 0)]
    [InlineData(-1, 1, 1)]
    [InlineData(1, -1, 1)]
    [InlineData(1, 1, -1)]
    public async Task CrearUsuario_ConIdsInvalidos_RetornaErrorValidacion(
        long paisId, long departamentoId, long municipioId)
    {
        // Arrange
        var request = new ReqUsuarioDto
        {
            Nombre = "Juan Pérez",
            Telefono = "3001234567",
            PaisId = paisId,
            DepartamentoId = departamentoId,
            MunicipioId = municipioId,
            Direccion = "Calle 123"
        };

        // Act
        var result = await _service.CrearUsuario(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(AppMessages.ErrorValidacion, result.Message);
    }

    [Fact]
    public async Task CrearUsuario_ConMultiplesErroresValidacion_RetornaTodosLosErrores()
    {
        // Arrange
        var request = new ReqUsuarioDto
        {
            Nombre = "", // Error
            Telefono = "123", // Error
            PaisId = 0, // Error
            DepartamentoId = 1,
            MunicipioId = 1,
            Direccion = "" // Error
        };

        // Act
        var result = await _service.CrearUsuario(request);

        // Assert
        Assert.False(result.Success);
        Assert.NotNull(result.Errors);
        Assert.True(result.Errors.Count >= 2, "Debe haber al menos 2 errores de validación");
    }

    #endregion

    #region CrearUsuario Tests - Errores del Repositorio

    [Fact]
    public async Task CrearUsuario_CuandoRepositorioFalla_RetornaError()
    {
        // Arrange
        var request = UsuarioTestHelper.CreateValidRequest();

        _mockRepository.Setup(r => r.CrearUsuario(It.IsAny<ReqUsuarioDto>()))
            .ReturnsAsync((0L, "El país no existe", false));

        // Act
        var result = await _service.CrearUsuario(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("El país no existe", result.Message);
        Assert.Null(result.Data);

        // Verificar que NO se intentó obtener el usuario
        _mockRepository.Verify(r => r.ObtenerUsuarioById(It.IsAny<long>()), Times.Never);
    }

    [Fact]
    public async Task CrearUsuario_CuandoNoSeEncuentraUsuarioCreado_RetornaError()
    {
        // Arrange
        var request = UsuarioTestHelper.CreateValidRequest();

        _mockRepository.Setup(r => r.CrearUsuario(It.IsAny<ReqUsuarioDto>()))
            .ReturnsAsync((1L, "Usuario creado", true));

        _mockRepository.Setup(r => r.ObtenerUsuarioById(1))
            .ReturnsAsync((Usuario?)null); // No se encuentra el usuario

        // Act
        var result = await _service.CrearUsuario(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(AppMessages.UsuarioRegistradoNoRecuperado, result.Message);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task CrearUsuario_CuandoOcurreExcepcion_RetornaErrorInterno()
    {
        // Arrange
        var request = UsuarioTestHelper.CreateValidRequest();

        _mockRepository.Setup(r => r.CrearUsuario(It.IsAny<ReqUsuarioDto>()))
            .ThrowsAsync(new Exception("Error de conexión a BD"));

        // Act
        var result = await _service.CrearUsuario(request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Error interno", result.Message);
        Assert.Contains("Error de conexión a BD", result.Message);
    }

    [Fact]
    public async Task CrearUsuario_CuandoObtenerUsuarioLanzaExcepcion_RetornaErrorInterno()
    {
        // Arrange
        var request = UsuarioTestHelper.CreateValidRequest();

        _mockRepository.Setup(r => r.CrearUsuario(It.IsAny<ReqUsuarioDto>()))
            .ReturnsAsync((1L, "Usuario creado", true));

        _mockRepository.Setup(r => r.ObtenerUsuarioById(1))
            .ThrowsAsync(new Exception("Error al consultar usuario"));

        // Act
        var result = await _service.CrearUsuario(request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Error interno", result.Message);
    }

    #endregion

    #region ObtenerUsuarioById Tests - Casos Exitosos

    [Fact]
    public async Task ObtenerUsuarioById_ConIdValido_RetornaUsuario()
    {
        // Arrange
        var usuario = CreateTestUsuario();
        _mockRepository.Setup(r => r.ObtenerUsuarioById(1))
            .ReturnsAsync(usuario);

        // Act
        var result = await _service.ObtenerUsuarioById(1);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(1, result.Data.Id);
        Assert.Equal("Juan Pérez", result.Data.Nombre);
        Assert.Equal("3001234567", result.Data.Telefono);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(999999)]
    public async Task ObtenerUsuarioById_ConDiferentesIdsValidos_RetornaUsuario(long id)
    {
        // Arrange
        var usuario = CreateTestUsuario(id);
        _mockRepository.Setup(r => r.ObtenerUsuarioById(id))
            .ReturnsAsync(usuario);

        // Act
        var result = await _service.ObtenerUsuarioById(id);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(id, result.Data.Id);
    }

    [Fact]
    public async Task ObtenerUsuarioById_VerificaLlamadaAlRepositorio()
    {
        // Arrange
        var usuario = CreateTestUsuario();
        _mockRepository.Setup(r => r.ObtenerUsuarioById(1))
            .ReturnsAsync(usuario);

        // Act
        await _service.ObtenerUsuarioById(1);

        // Assert
        _mockRepository.Verify(r => r.ObtenerUsuarioById(1), Times.Once);
    }

    #endregion

    #region ObtenerUsuarioById Tests - Validaciones

    [Fact]
    public async Task ObtenerUsuarioById_ConIdCero_RetornaError()
    {
        // Act
        var result = await _service.ObtenerUsuarioById(0);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(AppMessages.IdMayorCero, result.Message);
        Assert.Null(result.Data);

        // Verificar que NO se llamó al repositorio
        _mockRepository.Verify(r => r.ObtenerUsuarioById(It.IsAny<long>()), Times.Never);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    [InlineData(-999999)]
    public async Task ObtenerUsuarioById_ConIdsInvalidos_RetornaError(long id)
    {
        // Act
        var result = await _service.ObtenerUsuarioById(id);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(AppMessages.IdMayorCero, result.Message);

        // Verificar que NO se llamó al repositorio
        _mockRepository.Verify(r => r.ObtenerUsuarioById(It.IsAny<long>()), Times.Never);
    }

    #endregion

    #region ObtenerUsuarioById Tests - Usuario No Encontrado

    [Fact]
    public async Task ObtenerUsuarioById_CuandoUsuarioNoExiste_RetornaError()
    {
        // Arrange
        _mockRepository.Setup(r => r.ObtenerUsuarioById(999))
            .ReturnsAsync((Usuario?)null);

        // Act
        var result = await _service.ObtenerUsuarioById(999);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(AppMessages.UsuarioNoEncontrado, result.Message);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task ObtenerUsuarioById_VerificaQueSeInvoqueRepositorioAntesDeRetornarNoEncontrado()
    {
        // Arrange
        _mockRepository.Setup(r => r.ObtenerUsuarioById(It.IsAny<long>()))
            .ReturnsAsync((Usuario?)null);

        // Act
        await _service.ObtenerUsuarioById(100);

        // Assert
        _mockRepository.Verify(r => r.ObtenerUsuarioById(100), Times.Once);
    }

    #endregion

    #region ObtenerUsuarioById Tests - Errores

    [Fact]
    public async Task ObtenerUsuarioById_CuandoOcurreExcepcion_RetornaErrorInterno()
    {
        // Arrange
        _mockRepository.Setup(r => r.ObtenerUsuarioById(It.IsAny<long>()))
            .ThrowsAsync(new Exception("Error de base de datos"));

        // Act
        var result = await _service.ObtenerUsuarioById(1);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Error interno", result.Message);
        Assert.Contains("Error de base de datos", result.Message);
    }

    [Fact]
    public async Task ObtenerUsuarioById_CuandoOcurreNullReferenceException_RetornaErrorInterno()
    {
        // Arrange
        _mockRepository.Setup(r => r.ObtenerUsuarioById(It.IsAny<long>()))
            .ThrowsAsync(new NullReferenceException("Referencia nula"));

        // Act
        var result = await _service.ObtenerUsuarioById(1);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Error interno", result.Message);
    }

    #endregion

    #region Integration Tests (Flujos completos)

    [Fact]
    public async Task FlujoCompleto_CrearYObtenerUsuario_Exitoso()
    {
        // Arrange - Datos para crear
        var request = UsuarioTestHelper.CreateValidRequest();
        var usuario = CreateTestUsuario();

        _mockRepository.Setup(r => r.CrearUsuario(It.IsAny<ReqUsuarioDto>()))
            .ReturnsAsync((1L, "Usuario creado exitosamente", true));
        _mockRepository.Setup(r => r.ObtenerUsuarioById(1))
            .ReturnsAsync(usuario);

        // Act - Crear usuario
        var createResult = await _service.CrearUsuario(request);

        // Assert - Verificar creación
        Assert.True(createResult.Success);
        Assert.NotNull(createResult.Data);
        var usuarioId = createResult.Data.Id;

        // Act - Obtener usuario creado
        var getResult = await _service.ObtenerUsuarioById(usuarioId);

        // Assert - Verificar obtención
        Assert.True(getResult.Success);
        Assert.Equal(usuarioId, getResult.Data!.Id);
        Assert.Equal(createResult.Data.Nombre, getResult.Data.Nombre);
    }

    [Fact]
    public async Task FlujoCompleto_CrearUsuarioConDatosInvalidos_NoLlamaRepositorio()
    {
        // Arrange
        var request = new ReqUsuarioDto
        {
            Nombre = "", // Inválido
            Telefono = "123", // Inválido
            PaisId = 0, // Inválido
            DepartamentoId = 1,
            MunicipioId = 1,
            Direccion = ""
        };

        // Act
        var result = await _service.CrearUsuario(request);

        // Assert
        Assert.False(result.Success);

        // Verificar que NUNCA se llamó al repositorio
        _mockRepository.Verify(r => r.CrearUsuario(It.IsAny<ReqUsuarioDto>()), Times.Never);
        _mockRepository.Verify(r => r.ObtenerUsuarioById(It.IsAny<long>()), Times.Never);
    }

    #endregion

    #region Helper Methods

    private static Usuario CreateTestUsuario(long id = 1)
    {
        return new Usuario
        {
            Id = id,
            Nombre = "Juan Pérez",
            Telefono = "3001234567",
            Pais = "Colombia",
            Departamento = "Antioquia",
            Municipio = "Medellín",
            Direccion = "Calle 123 #45-67",
            FechaCreacion = DateTime.Now
        };
    }

    #endregion
}

