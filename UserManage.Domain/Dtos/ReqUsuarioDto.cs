using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManage.Domain.Dtos
{
    public class ReqUsuarioDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public long PaisId { get; set; }
        public long DepartamentoId { get; set; }
        public long MunicipioId { get; set; }
        public string Direccion { get; set; } = string.Empty;
    }
}
