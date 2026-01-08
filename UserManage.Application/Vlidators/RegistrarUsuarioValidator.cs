using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManage.Domain.Dtos;

namespace UserManage.Application.Vlidators
{
    public static class RegistrarUsuarioValidator
    {
        public static List<string> Validate(ReqUsuarioDto dto)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.Nombre))
                errors.Add("El nombre es requerido");
            else if (dto.Nombre.Length < 3 || dto.Nombre.Length > 200)
                errors.Add("El nombre debe tener entre 3 y 200 caracteres");

            if (string.IsNullOrWhiteSpace(dto.Telefono))
                errors.Add("El teléfono es requerido");
            else if (!System.Text.RegularExpressions.Regex.IsMatch(dto.Telefono, @"^[0-9+\-\s()]{7,20}$"))
                errors.Add("El formato del teléfono no es válido");

            if (dto.PaisId <= 0)
                errors.Add("Debe especificar un país válido");

            if (dto.DepartamentoId <= 0)
                errors.Add("Debe especificar un departamento válido");

            if (dto.MunicipioId <= 0)
                errors.Add("Debe especificar un municipio válido");

            if (string.IsNullOrWhiteSpace(dto.Direccion))
                errors.Add("La dirección es requerida");
            else if (dto.Direccion.Length < 5 || dto.Direccion.Length > 500)
                errors.Add("La dirección debe tener entre 5 y 500 caracteres");

            return errors;
        }
    }
}
