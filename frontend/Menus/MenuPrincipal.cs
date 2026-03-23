namespace SistemaBancario.Menus
{
    public static class MenuPrincipal
    {
        public static async Task Mostrar()
        {
            while (true)
            {
                Console.Clear();
                UI.Titulo($"MENÚ PRINCIPAL — {Session.Nombre} {Session.Apellido}");
                Console.WriteLine();
                Console.WriteLine("  1. Presupuestos");
                Console.WriteLine("  2. Transacciones");
                Console.WriteLine("  3. Obligaciones fijas");
                Console.WriteLine("  4. Categorías");
                Console.WriteLine("  5. Usuarios");
                Console.WriteLine("  6. Reportes");
                Console.WriteLine("  0. Cerrar sesión");

                switch (UI.LeerOpcion(6))
                {
                    case 1: MenuPresupuestos.Mostrar();  break;
                    case 2: MenuTransacciones.Mostrar(); break;
                    case 3: MenuObligaciones.Mostrar();  break;
                    case 4: MenuCategorias.Mostrar();    break;
                    case 5: MenuUsuarios.Mostrar();      break;
                    case 6: await MenuReportes.Mostrar();      break;
                    case 0:
                        Session.Cerrar();
                        return;
                }
            }
        }
    }
}
