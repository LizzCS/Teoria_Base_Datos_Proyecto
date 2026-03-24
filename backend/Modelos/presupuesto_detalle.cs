using System;

namespace SistemaBancario.Models
{
    public class PresupuestoDetalle
    {
        public int     Id                    { get; set; }
        public int     IdPresupuesto         { get; set; }
        public string  NombrePresupuesto     { get; set; }
        public int     IdSubcategoria        { get; set; }
        public string  NombreSubcategoria    { get; set; }
        public string  NombreCategoria       { get; set; }
        public decimal MontoMensual          { get; set; }
        public string  ObservacionMonto      { get; set; }
    }
}
 