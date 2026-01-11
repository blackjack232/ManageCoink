namespace UserManage.Domain.Entities
{
    public class Municipio
    {
        public long Id { get; set; }
        public string? Nombre { get; set; }
        public long Departamento_Id { get; set; }
        public bool Estado {get;set;}

        public TimeSpan Fecha_Creacion { get; set; }

    }
}
