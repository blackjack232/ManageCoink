using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace UserManage.Domain.Dtos;

/// <summary>
/// DTO para la solicitud de creación/actualización de usuario.
/// Implementa IValidatableObject para validaciones personalizadas.
/// </summary>
[ExcludeFromCodeCoverage]
public class ReqUsuarioDto : IValidatableObject
{
    /// <summary>
    /// Nombre completo del usuario.
    /// </summary>
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres.")]
    [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El nombre solo puede contener letras y espacios.")]
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Número de teléfono del usuario.
    /// </summary>
    [Required(ErrorMessage = "El telefono es obligatorio.")]
    [Phone(ErrorMessage = "El formato del telefono no es valido.")]
    [StringLength(20, MinimumLength = 10, ErrorMessage = "El telefono debe tener entre 10 y 20 caracteres.")]
    [RegularExpression(@"^\+?[0-9\s\-\(\)]+$", ErrorMessage = "El teléfono solo puede contener numeros, espacios, guiones y parentesis.")]
    public string Telefono { get; set; } = string.Empty;

    /// <summary>
    /// Identificador del país.
    /// </summary>
    [Required(ErrorMessage = "El Id del pais es obligatorio.")]
    [Range(1, long.MaxValue, ErrorMessage = "El PaisId debe ser mayor a 0.")]
    public long PaisId { get; set; }

    /// <summary>
    /// Identificador del departamento.
    /// </summary>
    [Required(ErrorMessage = "El Id del departamento es obligatorio.")]
    [Range(1, long.MaxValue, ErrorMessage = "El DepartamentoId debe ser mayor a 0.")]
    public long DepartamentoId { get; set; }

    /// <summary>
    /// Identificador del municipio.
    /// </summary>
    [Required(ErrorMessage = "El Id del municipio  es obligatorio.")]
    [Range(1, long.MaxValue, ErrorMessage = "El MunicipioId debe ser mayor a 0.")]
    public long MunicipioId { get; set; }

    /// <summary>
    /// Dirección del usuario.
    /// </summary>
    [Required(ErrorMessage = "La direccion es obligatoria.")]
    [StringLength(100, MinimumLength = 10, ErrorMessage = "La direccion debe tener entre 10 y 100 caracteres.")]
    public string Direccion { get; set; } = string.Empty;

    /// <summary>
    /// Realiza validaciones personalizadas del DTO.
    /// Valida relaciones entre campos y reglas de negocio adicionales.
    /// </summary>
    /// <param name="validationContext">Contexto de validación</param>
    /// <returns>Colección de resultados de validación</returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var errors = new List<ValidationResult>();
        if (!string.IsNullOrWhiteSpace(Nombre) && string.IsNullOrWhiteSpace(Nombre.Trim()))
        {
            errors.Add(new ValidationResult(
                "El nombre no puede contener solo espacios en blanco.",
                [nameof(Nombre)]
            ));
        }
        if (!string.IsNullOrWhiteSpace(Nombre))
        {
            var palabras = Nombre.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (palabras.Length < 2)
            {
                errors.Add(new ValidationResult(
                    "Debe proporcionar al menos nombre y apellido.",
                    [nameof(Nombre)]
                ));
            }
        }
        if (!string.IsNullOrWhiteSpace(Telefono))
        {
            var telefonoLimpio = Regex.Replace(Telefono, @"[\s\-\(\)\+]", "");
            if (telefonoLimpio.Length < 8)
            {
                errors.Add(new ValidationResult(
                    "El teléfono debe contener al menos 8 dígitos.",
                    [nameof(Telefono)]
                ));
            }
        }
        if (!string.IsNullOrWhiteSpace(Direccion) && string.IsNullOrWhiteSpace(Direccion.Trim()))
        {
            errors.Add(new ValidationResult(
                "La direccion no puede contener solo espacios en blanco.",
                [nameof(Direccion)]
            ));
        }
        if (DepartamentoId > 0 && MunicipioId > 0)
        {
            if (PaisId <= 0)
            {
                errors.Add(new ValidationResult(
                    "Debe proporcionar un PaisId valido cuando especifica Departamento y Municipio.",
                    [nameof(PaisId)]
                ));
            }
        }
        if ((PaisId > 0 && DepartamentoId == 0) ||
            (DepartamentoId > 0 && MunicipioId == 0))
        {
            errors.Add(new ValidationResult(
                "Debe proporcionar la ubicación completa: Pais, Departamento y Municipio.",
                [nameof(PaisId), nameof(DepartamentoId), nameof(MunicipioId)]
            ));
        }

        return errors;
    }
}