using System;
using SistemaBancario.Models;
using SistemaBancario.Repositories;

namespace SistemaBancario.Menus
{
    public static class MenuCategorias
    {
        public static void Mostrar()
        {
            while (true)
            {
                Console.Clear();
                UI.Titulo("CATEGORIAS");
                Console.WriteLine();
                Console.WriteLine("  1. Listar categorias");
                Console.WriteLine("  2. Nueva categoria");
                Console.WriteLine("  3. Editar categoria");
                Console.WriteLine("  4. Eliminar categoria");
                Console.WriteLine("  5. Consultar categoria");
                Console.WriteLine("  6. Ver subcategorias");
                Console.WriteLine("  7. Nueva subcategoria");
                Console.WriteLine("  8. Editar subcategoria");
                Console.WriteLine("  9. Eliminar subcategoria");
                Console.WriteLine("  10. Consultar subcategoria");
                Console.WriteLine("  0. Volver");

                switch (UI.LeerOpcion(9))
                {
                    case 1:
                    Listar();              
                    break;
                    case 2:
                     Nueva();               
                    break;
                    case 3: 
                    Editar();              
                    break;
                    case 4: 
                    Eliminar();            
                    break;
                    case 5: 
                    Consultar();           
                    break;
                    case 6: 
                    VerSubcategorias();    
                    break;
                    case 7: 
                    NuevaSubcategoria();   
                    break;
                    case 8: 
                    EditarSubcategoria();  
                    break;
                    case 9: 
                    EliminarSubcategoria();
                    break;
                    case 10:
                    ConsultarSubcategoria();
                    break;
                    case 0: return;
                }
            }
        }

        private static void Listar()
        {
            Console.Clear();
            UI.Titulo("LISTA DE CATEGORÍAS");
            try
            {
                Console.WriteLine("  Tipo: 1) Gasto  2) Ingreso  3) Ahorro");
                int op   = UI.LeerOpcion(3);
                string tipo = op == 1 ? "gasto" : op == 2 ? "ingreso" : "ahorro";

                var lista = CategoriaRepo.Listar(tipo);
                if (lista.Count == 0) { UI.Info("No hay categorías."); UI.Pausa(); return; }

                int anchoNombre = Math.Max(6, lista.Max(c => c.Nombre.Length) + 2);

                UI.Separador();
                Console.WriteLine($"  {"ID",-5}  {"Nombre".PadRight(anchoNombre)}  {"Tipo"}");
                UI.Separador();
                foreach (var c in lista)
                    Console.WriteLine($"  {c.IdCategoria,-5}  {c.Nombre.PadRight(anchoNombre)}  {c.TipoCategoria}");
            }
            catch (Exception ex) { UI.Error(ex.Message); }
            UI.Pausa();
        }

        private static void Nueva()
        {
            Console.Clear();
            UI.Titulo("NUEVA CATEGORÍA");
            string nombre = UI.Leer("Nombre");
            string desc   = UI.Leer("Descripción");
            Console.WriteLine("  Tipo: 1) Gasto  2) Ingreso  3) Ahorro");
            int op   = UI.LeerOpcion(3);
            string tipo = op == 1 ? "gasto" : op == 2 ? "ingreso" : "ahorro";

            try
            {
                CategoriaRepo.Insertar(nombre, desc, tipo);
                UI.Ok("Categoría creada. (Subcategoría 'General' creada automáticamente)");
            }
            catch (Exception ex) { UI.Error(ex.Message); }
            UI.Pausa();
        }

        private static void Editar()
        {
            Console.Clear();
            UI.Titulo("EDITAR CATEGORÍA");
            if (!int.TryParse(UI.Leer("ID de la categoría"), out int id))
            { UI.Error("ID inválido."); UI.Pausa(); return; }

            string nombre = UI.Leer("Nuevo nombre");
            string desc   = UI.Leer("Nueva descripción");

            try
            {
                CategoriaRepo.Actualizar(id, nombre, desc);
                UI.Ok("Categoría actualizada.");
            }
            catch (Exception ex) { UI.Error(ex.Message); }
            UI.Pausa();
        }

        private static void Eliminar()
        {
            Console.Clear();
            UI.Titulo("ELIMINAR CATEGORÍA");
            if (!int.TryParse(UI.Leer("ID de la categoría"), out int id))
            { UI.Error("ID inválido."); UI.Pausa(); return; }

            Console.Write("  ¿Confirmas eliminar? (s/n): ");
            if (Console.ReadLine()?.ToLower() != "s") { UI.Info("Cancelado."); UI.Pausa(); return; }

            try
            {
                CategoriaRepo.Eliminar(id);
                UI.Ok("Categoría eliminada.");
            }
            catch (Exception ex) { UI.Error(ex.Message); }
            UI.Pausa();
        }

