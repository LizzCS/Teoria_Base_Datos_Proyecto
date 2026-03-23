using SistemaBancario.Repositories;

namespace SistemaBancario.Menus
{
    public static class MenuLogin
    {
        public static void Mostrar()
        {
            while (true)
            {
                Console.Clear();
                UI.Titulo("SISTEMA BANCARIO");
                Console.WriteLine();
                Console.WriteLine("  1. Iniciar sesión");
                Console.WriteLine("  2. Crear cuenta");
                Console.WriteLine("  0. Salir");

                int op = UI.LeerOpcion(2);

                switch (op)
                {
                    case 1: Login();    break;
                    case 2: Registro(); break;
                    case 0: Environment.Exit(0); break;
                }
            }
        }

        private static async Task Login()
        {
            Console.Clear();
            UI.Titulo("INICIAR SESIÓN");
            string correo = UI.Leer("Correo");
            string pass   = UI.LeerPassword("Contraseña");

            try
            {
                bool ok = UsuarioRepo.Login(correo, pass);
                if (ok)
                {
                    UI.Ok($"Bienvenido, {Session.Nombre} {Session.Apellido}");
                    UI.Pausa();
                    await MenuPrincipal.Mostrar();
                }
                else
                {
                    UI.Error("Correo o contraseña incorrectos.");
                    UI.Pausa();
                }
            }
            catch (Exception ex)
            {
                UI.Error("Error de conexión: " + ex.Message);
                UI.Pausa();
            }
        }

        private static void Registro()
        {
            Console.Clear();
            UI.Titulo("CREAR CUENTA");
            string nombre   = UI.Leer("Nombre");
            string apellido = UI.Leer("Apellido");
            string correo   = UI.Leer("Correo electrónico");
            string pass     = UI.LeerPassword("Contraseña");

            if (!decimal.TryParse(UI.Leer("Salario mensual (L.)"), out decimal salario) || salario < 0)
            {
                UI.Error("Salario inválido.");
                UI.Pausa();
                return;
            }

            try
            {
                UsuarioRepo.Insertar(nombre, apellido, correo, pass, salario);
                UI.Ok("Cuenta creada exitosamente. Inicia sesión.");
            }
            catch (Exception ex)
            {
                UI.Error(ex.Message);
            }
            UI.Pausa();
        }
    }
}
