using System;

namespace SistemaBancario.Models
{
    public class Transaccion
    {
        public int      IdTransaccion         { get; set; }
        public int      IdPresupuestoDetalle  { get; set; }
        public decimal  Monto                 { get; set; }
        public string   MetodoPago            { get; set; }
        public string   TipoTransaccion       { get; set; }
        public DateTime FechaHoraRegistro     { get; set; }
        public string   Descripcion           { get; set; }
        public int      AnioTransaccion       { get; set; }
        public int      MesTransaccion        { get; set; }
    }
}