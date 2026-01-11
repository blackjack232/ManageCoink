namespace UserManage.Domain.Entities
{
    public class Departamento
    {
        public long Id { get; set; }
        public string? Nombre { get; set; }
        public long Pais_id { get; set; }
        public bool Estado { get; set; }
        public TimeSpan Fecha_Creacion { get; set; }
    }
}
