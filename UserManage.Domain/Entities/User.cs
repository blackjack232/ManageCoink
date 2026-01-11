namespace UserManage.Domain.Entities
{
    public class Usuario
    {
        public long Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Pais { get; set; } = string.Empty;
        public string Departamento { get; set; } = string.Empty;
        public string Municipio { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
    }
}
