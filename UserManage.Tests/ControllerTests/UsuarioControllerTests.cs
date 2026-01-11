
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Moq;
using UserManage.API.Controllers;
using UserManage.Application.Interface.Service;
using UserManage.Domain.Common;
using UserManage.Domain.Constants;
using UserManage.Domain.Dtos;

namespace UserManage.Tests.ControllerTests;

/// <summary>
/// Tests unitarios para el UsuarioController.
/// </summary>
public class UsuarioControllerTests
{
    private readonly Mock<IUsuarioService> _mockService;
    private readonly Mock<ILogger<UsuarioController>> _mockLogger;
    private readonly UsuarioController _controller;

    public UsuarioControllerTests()
    {
        _mockService = new Mock<IUsuarioService>();
        _mockLogger = new Mock<ILogger<UsuarioController>>();
        _controller = new UsuarioController(_mockService.Object, _mockLogger.Object);
    }

    #region CrearUsuario Tests - Casos Exitosos

    [Fact]
    public async Task CrearUsuario_ConDatosValidos_RetornaCreated201()
    {
        // Arrange
        var request = new ReqUsuarioDto
        {
            Nombre = "Juan Pérez",
            Telefono = "3001234567",
            PaisId = 1,
            DepartamentoId = 1,
            MunicipioId = 1,
            Direccion = "Calle 123 #45-67"
        };

        var usuarioCreado = new UsuarioResponseDto(
            id: 1,
            nombre: "Juan Pérez",
            telefono: "3001234567",
            pais: "Colombia",
            departamento: "Antioquia",
            municipio: "Medellín",
            direccion: "Calle 123 #45-67",
            fechaCreacion: DateTime.Now
        );

        var result = Result<UsuarioResponseDto>.SuccessResult(
            usuarioCreado,
            "Usuario creado exitosamente"
        );

        _mockService.Setup(s => s.CrearUsuario(It.IsAny<ReqUsuarioDto>()))
            .ReturnsAsync(result);

        // Act
        var response = await _controller.CrearUsuario(request);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(response);
        Assert.Equal(201, createdResult.StatusCode);
        Assert.Equal(nameof(_controller.ObtenerUsuarioById), createdResult.ActionName);
    }

    [Fact]
    public async Task CrearUsuario_VerificaQueSeInvoqueElServicio()
    {
        // Arrange
        var request = new ReqUsuarioDto
        {
            Nombre = "Test User",
            Telefono = "3009999999",
            PaisId = 1,
            DepartamentoId = 1,
            MunicipioId = 1,
            Direccion = "Test Address"
        };

        var usuarioCreado = new UsuarioResponseDto(1, "Test", "3009999999", "Col", "Ant", "Med", "Dir", DateTime.Now);
        var result = Result<UsuarioResponseDto>.SuccessResult(usuarioCreado);

        _mockService.Setup(s => s.CrearUsuario(It.IsAny<ReqUsuarioDto>()))
            .ReturnsAsync(result);

        // Act
        await _controller.CrearUsuario(request);

        // Assert
        _mockService.Verify(s => s.CrearUsuario(It.Is<ReqUsuarioDto>(
            r => r.Nombre == "Test User" && r.Telefono == "3009999999"
        )), Times.Once);
    }

    #endregion

    #region CrearUsuario Tests - Validaciones

