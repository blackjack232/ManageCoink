
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using UserManage.API.Controllers;
using UserManage.Application.Interface.Service;
using UserManage.Domain.Common;
using UserManage.Domain.Constants;
using UserManage.Domain.Entities;
using Xunit;

namespace UserManage.Tests.ControllerTests;

/// <summary>
/// Tests unitarios para el RegionController.
/// </summary>
public class RegionControllerTests
{
    private readonly Mock<IRegionService> _mockService;
    private readonly Mock<ILogger<RegionController>> _mockLogger;
    private readonly RegionController _controller;

    public RegionControllerTests()
    {
        _mockService = new Mock<IRegionService>();
        _mockLogger = new Mock<ILogger<RegionController>>();
        _controller = new RegionController(_mockService.Object, _mockLogger.Object);
    }

    #region ObtenerPais Tests


    [Fact]
    public async Task ObtenerPais_CuandoNoHayPaises_RetornaOkConListaVacia()
    {
        // Arrange
        var paises = new List<Pais>();
        var result = Result<IEnumerable<Pais>>.SuccessResult(paises);
        _mockService.Setup(s => s.ObtenerPais()).ReturnsAsync(result);

        // Act
        var response = await _controller.ObtenerPais();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(response);
        Assert.Equal(200, okResult.StatusCode);
    }

    #endregion

    #region ObtenerDepartamento Tests


    [Fact]
    public async Task ObtenerDepartamento_CuandoPaisNoTieneDepartamentos_RetornaOkConListaVacia()
    {
        // Arrange
        var departamentos = new List<Departamento>();
        var result = Result<IEnumerable<Departamento>>.SuccessResult(departamentos);
        _mockService.Setup(s => s.ObtenerDepartamento(1)).ReturnsAsync(result);

        // Act
        var response = await _controller.ObtenerDepartamento(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(response);
        Assert.Equal(200, okResult.StatusCode);
    }


    #endregion

    #region ObtenerMunicipios Tests

    [Fact]
    public async Task ObtenerMunicipios_ConIdNegativo_RetornaBadRequest()
    {
        // Act
        var response = await _controller.ObtenerMunicipios(-5);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(response);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

     [Fact]
    public async Task ObtenerMunicipios_CuandoDepartamentoNoTieneMunicipios_RetornaOkConListaVacia()
    {
        // Arrange
        var municipios = new List<Municipio>();
        var result = Result<IEnumerable<Municipio>>.SuccessResult(municipios);
        _mockService.Setup(s => s.ObtenerMunicipio(1)).ReturnsAsync(result);

        // Act
        var response = await _controller.ObtenerMunicipios(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(response);
        Assert.Equal(200, okResult.StatusCode);
    }

    #endregion

    #region Integration Tests (Flujo completo)

    [Fact]
    public async Task FlujoCompleto_ObtenerPaisDepartamentoMunicipio_Exitoso()
    {
        // Arrange
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

        _mockService.Setup(s => s.ObtenerPais())
            .ReturnsAsync(Result<IEnumerable<Pais>>.SuccessResult(paises));
        _mockService.Setup(s => s.ObtenerDepartamento(1))
            .ReturnsAsync(Result<IEnumerable<Departamento>>.SuccessResult(departamentos));
        _mockService.Setup(s => s.ObtenerMunicipio(1))
            .ReturnsAsync(Result<IEnumerable<Municipio>>.SuccessResult(municipios));

        // Act
        var paisesResponse = await _controller.ObtenerPais();
        var deptosResponse = await _controller.ObtenerDepartamento(1);
        var municipiosResponse = await _controller.ObtenerMunicipios(1);

        // Assert
        Assert.IsType<OkObjectResult>(paisesResponse);
        Assert.IsType<OkObjectResult>(deptosResponse);
        Assert.IsType<OkObjectResult>(municipiosResponse);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-999)]
    public async Task ObtenerDepartamento_ConDiferentesIdsInvalidos_RetornaBadRequest(long paisId)
    {
        // Act
        var response = await _controller.ObtenerDepartamento(paisId);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(response);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task ObtenerMunicipios_ConDiferentesIdsInvalidos_RetornaBadRequest(long departamentoId)
    {
        // Act
        var response = await _controller.ObtenerMunicipios(departamentoId);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(response);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

    #endregion
}

