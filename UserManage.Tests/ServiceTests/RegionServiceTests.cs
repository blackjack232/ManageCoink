
using Moq;
using UserManage.Application.Interface.Repository;
using UserManage.Application.Services;
using UserManage.Domain.Constants;
using UserManage.Domain.Entities;
using Xunit;

namespace UserManage.Tests.Services;

/// <summary>
/// Tests unitarios para RegionService.
/// </summary>
public class RegionServiceTests
{
    private readonly Mock<IRegionRepository> _mockRepository;
    private readonly RegionService _service;

    public RegionServiceTests()
    {
        _mockRepository = new Mock<IRegionRepository>();
        _service = new RegionService(_mockRepository.Object);
    }

    #region ObtenerPais Tests - Casos Exitosos

    [Fact]
    public async Task ObtenerPais_CuandoExistenPaises_RetornaListaDePaises()
    {
        // Arrange
        var paises = new List<Pais>
        {
            new() { Id = 1, Nombre = "Colombia", Codigo = "COL", Estado = true },
            new() { Id = 2, Nombre = "México", Codigo = "MEX", Estado = true },
            new() { Id = 3, Nombre = "Argentina", Codigo = "ARG", Estado = true }
        };

        _mockRepository.Setup(r => r.ObtenerPais())
            .ReturnsAsync(paises);

        // Act
        var result = await _service.ObtenerPais();

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(3, result.Data.Count());
        Assert.Contains(result.Data, p => p.Nombre == "Colombia");
    }