    [Fact]
    public async Task CrearUsuario_ConModelStateInvalido_RetornaBadRequest400()
    {
        // Arrange
        var request = new ReqUsuarioDto { Nombre = "" }; // Nombre vacío
        _controller.ModelState.AddModelError("Nombre", "El nombre es requerido");

        // Act
        var response = await _controller.CrearUsuario(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(response);
        Assert.Equal(400, badRequestResult.StatusCode);

    }

    [Fact]
    public async Task CrearUsuario_ConRequestNull_RetornaBadRequest400()
    {
        // Arrange
        _controller.ModelState.Clear(); // Asegurar que ModelState es válido

        // Act
        var response = await _controller.CrearUsuario(null!);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(response);
        Assert.Equal(400, badRequestResult.StatusCode);

        // Usar helper para validar respuesta
        UsuarioTestHelper.AssertBadRequestWithMessage(badRequestResult, AppMessages.CuerpoVacio);
    }

    [Fact]
    public async Task CrearUsuario_CuandoServicioFallaValidacion_RetornaBadRequest400()
    {
        // Arrange
        var request = new ReqUsuarioDto
        {
            Nombre = "Juan",
            Telefono = "300",
            PaisId = 1,
            DepartamentoId = 1,
            MunicipioId = 1,
            Direccion = "Test"
        };

        var errors = new List<string> { "El teléfono debe tener al menos 10 dígitos" };
        var result = Result<UsuarioResponseDto>.FailureResult(
            AppMessages.ErrorValidacion,
            errors
        );

        _mockService.Setup(s => s.CrearUsuario(It.IsAny<ReqUsuarioDto>()))
            .ReturnsAsync(result);

        // Act
        var response = await _controller.CrearUsuario(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(response);
        Assert.Equal(400, badRequestResult.StatusCode);

    }

    #endregion

    #region CrearUsuario Tests - Errores

    [Fact]
    public async Task CrearUsuario_CuandoOcurreArgumentException_RetornaBadRequest400()
    {
        // Arrange
        var request = new ReqUsuarioDto
        {
            Nombre = "Test",
            Telefono = "3001234567",
            PaisId = -1, // ID inválido
            DepartamentoId = 1,
            MunicipioId = 1,
            Direccion = "Test"
        };

        _mockService.Setup(s => s.CrearUsuario(It.IsAny<ReqUsuarioDto>()))
            .ThrowsAsync(new ArgumentException("El ID del país debe ser mayor a 0"));

        // Act
        var response = await _controller.CrearUsuario(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(response);
        Assert.Equal(400, badRequestResult.StatusCode);

    }

    [Fact]
    public async Task CrearUsuario_CuandoOcurreExcepcion_RetornaError500()
    {
        // Arrange
        var request = new ReqUsuarioDto
        {
            Nombre = "Test",
            Telefono = "3001234567",
            PaisId = 1,
            DepartamentoId = 1,
            MunicipioId = 1,
            Direccion = "Test"
        };

        _mockService.Setup(s => s.CrearUsuario(It.IsAny<ReqUsuarioDto>()))
            .ThrowsAsync(new Exception("Error de conexión a base de datos"));

        // Act
        var response = await _controller.CrearUsuario(request);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(response);
        Assert.Equal(500, statusResult.StatusCode);

    }

    [Fact]
    public async Task CrearUsuario_ConNombreNull_LogeaNombreComoNA()
    {
        // Arrange
        var request = new ReqUsuarioDto
        {
            Nombre = null!,
            Telefono = "3001234567",
            PaisId = 1,
            DepartamentoId = 1,
            MunicipioId = 1,
            Direccion = "Test"
        };

        _mockService.Setup(s => s.CrearUsuario(It.IsAny<ReqUsuarioDto>()))
            .ThrowsAsync(new Exception("Error"));

        // Act
        await _controller.CrearUsuario(request);

        // Assert - Verificar que se usó "N/A" en el log
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("N/A")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    #endregion

    #region ObtenerUsuarioById Tests - Casos Exitosos

    [Fact]
    public async Task ObtenerUsuarioById_ConIdValido_RetornaOk200()
    {
        // Arrange
        var usuarioResponse = new UsuarioResponseDto(
            id: 1,
            nombre: "Juan Pérez",
            telefono: "3001234567",
            pais: "Colombia",
            departamento: "Antioquia",
            municipio: "Medellín",
            direccion: "Calle 123 #45-67",
            fechaCreacion: DateTime.Now
        );

        var result = Result<UsuarioResponseDto>.SuccessResult(usuarioResponse);
        _mockService.Setup(s => s.ObtenerUsuarioById(1)).ReturnsAsync(result);

        // Act
        var response = await _controller.ObtenerUsuarioById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(response);
        Assert.Equal(200, okResult.StatusCode);

    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(999999)]
    public async Task ObtenerUsuarioById_ConDiferentesIdsValidos_RetornaOk200(int id)
    {
        // Arrange
        var usuarioResponse = new UsuarioResponseDto(
            id, "Test", "300", "Col", "Ant", "Med", "Dir", DateTime.Now
        );
        var result = Result<UsuarioResponseDto>.SuccessResult(usuarioResponse);
        _mockService.Setup(s => s.ObtenerUsuarioById(id)).ReturnsAsync(result);

        // Act
        var response = await _controller.ObtenerUsuarioById(id);

        // Assert
        Assert.IsType<OkObjectResult>(response);
    }

    #endregion

    #region ObtenerUsuarioById Tests - Validaciones

    [Fact]
    public async Task ObtenerUsuarioById_ConIdCero_RetornaBadRequest400()
    {
        // Act
        var response = await _controller.ObtenerUsuarioById(0);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(response);
        Assert.Equal(400, badRequestResult.StatusCode);

        _mockService.Verify(s => s.ObtenerUsuarioById(It.IsAny<long>()), Times.Never);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    [InlineData(-999999)]
    public async Task ObtenerUsuarioById_ConIdsInvalidos_RetornaBadRequest400(long id)
    {
        // Act
        var response = await _controller.ObtenerUsuarioById(id);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(response);
        Assert.Equal(400, badRequestResult.StatusCode);

    }

    #endregion

    #region ObtenerUsuarioById Tests - Usuario No Encontrado

   

    [Fact]
    public async Task ObtenerUsuarioById_CuandoUsuarioInactivo_RetornaNotFound404()
    {
        // Arrange
        var result = Result<UsuarioResponseDto>.FailureResult("El usuario está inactivo");
        _mockService.Setup(s => s.ObtenerUsuarioById(1)).ReturnsAsync(result);

        // Act
        var response = await _controller.ObtenerUsuarioById(1);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(response);
        Assert.Equal(404, notFoundResult.StatusCode);
    }

    #endregion

    #region ObtenerUsuarioById Tests - Errores



    [Fact]
    public async Task ObtenerUsuarioById_CuandoOcurreNullReferenceException_RetornaError500()
    {
        // Arrange
        _mockService.Setup(s => s.ObtenerUsuarioById(It.IsAny<long>()))
            .ThrowsAsync(new NullReferenceException("Referencia nula"));

        // Act
        var response = await _controller.ObtenerUsuarioById(1);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(response);
        Assert.Equal(500, statusResult.StatusCode);
    }

    #endregion

    #region Integration Tests (Flujo completo)

    [Fact]
    public async Task FlujoCompleto_CrearYObtenerUsuario_Exitoso()
    {
        // Arrange - Crear
        var request = new ReqUsuarioDto
        {
            Nombre = "Juan Pérez",
            Telefono = "3001234567",
            PaisId = 1,
            DepartamentoId = 1,
            MunicipioId = 1,
            Direccion = "Calle 123"
        };

        var usuarioCreado = new UsuarioResponseDto(
            1, "Juan Pérez", "3001234567", "Colombia", "Antioquia", "Medellín",
            "Calle 123", DateTime.Now
        );

        _mockService.Setup(s => s.CrearUsuario(It.IsAny<ReqUsuarioDto>()))
            .ReturnsAsync(Result<UsuarioResponseDto>.SuccessResult(usuarioCreado));

        _mockService.Setup(s => s.ObtenerUsuarioById(1))
            .ReturnsAsync(Result<UsuarioResponseDto>.SuccessResult(usuarioCreado));

        // Act - Crear usuario
        var createResponse = await _controller.CrearUsuario(request);

        // Assert - Verificar creación
        var createdResult = Assert.IsType<CreatedAtActionResult>(createResponse);
        Assert.Equal(201, createdResult.StatusCode);

        // Act - Obtener usuario creado
        var getResponse = await _controller.ObtenerUsuarioById(1);

        // Assert - Verificar obtención
        var okResult = Assert.IsType<OkObjectResult>(getResponse);
        Assert.Equal(200, okResult.StatusCode);
    }

    [Fact]
    public async Task FlujoCompleto_CrearUsuarioDuplicado_RetornaError()
    {
        // Arrange
        var request = new ReqUsuarioDto
        {
            Nombre = "Juan Pérez",
            Telefono = "3001234567",
            PaisId = 1,
            DepartamentoId = 1,
            MunicipioId = 1,
            Direccion = "Test"
        };

        // Primera llamada exitosa
        var usuarioCreado = new UsuarioResponseDto(1, "Juan", "300", "Col", "Ant", "Med", "Dir", DateTime.Now);
        _mockService.SetupSequence(s => s.CrearUsuario(It.IsAny<ReqUsuarioDto>()))
            .ReturnsAsync(Result<UsuarioResponseDto>.SuccessResult(usuarioCreado))
            .ReturnsAsync(Result<UsuarioResponseDto>.FailureResult("El usuario ya existe"));

        // Act
        var firstResponse = await _controller.CrearUsuario(request);
        var secondResponse = await _controller.CrearUsuario(request);

        // Assert
        Assert.IsType<CreatedAtActionResult>(firstResponse);
        Assert.IsType<BadRequestObjectResult>(secondResponse);
    }

    #endregion
}
public static class UsuarioTestHelper
{
    public static ReqUsuarioDto CreateValidRequest(
        string nombre = "Juan Pérez",
        string telefono = "3001234567",
        long paisId = 1,
        long departamentoId = 1,
        long municipioId = 1,
        string direccion = "Calle 123 #45-67")
    {
        return new ReqUsuarioDto
        {
            Nombre = nombre,
            Telefono = telefono,
            PaisId = paisId,
            DepartamentoId = departamentoId,
            MunicipioId = municipioId,
            Direccion = direccion
        };
    }

    public static UsuarioResponseDto CreateUsuarioResponse(
        int id = 1,
        string nombre = "Juan Pérez",
        string telefono = "3001234567",
        string pais = "Colombia",
        string departamento = "Antioquia",
        string municipio = "Medellín",
        string direccion = "Calle 123 #45-67")
    {
        return new UsuarioResponseDto(
            id, nombre, telefono, pais, departamento, municipio, direccion, DateTime.Now
        );
    }

    /// <summary>
    /// Helper para validar respuestas con objetos anónimos sin usar dynamic
    /// </summary>
    public static void AssertBadRequestWithMessage(ObjectResult result, string expectedMessage)
    {
        Assert.NotNull(result.Value);

        var successProperty = result.Value.GetType().GetProperty("success");
        var messageProperty = result.Value.GetType().GetProperty("message");

        Assert.NotNull(successProperty);
        Assert.NotNull(messageProperty);

        var success = (bool)successProperty.GetValue(result.Value)!;
        var message = (string)messageProperty.GetValue(result.Value)!;

        Assert.False(success);
        Assert.Equal(expectedMessage, message);
    }
}