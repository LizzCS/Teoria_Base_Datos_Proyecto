namespace SistemaBancario
{
    public static class Session
    {
        public static int    IdUsuario  { get; set; }
        public static string Nombre     { get; set; }
        public static string Apellido   { get; set; }
        public static string Correo     { get; set; }
 
        public static bool EstaLogueado => IdUsuario != 0;
 
        public static void Cerrar()
        {
            IdUsuario = 0;
            Nombre    = null;
            Apellido  = null;
            Correo    = null;
        }
    }
}
 