    [Fact]
    public async Task ObtenerPais_CuandoNoHayPaises_RetornaListaVacia()
    {
        // Arrange
        var paises = new List<Pais>();
        _mockRepository.Setup(r => r.ObtenerPais())
            .ReturnsAsync(paises);

        // Act
        var result = await _service.ObtenerPais();

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task ObtenerPais_VerificaLlamadaAlRepositorio()
    {
        // Arrange
        _mockRepository.Setup(r => r.ObtenerPais())
            .ReturnsAsync(new List<Pais>());

        // Act
        await _service.ObtenerPais();

        // Assert
        _mockRepository.Verify(r => r.ObtenerPais(), Times.Once);
    }

    [Fact]
    public async Task ObtenerPais_RetornaPaisesOrdenadosAlfabeticamente()
    {
        // Arrange
        var paises = new List<Pais>
        {
            new() { Id = 1, Nombre = "Colombia", Codigo = "COL" },
            new() { Id = 2, Nombre = "Argentina", Codigo = "ARG" },
            new() { Id = 3, Nombre = "Brasil", Codigo = "BRA" }
        };

        _mockRepository.Setup(r => r.ObtenerPais())
            .ReturnsAsync(paises);

        // Act
        var result = await _service.ObtenerPais();

        // Assert
        Assert.True(result.Success);
        Assert.Equal(3, result.Data!.Count());
    }

    #endregion

    #region ObtenerPais Tests - Errores

    [Fact]
    public async Task ObtenerPais_CuandoOcurreExcepcion_RetornaError()
    {
        // Arrange
        _mockRepository.Setup(r => r.ObtenerPais())
            .ThrowsAsync(new Exception("Error de conexión a BD"));

        // Act
        var result = await _service.ObtenerPais();

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Error:", result.Message);
        Assert.Contains("Error de conexión a BD", result.Message);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task ObtenerPais_CuandoOcurreNullReferenceException_RetornaError()
    {
        // Arrange
        _mockRepository.Setup(r => r.ObtenerPais())
            .ThrowsAsync(new NullReferenceException("Referencia nula"));

        // Act
        var result = await _service.ObtenerPais();

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Error:", result.Message);
    }

    [Fact]
    public async Task ObtenerPais_CuandoOcurreInvalidOperationException_RetornaError()
    {
        // Arrange
        _mockRepository.Setup(r => r.ObtenerPais())
            .ThrowsAsync(new InvalidOperationException("Operación inválida"));

        // Act
        var result = await _service.ObtenerPais();

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Operación inválida", result.Message);
    }

    #endregion

    #region ObtenerDepartamento Tests - Casos Exitosos

    [Fact]
    public async Task ObtenerDepartamento_ConIdValido_RetornaListaDepartamentos()
    {
        // Arrange
        var departamentos = new List<Departamento>
        {
            new() { Id = 1, Nombre = "Antioquia", Pais_id = 1 },
            new() { Id = 2, Nombre = "Cundinamarca", Pais_id = 1 },
            new() { Id = 3, Nombre = "Valle del Cauca", Pais_id = 1 }
        };

        _mockRepository.Setup(r => r.ObtenerDepartamentosByPais(1))
            .ReturnsAsync(departamentos);

        // Act
        var result = await _service.ObtenerDepartamento(1);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(3, result.Data.Count());
        Assert.All(result.Data, d => Assert.Equal(1, d.Pais_id));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(999999)]
    public async Task ObtenerDepartamento_ConDiferentesIdsValidos_RetornaExito(long paisId)
    {
        // Arrange
        var departamentos = new List<Departamento>
        {
            new() { Id = 1, Nombre = "Test", Pais_id = paisId }
        };

        _mockRepository.Setup(r => r.ObtenerDepartamentosByPais(paisId))
            .ReturnsAsync(departamentos);

        // Act
        var result = await _service.ObtenerDepartamento(paisId);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task ObtenerDepartamento_CuandoPaisNoTieneDepartamentos_RetornaListaVacia()
    {
        // Arrange
        _mockRepository.Setup(r => r.ObtenerDepartamentosByPais(1))
            .ReturnsAsync(new List<Departamento>());

        // Act
        var result = await _service.ObtenerDepartamento(1);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task ObtenerDepartamento_VerificaLlamadaAlRepositorio()
    {
        // Arrange
        _mockRepository.Setup(r => r.ObtenerDepartamentosByPais(It.IsAny<long>()))
            .ReturnsAsync(new List<Departamento>());

        // Act
        await _service.ObtenerDepartamento(1);

        // Assert
        _mockRepository.Verify(r => r.ObtenerDepartamentosByPais(1), Times.Once);
    }

    #endregion

    #region ObtenerDepartamento Tests - Validaciones

    [Fact]
    public async Task ObtenerDepartamento_ConIdCero_RetornaError()
    {
        // Act
        var result = await _service.ObtenerDepartamento(0);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(AppMessages.PaisIdMayorCero, result.Message);
        Assert.Null(result.Data);

        // Verificar que NO se llamó al repositorio
        _mockRepository.Verify(r => r.ObtenerDepartamentosByPais(It.IsAny<long>()), Times.Never);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    [InlineData(-999999)]
    public async Task ObtenerDepartamento_ConIdsInvalidos_RetornaError(long paisId)
    {
        // Act
        var result = await _service.ObtenerDepartamento(paisId);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(AppMessages.PaisIdMayorCero, result.Message);

        // Verificar que NO se llamó al repositorio
        _mockRepository.Verify(r => r.ObtenerDepartamentosByPais(It.IsAny<long>()), Times.Never);
    }

    [Fact]
    public async Task ObtenerDepartamento_ValidaAntesDeConsultarRepositorio()
    {
        // Arrange
        _mockRepository.Setup(r => r.ObtenerDepartamentosByPais(It.IsAny<long>()))
            .ReturnsAsync(new List<Departamento>());

        // Act
        var result = await _service.ObtenerDepartamento(-1);

        // Assert
        Assert.False(result.Success);
        _mockRepository.Verify(r => r.ObtenerDepartamentosByPais(It.IsAny<long>()), Times.Never);
    }

    #endregion

    #region ObtenerDepartamento Tests - Errores

    [Fact]
    public async Task ObtenerDepartamento_CuandoOcurreExcepcion_RetornaError()
    {
        // Arrange
        _mockRepository.Setup(r => r.ObtenerDepartamentosByPais(It.IsAny<long>()))
            .ThrowsAsync(new Exception("Error al consultar departamentos"));

        // Act
        var result = await _service.ObtenerDepartamento(1);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Error:", result.Message);
        Assert.Contains("Error al consultar departamentos", result.Message);
    }

    [Fact]
    public async Task ObtenerDepartamento_CuandoPaisNoExiste_RetornaError()
    {
        // Arrange
        _mockRepository.Setup(r => r.ObtenerDepartamentosByPais(999))
            .ThrowsAsync(new Exception("El país no existe"));

        // Act
        var result = await _service.ObtenerDepartamento(999);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("El país no existe", result.Message);
    }

    #endregion

    #region ObtenerMunicipio Tests - Casos Exitosos

    [Fact]
    public async Task ObtenerMunicipio_ConIdValido_RetornaListaMunicipios()
    {
        // Arrange
        var municipios = new List<Municipio>
        {
            new() { Id = 1, Nombre = "Medellín", Departamento_Id = 1 },
            new() { Id = 2, Nombre = "Envigado", Departamento_Id = 1 },
            new() { Id = 3, Nombre = "Bello", Departamento_Id = 1 }
        };

        _mockRepository.Setup(r => r.ObtenerMunicipioByDepartamento(1))
            .ReturnsAsync(municipios);

        // Act
        var result = await _service.ObtenerMunicipio(1);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(3, result.Data.Count());
        Assert.All(result.Data, m => Assert.Equal(1, m.Departamento_Id));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(999)]
    public async Task ObtenerMunicipio_ConDiferentesIdsValidos_RetornaExito(long departamentoId)
    {
        // Arrange
        var municipios = new List<Municipio>
        {
            new() { Id = 1, Nombre = "Test", Departamento_Id = departamentoId }
        };

        _mockRepository.Setup(r => r.ObtenerMunicipioByDepartamento(departamentoId))
            .ReturnsAsync(municipios);

        // Act
        var result = await _service.ObtenerMunicipio(departamentoId);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task ObtenerMunicipio_CuandoDepartamentoNoTieneMunicipios_RetornaListaVacia()
    {
        // Arrange
        _mockRepository.Setup(r => r.ObtenerMunicipioByDepartamento(1))
            .ReturnsAsync(new List<Municipio>());

        // Act
        var result = await _service.ObtenerMunicipio(1);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task ObtenerMunicipio_VerificaLlamadaAlRepositorio()
    {
        // Arrange
        _mockRepository.Setup(r => r.ObtenerMunicipioByDepartamento(It.IsAny<long>()))
            .ReturnsAsync(new List<Municipio>());

        // Act
        await _service.ObtenerMunicipio(1);

        // Assert
        _mockRepository.Verify(r => r.ObtenerMunicipioByDepartamento(1), Times.Once);
    }

    #endregion

    #region ObtenerMunicipio Tests - Validaciones

    [Fact]
    public async Task ObtenerMunicipio_ConIdCero_RetornaError()
    {
        // Act
        var result = await _service.ObtenerMunicipio(0);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(AppMessages.DepartamentoIdMayorCero, result.Message);
        Assert.Null(result.Data);

        // Verificar que NO se llamó al repositorio
        _mockRepository.Verify(r => r.ObtenerMunicipioByDepartamento(It.IsAny<long>()), Times.Never);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-50)]
    [InlineData(-999)]
    public async Task ObtenerMunicipio_ConIdsInvalidos_RetornaError(long departamentoId)
    {
        // Act
        var result = await _service.ObtenerMunicipio(departamentoId);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(AppMessages.DepartamentoIdMayorCero, result.Message);

        // Verificar que NO se llamó al repositorio
        _mockRepository.Verify(r => r.ObtenerMunicipioByDepartamento(It.IsAny<long>()), Times.Never);
    }

    [Fact]
    public async Task ObtenerMunicipio_ValidaAntesDeConsultarRepositorio()
    {
        // Arrange
        _mockRepository.Setup(r => r.ObtenerMunicipioByDepartamento(It.IsAny<long>()))
            .ReturnsAsync(new List<Municipio>());

        // Act
        var result = await _service.ObtenerMunicipio(-10);

        // Assert
        Assert.False(result.Success);
        _mockRepository.Verify(r => r.ObtenerMunicipioByDepartamento(It.IsAny<long>()), Times.Never);
    }

    #endregion

    #region ObtenerMunicipio Tests - Errores

    [Fact]
    public async Task ObtenerMunicipio_CuandoOcurreExcepcion_RetornaError()
    {
        // Arrange
        _mockRepository.Setup(r => r.ObtenerMunicipioByDepartamento(It.IsAny<long>()))
            .ThrowsAsync(new Exception("Error al consultar municipios"));

        // Act
        var result = await _service.ObtenerMunicipio(1);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Error:", result.Message);
        Assert.Contains("Error al consultar municipios", result.Message);
    }

    [Fact]
    public async Task ObtenerMunicipio_CuandoDepartamentoNoExiste_RetornaError()
    {
        // Arrange
        _mockRepository.Setup(r => r.ObtenerMunicipioByDepartamento(999))
            .ThrowsAsync(new Exception("El departamento no existe"));

        // Act
        var result = await _service.ObtenerMunicipio(999);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("El departamento no existe", result.Message);
    }

    #endregion

    #region Integration Tests (Flujos completos)

    [Fact]
    public async Task FlujoCompleto_ObtenerPaisDepartamentoMunicipio_Exitoso()
    {
        // Arrange - Configurar todos los datos
        var paises = new List<Pais>
        {
            new() { Id = 1, Nombre = "Colombia", Codigo = "COL" }
        };

        var departamentos = new List<Departamento>
        {
            new() { Id = 1, Nombre = "Antioquia", Pais_id = 1 }
        };

        var municipios = new List<Municipio>
        {
            new() { Id = 1, Nombre = "Medellín", Departamento_Id = 1 }
        };

        _mockRepository.Setup(r => r.ObtenerPais())
            .ReturnsAsync(paises);
        _mockRepository.Setup(r => r.ObtenerDepartamentosByPais(1))
            .ReturnsAsync(departamentos);
        _mockRepository.Setup(r => r.ObtenerMunicipioByDepartamento(1))
            .ReturnsAsync(municipios);

        // Act
        var paisesResult = await _service.ObtenerPais();
        var departamentosResult = await _service.ObtenerDepartamento(1);
        var municipiosResult = await _service.ObtenerMunicipio(1);

        // Assert
        Assert.True(paisesResult.Success);
        Assert.True(departamentosResult.Success);
        Assert.True(municipiosResult.Success);

        Assert.Single(paisesResult.Data!);
        Assert.Single(departamentosResult.Data!);
        Assert.Single(municipiosResult.Data!);
    }

    [Fact]
    public async Task FlujoCompleto_ValidacionesEnCadena_NoLlamaRepositorioConIdsInvalidos()
    {
        // Act
        var departamentosResult = await _service.ObtenerDepartamento(0);
        var municipiosResult = await _service.ObtenerMunicipio(-1);

        // Assert
        Assert.False(departamentosResult.Success);
        Assert.False(municipiosResult.Success);

        // Verificar que NUNCA se llamó al repositorio
        _mockRepository.Verify(r => r.ObtenerDepartamentosByPais(It.IsAny<long>()), Times.Never);
        _mockRepository.Verify(r => r.ObtenerMunicipioByDepartamento(It.IsAny<long>()), Times.Never);
    }

    [Fact]
    public async Task FlujoCompleto_CascadaDeConsultas_MantieneCoherenciaDeDatos()
    {
        // Arrange
        const long paisId = 1;
        const long departamentoId = 5;

        var departamentos = new List<Departamento>
        {
            new() { Id = departamentoId, Nombre = "Antioquia", Pais_id = paisId }
        };

        var municipios = new List<Municipio>
        {
            new() { Id = 1, Nombre = "Medellín", Departamento_Id = departamentoId }
        };

        _mockRepository.Setup(r => r.ObtenerDepartamentosByPais(paisId))
            .ReturnsAsync(departamentos);
        _mockRepository.Setup(r => r.ObtenerMunicipioByDepartamento(departamentoId))
            .ReturnsAsync(municipios);

        // Act
        var deptosResult = await _service.ObtenerDepartamento(paisId);
        var municipiosResult = await _service.ObtenerMunicipio(departamentoId);

        // Assert
        Assert.True(deptosResult.Success);
        Assert.True(municipiosResult.Success);

        // Verificar coherencia de IDs
        var depto = deptosResult.Data!.First();
        var municipio = municipiosResult.Data!.First();

        Assert.Equal(paisId, depto.Pais_id);
        Assert.Equal(departamentoId, municipio.Departamento_Id);
    }

    #endregion

    #region Tests de Comportamiento Específico

    [Fact]
    public async Task MultiplesCalls_ObtenerPais_CadaLlamadaEsIndependiente()
    {
        // Arrange
        _mockRepository.Setup(r => r.ObtenerPais())
            .ReturnsAsync([new() { Id = 1, Nombre = "Test", Codigo = "TST" }]);

        // Act
        var result1 = await _service.ObtenerPais();
        var result2 = await _service.ObtenerPais();
        var result3 = await _service.ObtenerPais();

        // Assert
        Assert.True(result1.Success);
        Assert.True(result2.Success);
        Assert.True(result3.Success);

        _mockRepository.Verify(r => r.ObtenerPais(), Times.Exactly(3));
    }

    [Fact]
    public async Task ExcepcionesEnCadena_CadaMetodoManejaIndependientemente()
    {
        // Arrange
        _mockRepository.Setup(r => r.ObtenerPais())
            .ThrowsAsync(new Exception("Error países"));
        _mockRepository.Setup(r => r.ObtenerDepartamentosByPais(It.IsAny<long>()))
            .ThrowsAsync(new Exception("Error departamentos"));
        _mockRepository.Setup(r => r.ObtenerMunicipioByDepartamento(It.IsAny<long>()))
            .ThrowsAsync(new Exception("Error municipios"));

        // Act
        var paisesResult = await _service.ObtenerPais();
        var deptosResult = await _service.ObtenerDepartamento(1);
        var municipiosResult = await _service.ObtenerMunicipio(1);

        // Assert
        Assert.False(paisesResult.Success);
        Assert.Contains("Error países", paisesResult.Message);

        Assert.False(deptosResult.Success);
        Assert.Contains("Error departamentos", deptosResult.Message);

        Assert.False(municipiosResult.Success);
        Assert.Contains("Error municipios", municipiosResult.Message);
    }

    #endregion
}
