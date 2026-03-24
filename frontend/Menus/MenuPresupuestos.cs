using System.Text.Json;
using SistemaBancario.Repositories;

namespace SistemaBancario.Menus
{
    public static class MenuPresupuestos
    {
        public static void Mostrar()
        {
            while (true)
            {
                Console.Clear();
                UI.Titulo("PRESUPUESTOS");
                Console.WriteLine();
                Console.WriteLine("  1. Listar presupuestos");
                Console.WriteLine("  2. Nuevo presupuesto");
                Console.WriteLine("  3. Editar presupuesto");
                Console.WriteLine("  4. Eliminar presupuesto");
                Console.WriteLine("  5. Consultar presupuesto");
                Console.WriteLine("  6. Ver detalles");
                Console.WriteLine("  7. Agregar detalle");
                Console.WriteLine("  8. Editar detalle");
                Console.WriteLine("  9. Eliminar detalle");
                Console.WriteLine("  10. Cerrar presupuesto");
                Console.WriteLine("  0. Volver");

                switch (UI.LeerOpcion(10))
                {
                    case 1:  Listar();          break;
                    case 2:  Nuevo();           break;
                    case 3:  Editar();          break;
                    case 4:  Eliminar();        break;
                    case 5:  Consultar();       break;
                    case 6:  VerDetalle();      break;
                    case 7:  AgregarDetalle();  break;
                    case 8:  EditarDetalle();   break;
                    case 9:  EliminarDetalle(); break;
                    case 10: Cerrar();          break;
                    case 0:  return;
                }
            }
        }

        private static void Listar()
        {
            Console.Clear();
            UI.Titulo("LISTA DE PRESUPUESTOS");
            try
            {
                var lista = PresupuestoRepo.Listar();
                if (lista.Count == 0) { UI.Info("No tienes presupuestos aún."); UI.Pausa(); return; }

                int anchoNombre = Math.Max(6, lista.Max(p => p.NombreDescriptivo.Length) + 2);

                UI.Separador();
                Console.WriteLine($"  {"ID",-5}  {"Nombre".PadRight(anchoNombre)}  {"Período",-20}  {"Estado"}");
                UI.Separador();
                foreach (var p in lista)
                {
                    string estado  = p.EstadoPresupuesto == 1 ? "Activo"
                                   : p.EstadoPresupuesto == 2 ? "Cerrado" : "Borrador";
                    string periodo = $"{p.MesInicio}/{p.AnioInicio} — {p.MesFin}/{p.AnioFin}";
                    Console.WriteLine($"  {p.IdPresupuesto,-5}  {p.NombreDescriptivo.PadRight(anchoNombre)}  {periodo,-20}  {estado}");
                }
            }
            catch (Exception ex) { UI.Error(ex.Message); }
            UI.Pausa();
        }

        private static void Nuevo()
        {
            Console.Clear();
            UI.Titulo("NUEVO PRESUPUESTO");

            string nombre = UI.Leer("Nombre descriptivo");
            string desc   = UI.Leer("Descripción");

            if (!int.TryParse(UI.Leer("Mes inicio (1-12)"), out int mesI)  ||
                !int.TryParse(UI.Leer("Año inicio"),        out int anioI) ||
                !int.TryParse(UI.Leer("Mes fin (1-12)"),    out int mesF)  ||
                !int.TryParse(UI.Leer("Año fin"),           out int anioF))
            { UI.Error("Valores inválidos."); UI.Pausa(); return; }

            // Mostrar subcategorías disponibles
            Console.WriteLine();
            UI.Info("Subcategorías disponibles:");
            try
            {
                var categorias = CategoriaRepo.Listar("gasto")
                    .Concat(CategoriaRepo.Listar("ingreso"))
                    .Concat(CategoriaRepo.Listar("ahorro"));

                UI.Separador();
                Console.WriteLine($"  {"ID",-5}  {"Subcategoría",-25}  {"Categoría",-20}  {"Tipo"}");
                UI.Separador();
                foreach (var cat in categorias)
                {
                    var subs = CategoriaRepo.ListarSubcategorias(cat.IdCategoria);
                    foreach (var s in subs.Where(s => s.EsActivo))
                        Console.WriteLine($"  {s.IdSubcategoria,-5}  {s.Nombre,-25}  {cat.Nombre,-20}  {cat.TipoCategoria}");
                }
                UI.Separador();
            }
            catch (Exception ex) { UI.Error(ex.Message); UI.Pausa(); return; }

            // Recopilar subcategorías y montos
            var subcategorias = new List<object>();
            while (true)
            {
                if (!int.TryParse(UI.Leer("ID subcategoría (0 para terminar)"), out int idSub) || idSub == 0) break;
                if (!decimal.TryParse(UI.Leer("Monto mensual (L.)"), out decimal monto)) { UI.Error("Monto inválido."); continue; }
                subcategorias.Add(new { id_subcategoria = idSub, monto_mensual = monto });
                UI.Ok($"  Subcategoría {idSub} agregada — L. {monto:N2}");
            }

            if (subcategorias.Count == 0) { UI.Error("Debe agregar al menos una subcategoría."); UI.Pausa(); return; }

            string json = JsonSerializer.Serialize(subcategorias);

            try
            {
                PresupuestoRepo.CrearCompleto(nombre, desc, anioI, anioF, mesI, mesF, json);
                UI.Ok("Presupuesto creado con todos sus detalles.");
            }
            catch (Exception ex) { UI.Error(ex.Message); }
            UI.Pausa();
        }

