namespace UserManage.Domain.Dtos
{
    public class UsuarioResponseDto(long id, string nombre, string telefono, string pais, string departamento, string municipio, string direccion, DateTime fechaCreacion)
    {
        public long Id { get; set; } = id;
        public string Nombre { get; set; } = nombre;
        public string Telefono { get; set; } = telefono;
        public string Pais { get; set; } = pais;
        public string Departamento { get; set; } = departamento;
        public string Municipio { get; set; } = municipio;
        public string Direccion { get; set; } = direccion;
        public DateTime FechaCreacion { get; set; } = fechaCreacion;
    }
}