        private static void Consultar()
        {
            Console.Clear();
            UI.Titulo("CONSULTAR CATEGORÍA");
            if (!int.TryParse(UI.Leer("ID de la categoría"), out int id))
            { UI.Error("ID inválido."); UI.Pausa(); return; }

            try
            {
                var c = CategoriaRepo.Consultar(id);
                if (c == null) { UI.Error("No existe."); UI.Pausa(); return; }

                UI.Separador();
                Console.WriteLine($"  ID          : {c.IdCategoria}");
                Console.WriteLine($"  Nombre      : {c.Nombre}");
                Console.WriteLine($"  Descripción : {c.Descripcion}");
                Console.WriteLine($"  Tipo        : {c.TipoCategoria}");
                UI.Separador();
            }
            catch (Exception ex) { UI.Error(ex.Message); }
            UI.Pausa();
        }

        private static void VerSubcategorias()
        {
            Console.Clear();
            UI.Titulo("SUBCATEGORÍAS");
            if (!int.TryParse(UI.Leer("ID de la categoría"), out int id))
            { UI.Error("ID inválido."); UI.Pausa(); return; }

            try
            {
                var lista = CategoriaRepo.ListarSubcategorias(id);
                if (lista.Count == 0) { UI.Info("No hay subcategorías."); UI.Pausa(); return; }

                int anchoNombre = Math.Max(6,  lista.Max(s => s.Nombre.Length) + 2);
                int descWidth   = Math.Max(12, lista.Max(s => (s.Descripcion ?? "").Length) + 2);

                UI.Separador();
                Console.WriteLine($"  {"ID",-5}  {"Nombre".PadRight(anchoNombre)}  {"Descripción".PadRight(descWidth)}  {"S. Defecto",-12}  {"Activo"}");
                UI.Separador();
                foreach (var s in lista)
                    Console.WriteLine($"  {s.IdSubcategoria,-5}  {s.Nombre.PadRight(anchoNombre)}  {(s.Descripcion ?? "").PadRight(descWidth)}  {(s.SubcategoriaPorDefecto ? "Sí" : "No"),-12}  {(s.EsActivo ? "Sí" : "No")}");
            }
            catch (Exception ex) { UI.Error(ex.Message); }
            UI.Pausa();
        }

        private static void NuevaSubcategoria()
        {
            Console.Clear();
            UI.Titulo("NUEVA SUBCATEGORÍA");
            if (!int.TryParse(UI.Leer("ID de la categoría"), out int idCat))
            { UI.Error("ID inválido."); UI.Pausa(); return; }

            string nombre = UI.Leer("Nombre");
            string desc   = UI.Leer("Descripción");
            Console.Write("  ¿Es subcategoría por defecto? (s/n): ");
            bool esDefecto = Console.ReadLine()?.ToLower() == "s";

            try
            {
                CategoriaRepo.InsertarSubcategoria(idCat, nombre, desc, esDefecto);
                UI.Ok("Subcategoría creada.");
            }
            catch (Exception ex) { UI.Error(ex.Message); }
            UI.Pausa();
        }

        private static void EditarSubcategoria()
        {
            Console.Clear();
            UI.Titulo("EDITAR SUBCATEGORÍA");
            if (!int.TryParse(UI.Leer("ID de la subcategoría"), out int id))
            { UI.Error("ID inválido."); UI.Pausa(); return; }

            string nombre = UI.Leer("Nuevo nombre");
            string desc   = UI.Leer("Nueva descripción");

            try
            {
                CategoriaRepo.ActualizarSubcategoria(id, nombre, desc);
                UI.Ok("Subcategoría actualizada.");
            }
            catch (Exception ex) { UI.Error(ex.Message); }
            UI.Pausa();
        }

        private static void EliminarSubcategoria()
        {
            Console.Clear();
            UI.Titulo("ELIMINAR SUBCATEGORÍA");
            if (!int.TryParse(UI.Leer("ID de la subcategoría"), out int id))
            { UI.Error("ID inválido."); UI.Pausa(); return; }

            Console.Write("  ¿Confirmas eliminar? (s/n): ");
            if (Console.ReadLine()?.ToLower() != "s") { UI.Info("Cancelado."); UI.Pausa(); return; }

            try
            {
                CategoriaRepo.EliminarSubcategoria(id);
                UI.Ok("Subcategoría eliminada.");
            }
            catch (Exception ex) { UI.Error(ex.Message); }
            UI.Pausa();
        }

        private static void ConsultarSubcategoria()
        {
            Console.Clear();
            UI.Titulo("CONSULTAR SUBCATEGORÍA");
            if (!int.TryParse(UI.Leer("ID de la subcategoría"), out int id))
            { UI.Error("ID inválido."); UI.Pausa(); return; }

            try
            {
                CategoriaRepo.ConsultarSubcategoria(id);
            }
            catch (Exception ex) { UI.Error(ex.Message); }
            UI.Pausa();
        }
    }
}