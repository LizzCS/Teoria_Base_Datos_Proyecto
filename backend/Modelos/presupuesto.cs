using System;

namespace SistemaBancario.Models
{
    public class Presupuesto
    {
        public int     IdPresupuesto      { get; set; }
        public int     UsuarioId          { get; set; }
        public string  NombreDescriptivo  { get; set; }
        public string  Descripcion        { get; set; }
        public int     AnioInicio         { get; set; }
        public int     MesInicio          { get; set; }
        public int     AnioFin            { get; set; }
        public int     MesFin             { get; set; }
        public decimal TotalIngreso       { get; set; }
        public decimal TotalGasto         { get; set; }
        public decimal TotalAhorro        { get; set; }
        public int     EstadoPresupuesto  { get; set; }
    }
}