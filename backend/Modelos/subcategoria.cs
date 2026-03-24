using System;

namespace SistemaBancario.Models
{
    public class Subcategoria
    {
        public int    IdSubcategoria          { get; set; }
        public int    IdCategoria             { get; set; }
        public string NombreCategoria         { get; set; }
        public string Nombre                  { get; set; }
        public string Descripcion             { get; set; }
        public bool   EsActivo                { get; set; }
        public bool   SubcategoriaPorDefecto  { get; set; }
    }
}
 