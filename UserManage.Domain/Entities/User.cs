using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManage.Domain.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Pais { get; set; } = string.Empty;
        public string Departamento { get; set; } = string.Empty;
        public string Municipio { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
    }
}
