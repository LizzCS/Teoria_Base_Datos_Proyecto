// Menus/UI.cs  — shared console helpers
namespace SistemaBancario.Menus
{
    public static class UI
    {
        public static void Titulo(string texto)
        {
            Console.WriteLine();
            Console.WriteLine("----------------------------------------");
            Console.WriteLine($"|  {texto,-36}|");
            Console.WriteLine("-----------------------------------------");
        }

        public static void Separador() =>
            Console.WriteLine("──────────────────────────────────────────");

        public static void Ok(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n ✓ " + msg);
            Console.ResetColor();
        }

        public static void Error(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n # " + msg);
            Console.ResetColor();
        }

        public static void Info(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("  " + msg);
            Console.ResetColor();
        }

        public static string Leer(string label)
        {
            Console.Write($"  {label}: ");
            return Console.ReadLine()?.Trim() ?? "";
        }

        public static string LeerPassword(string label)
        {
            Console.Write($"  {label}: ");
            string pass = "";
            ConsoleKeyInfo key;
            while ((key = Console.ReadKey(true)).Key != ConsoleKey.Enter)
            {
                if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
                {
                    pass = pass[..^1];
                    Console.Write("\b \b");
                }
                else if (key.Key != ConsoleKey.Backspace)
                {
                    pass += key.KeyChar;
                    Console.Write("*");
                }
            }
            Console.WriteLine();
            return pass;
        }

        public static int LeerOpcion(int max)
        {
            while (true)
            {
                Console.Write("\n  Opción: ");
                if (int.TryParse(Console.ReadLine(), out int op) && op >= 0 && op <= max)
                    return op;
                Error("Opción inválida. Intenta de nuevo.");
            }
        }

        public static void Pausa()
        {
            Console.WriteLine("\n  Presiona Enter para continuar...");
            Console.ReadLine();
        }
    }
}
