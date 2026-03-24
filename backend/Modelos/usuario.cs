using System;

namespace SistemaBancario.Models
{
    public class Usuario
    {
        public int      IdUsuario         { get; set; }
        public string   Nombre            { get; set; }
        public string   Apellido          { get; set; }
        public string   CorreoElectronico { get; set; }
        public decimal  SalarioMensual    { get; set; }
        public bool     EstadoUsuario     { get; set; }
        public DateTime FechaRegistro     { get; set; }
    }
}