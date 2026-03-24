using System;
using SistemaBancario.Models;
using SistemaBancario.Repositories;

namespace SistemaBancario.Menus
{
    public static class MenuUsuarios
    {
        public static void Mostrar()
        {
            while (true)
            {
                Console.Clear();
                UI.Titulo("USUARIOS");
                Console.WriteLine();
                Console.WriteLine("  1. Listar usuarios");
                Console.WriteLine("  2. Editar mi perfil");
                Console.WriteLine("  3. Desactivar usuario");
                Console.WriteLine("  0. Volver");

                switch (UI.LeerOpcion(3))
                {
                    case 1: Listar();   break;
                    case 2: EditarPerfil(); break;
                    case 3: Desactivar();   break;
                    case 0: return;
                }
            }
        }

        private static void Listar()
        {
            Console.Clear();
            UI.Titulo("LISTA DE USUARIOS");
            try
            {
                var lista = UsuarioRepo.Listar();
                UI.Separador();
                Console.WriteLine($"  {"ID",-5} {"Nombre",-25} {"Correo",-30} {"Salario",12} {"Estado"}");
                UI.Separador();
                foreach (var u in lista)
                    Console.WriteLine($"  {u.IdUsuario,-5} {u.Nombre + " " + u.Apellido,-25} {u.CorreoElectronico,-30} L.{u.SalarioMensual,11:N2}  {(u.EstadoUsuario ? "Activo" : "Inactivo")}");
            }
            catch (Exception ex) { UI.Error(ex.Message); }
            UI.Pausa();
        }

        private static void EditarPerfil()
        {
            Console.Clear();
            UI.Titulo("EDITAR MI PERFIL");
            UI.Info($"Usuario actual: {Session.Nombre} {Session.Apellido}");
            Console.WriteLine();

            string nombre   = UI.Leer("Nuevo nombre (Enter = sin cambio)");
            string apellido = UI.Leer("Nuevo apellido (Enter = sin cambio)");
            string salStr   = UI.Leer("Nuevo salario (Enter = sin cambio)");

            if (string.IsNullOrEmpty(nombre))   nombre   = Session.Nombre;
            if (string.IsNullOrEmpty(apellido)) apellido = Session.Apellido;

            if (!decimal.TryParse(salStr, out decimal salario) && !string.IsNullOrEmpty(salStr))
            { UI.Error("Salario inválido."); UI.Pausa(); return; }

            try
            {
                UsuarioRepo.Actualizar(Session.IdUsuario, nombre, apellido,
                    string.IsNullOrEmpty(salStr) ? 0 : salario);
                Session.Nombre   = nombre;
                Session.Apellido = apellido;
                UI.Ok("Perfil actualizado.");
            }
            catch (Exception ex) { UI.Error(ex.Message); }
            UI.Pausa();
        }

        private static void Desactivar()
        {
            Console.Clear();
            UI.Titulo("DESACTIVAR USUARIO");
            if (!int.TryParse(UI.Leer("ID del usuario"), out int id))
            { UI.Error("ID inválido."); UI.Pausa(); return; }

            if (id == Session.IdUsuario)
            { UI.Error("No puedes desactivarte a ti mismo."); UI.Pausa(); return; }

            Console.Write("  ¿Confirmas desactivar? (s/n): ");
            if (Console.ReadLine()?.ToLower() != "s") { UI.Info("Cancelado."); UI.Pausa(); return; }

            try
            {
                UsuarioRepo.Eliminar(id);
                UI.Ok("Usuario desactivado.");
            }
            catch (Exception ex) { UI.Error(ex.Message); }
            UI.Pausa();
        }
    }
}
