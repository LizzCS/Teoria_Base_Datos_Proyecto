using System;
using SistemaBancario.Models;
using SistemaBancario.Repositories;


namespace SistemaBancario.Menus
{
    public static class MenuObligaciones
    {
        public static void Mostrar()
        {
            while (true)
            {
                Console.Clear();
                UI.Titulo("OBLIGACIONES FIJAS");
                Console.WriteLine();
                Console.WriteLine("  1. Listar obligaciones");
                Console.WriteLine("  2. Nueva obligación");
                Console.WriteLine("  3. Editar obligación");
                Console.WriteLine("  4. Consultar obligación");
                Console.WriteLine("  5. Desactivar obligación");
                Console.WriteLine("  6. Procesar obligaciones del mes");
                Console.WriteLine("  0. Volver");

                switch (UI.LeerOpcion(6))
                {
                    case 1: Listar();     break;
                    case 2: Nueva();      break;
                    case 3: Editar();     break;
                    case 4: Consultar();  break;
                    case 5: Desactivar(); break;
                    case 6: ProcesarMes(); break;
                    case 0: return;
                }
            }
        }

        private static void Listar()
        {
            Console.Clear();
            UI.Titulo("LISTA DE OBLIGACIONES");
            try
            {
                var lista = ObligacionRepo.Listar(true);
                if (lista.Count == 0) { UI.Info("No hay obligaciones activas."); UI.Pausa(); return; }

                int anchoNombre = Math.Max(6,  lista.Max(o => o.Nombre.Length) + 2);
                int anchoSub    = Math.Max(12, lista.Max(o => o.NombreSubcategoria?.Length ?? 0) + 2);

                UI.Separador();
                Console.WriteLine($"  {"ID",-5}  {"Nombre".PadRight(anchoNombre)}  {"Subcategoría".PadRight(anchoSub)}  {"Monto",14}  {"Día",3}  {"Días rest.",10}  {"Estado"}");
                UI.Separador();

                foreach (var o in lista)
                {
                    int    dias   = (int)(o.FechaFinalizacion - DateTime.Today).TotalDays;
                    string alerta = dias < 0 ? "Vencida"
                                    : dias < 3 ? "Por vencer"
                                    : "Vigente";

                    Console.WriteLine($"  {o.Id,-5}  {o.Nombre.PadRight(anchoNombre)}  {(o.NombreSubcategoria ?? "").PadRight(anchoSub)}  L.{o.MontoFijoMensual,13:N2}  {o.DiaDelMes,3}  {(dias + " días").PadLeft(10)}  {alerta}");
                }
            }
            catch (Exception ex) { UI.Error(ex.Message); }
            UI.Pausa();
        }
        private static void Nueva()
        {
            Console.Clear();
            UI.Titulo("NUEVA OBLIGACIÓN");

            if (!int.TryParse(UI.Leer("ID de subcategoría"), out int idSub))
            { UI.Error("ID inválido."); UI.Pausa(); return; }

            string nombre = UI.Leer("Nombre");
            string desc   = UI.Leer("Descripción");

            if (!decimal.TryParse(UI.Leer("Monto mensual (L.)"), out decimal monto) ||
                !int.TryParse(UI.Leer("Día de vencimiento (1-31)"), out int dia))
            { UI.Error("Valores inválidos."); UI.Pausa(); return; }

            if (!DateTime.TryParseExact(UI.Leer("Fecha inicio (dd/mm/aaaa)"), "dd/MM/yyyy",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out DateTime fechaIni) ||
                !DateTime.TryParseExact(UI.Leer("Fecha fin (dd/mm/aaaa)"), "dd/MM/yyyy",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out DateTime fechaFin))
            { UI.Error("Fechas inválidas."); UI.Pausa(); return; }

            try
            {
                ObligacionRepo.Insertar(idSub, nombre, desc, monto, dia, fechaIni, fechaFin);
                UI.Ok("Obligación creada.");
            }
            catch (Exception ex) { UI.Error(ex.Message); }
            UI.Pausa();
        }

        private static void Editar()
        {
            Console.Clear();
            UI.Titulo("EDITAR OBLIGACIÓN");
            if (!int.TryParse(UI.Leer("ID de la obligación"), out int id))
            { UI.Error("ID inválido."); UI.Pausa(); return; }

            string nombre = UI.Leer("Nombre");
            string desc   = UI.Leer("Descripción");

            if (!decimal.TryParse(UI.Leer("Monto mensual (L.)"), out decimal monto) ||
                !int.TryParse(UI.Leer("Día de vencimiento (1-31)"), out int dia))
            { UI.Error("Valores inválidos."); UI.Pausa(); return; }

            if (!DateTime.TryParseExact(UI.Leer("Fecha fin (dd/mm/aaaa)"), "dd/MM/yyyy",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out DateTime fechaFin))
            { UI.Error("Fecha inválida."); UI.Pausa(); return; }

            Console.Write("  ¿Está vigente? (s/n): ");
            bool vigente = Console.ReadLine()?.ToLower() == "s";

            try
            {
                ObligacionRepo.Actualizar(id, nombre, desc, monto, dia, fechaFin, vigente);
                UI.Ok("Obligación actualizada.");
            }
            catch (Exception ex) { UI.Error(ex.Message); }
            UI.Pausa();
        }

        private static void Consultar()
        {
            Console.Clear();
            UI.Titulo("CONSULTAR OBLIGACIÓN");
            if (!int.TryParse(UI.Leer("ID de la obligación"), out int id))
            { UI.Error("ID inválido."); UI.Pausa(); return; }

            try
            {
                ObligacionRepo.Consultar(id);
            }
            catch (Exception ex) { UI.Error(ex.Message); }
            UI.Pausa();
        }

        private static void Desactivar()
        {
            Console.Clear();
            UI.Titulo("DESACTIVAR OBLIGACIÓN");
            if (!int.TryParse(UI.Leer("ID de la obligación"), out int id))
            { UI.Error("ID inválido."); UI.Pausa(); return; }

            Console.Write("  ¿Confirmas desactivar? (s/n): ");
            if (Console.ReadLine()?.ToLower() != "s") { UI.Info("Cancelado."); UI.Pausa(); return; }

            try
            {
                ObligacionRepo.Eliminar(id);
                UI.Ok("Obligación desactivada.");
            }
            catch (Exception ex) { UI.Error(ex.Message); }
            UI.Pausa();
        }

        private static void ProcesarMes()
        {
            Console.Clear();
            UI.Titulo("PROCESAR OBLIGACIONES DEL MES");
            if (!int.TryParse(UI.Leer("Año"),           out int anio)) { UI.Error("Inválido."); UI.Pausa(); return; }
            if (!int.TryParse(UI.Leer("Mes"),            out int mes))  { UI.Error("Inválido."); UI.Pausa(); return; }
            if (!int.TryParse(UI.Leer("ID Presupuesto"), out int idP))  { UI.Error("Inválido."); UI.Pausa(); return; }

            try
            {
                ObligacionRepo.ProcesarObligacionesMes(anio, mes, idP);
            }
            catch (Exception ex) { UI.Error(ex.Message); }
            UI.Pausa();
        }
    }
}