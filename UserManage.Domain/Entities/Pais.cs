namespace UserManage.Domain.Entities
{
    public class Pais
    {
        public long Id { get; set; }
        public string? Nombre { get; set; }
        public string? Codigo { get; set; }
        public bool Estado { get; set; }
        public DateTime Fecha_Creacion { get; set; }
    }
}
