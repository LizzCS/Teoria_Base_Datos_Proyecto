using System;

namespace SistemaBancario.Models
{
    public class Categoria
    {
        public int    IdCategoria   { get; set; }
        public string Nombre        { get; set; }
        public string Descripcion   { get; set; }
        public string TipoCategoria { get; set; }
    }
}