        private static void Editar()
        {
            Console.Clear();
            UI.Titulo("EDITAR PRESUPUESTO");
            if (!int.TryParse(UI.Leer("ID del presupuesto"), out int id))
            { UI.Error("ID inválido."); UI.Pausa(); return; }

            string nombre = UI.Leer("Nuevo nombre");
            string desc   = UI.Leer("Nueva descripción");

            if (!int.TryParse(UI.Leer("Mes inicio (1-12)"), out int mesI)  ||
                !int.TryParse(UI.Leer("Año inicio"),        out int anioI) ||
                !int.TryParse(UI.Leer("Mes fin (1-12)"),    out int mesF)  ||
                !int.TryParse(UI.Leer("Año fin"),           out int anioF))
            { UI.Error("Valores inválidos."); UI.Pausa(); return; }

            try
            {
                PresupuestoRepo.Actualizar(id, nombre, desc, mesI, anioI, mesF, anioF);
                UI.Ok("Presupuesto actualizado.");
            }
            catch (Exception ex) { UI.Error(ex.Message); }
            UI.Pausa();
        }

        private static void Eliminar()
        {
            Console.Clear();
            UI.Titulo("ELIMINAR PRESUPUESTO");
            if (!int.TryParse(UI.Leer("ID del presupuesto"), out int id))
            { UI.Error("ID inválido."); UI.Pausa(); return; }

            Console.Write("  ¿Confirmas eliminar? (s/n): ");
            if (Console.ReadLine()?.ToLower() != "s") { UI.Info("Cancelado."); UI.Pausa(); return; }

            try
            {
                PresupuestoRepo.Eliminar(id);
                UI.Ok("Presupuesto eliminado.");
            }
            catch (Exception ex) { UI.Error(ex.Message); }
            UI.Pausa();
        }

        private static void Consultar()
        {
            Console.Clear();
            UI.Titulo("CONSULTAR PRESUPUESTO");
            if (!int.TryParse(UI.Leer("ID del presupuesto"), out int id))
            { UI.Error("ID inválido."); UI.Pausa(); return; }

            try
            {
                var p = PresupuestoRepo.Consultar(id);
                if (p == null) { UI.Error("No existe."); UI.Pausa(); return; }

                UI.Separador();
                Console.WriteLine($"  ID          : {p.IdPresupuesto}");
                Console.WriteLine($"  Nombre      : {p.NombreDescriptivo}");
                Console.WriteLine($"  Descripción : {p.Descripcion}");
                Console.WriteLine($"  Período     : {p.MesInicio}/{p.AnioInicio} — {p.MesFin}/{p.AnioFin}");
                Console.WriteLine($"  Estado      : {(p.EstadoPresupuesto == 1 ? "Activo" : p.EstadoPresupuesto == 2 ? "Cerrado" : "Borrador")}");
                UI.Separador();
            }
            catch (Exception ex) { UI.Error(ex.Message); }
            UI.Pausa();
        }

