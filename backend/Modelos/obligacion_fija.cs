using System;


namespace SistemaBancario.Models
{
    public class ObligacionFija
    {
        public int      Id                  { get; set; }
        public int      IdSubcategoria      { get; set; }
        public string   NombreSubcategoria  { get; set; }
        public string   Nombre              { get; set; }
        public string   Descripcion         { get; set; }
        public decimal  MontoFijoMensual    { get; set; }
        public int      DiaDelMes           { get; set; }
        public bool     EstaVigente         { get; set; }
        public DateTime FechaInicio         { get; set; }
        public DateTime FechaFinalizacion   { get; set; }
    }
}