        private static void VerDetalle()
        {
            Console.Clear();
            UI.Titulo("VER DETALLES");
            if (!int.TryParse(UI.Leer("ID del presupuesto"), out int id))
            { UI.Error("ID inválido."); UI.Pausa(); return; }

            try
            {
                var detalles = PresupuestoRepo.ListarDetalles(id);
                if (detalles.Count == 0) { UI.Info("Este presupuesto no tiene detalles."); UI.Pausa(); return; }

                int anchoNombre = Math.Max(12, detalles.Max(d => d.NombreSubcategoria.Length) + 2);

                UI.Separador();
                Console.WriteLine($"  {"ID",-5}  {"Subcategoría".PadRight(anchoNombre)}  {"Monto mensual",14}  {"Observaciones"}");
                UI.Separador();
                foreach (var d in detalles)
                    Console.WriteLine($"  {d.Id,-5}  {d.NombreSubcategoria.PadRight(anchoNombre)}  L. {d.MontoMensual,12:N2}  {d.ObservacionMonto}");
                UI.Separador();

                // Balance mes actual
                int mes  = DateTime.Now.Month;
                int anio = DateTime.Now.Year;
                var (ing, gas, aho, bal) = PresupuestoRepo.CalcularBalance(id, anio, mes);
                Console.WriteLine();
                UI.Info($"Balance {mes}/{anio}:");
                Console.WriteLine($"  Ingresos : L. {ing:N2}");
                Console.WriteLine($"  Gastos   : L. {gas:N2}");
                Console.WriteLine($"  Ahorros  : L. {aho:N2}");
                Console.WriteLine($"  Balance  : L. {bal:N2}");
            }
            catch (Exception ex) { UI.Error(ex.Message); }
            UI.Pausa();
        }

        private static void AgregarDetalle()
        {
            Console.Clear();
            UI.Titulo("AGREGAR DETALLE");

            if (!int.TryParse(UI.Leer("ID del presupuesto"),    out int idPres) ||
                !int.TryParse(UI.Leer("ID de subcategoría"),    out int idSub)  ||
                !decimal.TryParse(UI.Leer("Monto mensual (L.)"), out decimal monto))
            { UI.Error("Valores inválidos."); UI.Pausa(); return; }

            string obs = UI.Leer("Observaciones (opcional)");

            try
            {
                PresupuestoRepo.InsertarDetalle(idPres, idSub, monto, obs);
                UI.Ok("Detalle agregado.");
            }
            catch (Exception ex) { UI.Error(ex.Message); }
            UI.Pausa();
        }

        private static void EditarDetalle()
        {
            Console.Clear();
            UI.Titulo("EDITAR DETALLE");

            if (!int.TryParse(UI.Leer("ID del detalle"), out int idDetalle) ||
                !decimal.TryParse(UI.Leer("Nuevo monto mensual (L.)"), out decimal monto))
            { UI.Error("Valores inválidos."); UI.Pausa(); return; }

            string obs = UI.Leer("Observaciones (opcional)");

            try
            {
                PresupuestoRepo.ActualizarDetalle(idDetalle, monto, obs);
                UI.Ok("Detalle actualizado.");
            }
            catch (Exception ex) { UI.Error(ex.Message); }
            UI.Pausa();
        }

        private static void EliminarDetalle()
        {
            Console.Clear();
            UI.Titulo("ELIMINAR DETALLE");
            if (!int.TryParse(UI.Leer("ID del detalle"), out int idDetalle))
            { UI.Error("ID inválido."); UI.Pausa(); return; }

            Console.Write("  ¿Confirmas eliminar? (s/n): ");
            if (Console.ReadLine()?.ToLower() != "s") { UI.Info("Cancelado."); UI.Pausa(); return; }

            try
            {
                PresupuestoRepo.EliminarDetalle(idDetalle);
                UI.Ok("Detalle eliminado.");
            }
            catch (Exception ex) { UI.Error(ex.Message); }
            UI.Pausa();
        }

        private static void Cerrar()
        {
            Console.Clear();
            UI.Titulo("CERRAR PRESUPUESTO");
            if (!int.TryParse(UI.Leer("ID del presupuesto"), out int id))
            { UI.Error("ID inválido."); UI.Pausa(); return; }

            Console.Write("  ¿Confirmas cerrar? (s/n): ");
            if (Console.ReadLine()?.ToLower() != "s") { UI.Info("Cancelado."); UI.Pausa(); return; }

            try
            {
                PresupuestoRepo.Cerrar(id);
                UI.Ok("Presupuesto cerrado.");
            }
            catch (Exception ex) { UI.Error(ex.Message); }
            UI.Pausa();
        }
    }
}