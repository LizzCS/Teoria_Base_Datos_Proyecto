// Menus/MenuTransacciones.cs
using System;
using SistemaBancario.Models;
using SistemaBancario.Repositories;

namespace SistemaBancario.Menus
{
    public static class MenuTransacciones
    {
        public static void Mostrar()
        {
            while (true)
            {
                Console.Clear();
                UI.Titulo("TRANSACCIONES");
                Console.WriteLine();
                Console.WriteLine("  1. Registrar nueva transacción");
                Console.WriteLine("  2. Listar transacciones");
                Console.WriteLine("  3. Consultar transacción");
                Console.WriteLine("  4. Editar transacción");
                Console.WriteLine("  5. Eliminar transacción");
                Console.WriteLine("  6. Ver monto ejecutado de subcategoría");
                Console.WriteLine("  7. Ver porcentaje de ejecución");
                Console.WriteLine("  8. Ver balance de subcategoría");
                Console.WriteLine("  9. Ver proyección del mes");
                Console.WriteLine("  10. Ver promedio histórico");
                Console.WriteLine("  0. Volver");

                switch (UI.LeerOpcion(10))
                {
                    case 1:  Registrar();        break;
                    case 2:  Listar();           break;
                    case 3:  Consultar();        break;
                    case 4:  Editar();           break;
                    case 5:  Eliminar();         break;
                    case 6:  VerEjecutado();     break;
                    case 7:  VerPorcentaje();    break;
                    case 8:  VerBalance();       break;
                    case 9:  VerProyeccion();    break;
                    case 10: VerPromedio();      break;
                    case 0:  return;
                }
            }
        }

        private static void Registrar()
        {
            Console.Clear();
            UI.Titulo("NUEVA TRANSACCIÓN");

            if (!int.TryParse(UI.Leer("ID del presupuesto"), out int idPres) ||
                !int.TryParse(UI.Leer("ID de subcategoría"), out int idSub))
            { UI.Error("ID inválido."); UI.Pausa(); return; }

            Console.WriteLine("  Tipo: 1) gasto  2) ingreso  3) ahorro");
            int tipoOp = UI.LeerOpcion(3);
            string tipo = tipoOp == 1 ? "gasto" : tipoOp == 2 ? "ingreso" : "ahorro";

            string desc = UI.Leer("Descripción");

            if (!decimal.TryParse(UI.Leer("Monto (L.)"), out decimal monto))
            { UI.Error("Monto inválido."); UI.Pausa(); return; }

            Console.WriteLine("  Método: 1) efectivo  2) tarjeta debito  3) tarjeta credito  4) transferencia");
            int metOp  = UI.LeerOpcion(4);
            string metodo = metOp switch { 1 => "efectivo", 2 => "tarjeta debito", 3 => "tarjeta credito", _ => "transferencia" };

            if (!int.TryParse(UI.Leer("Año"),        out int anio) ||
                !int.TryParse(UI.Leer("Mes (1-12)"), out int mes))
            { UI.Error("Fecha inválida."); UI.Pausa(); return; }

            DateTime fecha    = DateTime.Now;
            string fechaStr   = UI.Leer("Fecha (dd/mm/aaaa, Enter = hoy)");
            if (!string.IsNullOrEmpty(fechaStr))
                DateTime.TryParseExact(fechaStr, "dd/MM/yyyy",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out fecha);

            // Validar vigencia antes de insertar
            if (!TransaccionRepo.FnValidarVigencia(fecha, idPres))
            {
                UI.Error("La fecha no está dentro del período del presupuesto.");
                UI.Pausa();
                return;
            }

            try
            {
                TransaccionRepo.Insertar(idPres, idSub, tipo, desc, monto, fecha, metodo, anio, mes);
                UI.Ok("Transacción registrada.");
            }
            catch (Exception ex) { UI.Error(ex.Message); }
            UI.Pausa();
        }

        private static void Listar()
        {
            Console.Clear();
            UI.Titulo("LISTAR TRANSACCIONES");

            if (!int.TryParse(UI.Leer("ID del presupuesto"), out int idPres))
            { UI.Error("ID inválido."); UI.Pausa(); return; }

            string anioStr = UI.Leer("Año (Enter = todos)");
            string mesStr  = UI.Leer("Mes (Enter = todos)");
            string tipo    = UI.Leer("Tipo gasto/ingreso/ahorro (Enter = todos)");

            int? anio = string.IsNullOrEmpty(anioStr) ? null : int.Parse(anioStr);
            int? mes  = string.IsNullOrEmpty(mesStr)  ? null : int.Parse(mesStr);
            tipo      = string.IsNullOrEmpty(tipo)    ? null : tipo;

            try
            {
                var lista = TransaccionRepo.Listar(idPres, anio, mes, tipo);
                if (lista.Count == 0) { UI.Info("No hay transacciones."); UI.Pausa(); return; }

                int anchoDesc = Math.Max(11, lista.Max(t => (t.Descripcion ?? "").Length) + 2);

                UI.Separador();
                Console.WriteLine($"  {"ID",-6}  {"Descripción".PadRight(anchoDesc)}  {"Tipo",-10}  {"Monto",13}  {"Período",-8}  {"Fecha"}");
                UI.Separador();
                foreach (var t in lista)
                    Console.WriteLine($"  {t.IdTransaccion,-6}  {(t.Descripcion ?? "").PadRight(anchoDesc)}  {t.TipoTransaccion,-10}  L.{t.Monto,12:N2}  {t.MesTransaccion}/{t.AnioTransaccion,-4}  {t.FechaHoraRegistro:dd/MM/yyyy}");
            }
            catch (Exception ex) { UI.Error(ex.Message); }
            UI.Pausa();
        }

        private static void Consultar()
        {
            Console.Clear();
            UI.Titulo("CONSULTAR TRANSACCIÓN");
            if (!int.TryParse(UI.Leer("ID de la transacción"), out int id))
            { UI.Error("ID inválido."); UI.Pausa(); return; }

            try
            {
                TransaccionRepo.Consultar(id);
            }
            catch (Exception ex) { UI.Error(ex.Message); }
            UI.Pausa();
        }

        private static void Editar()
        {
            Console.Clear();
            UI.Titulo("EDITAR TRANSACCIÓN");

            if (!int.TryParse(UI.Leer("ID de la transacción"), out int id))
            { UI.Error("ID inválido."); UI.Pausa(); return; }

            if (!decimal.TryParse(UI.Leer("Nuevo monto (L.)"), out decimal monto))
            { UI.Error("Monto inválido."); UI.Pausa(); return; }

            Console.WriteLine("  Tipo: 1) gasto  2) ingreso  3) ahorro");
            int tipoOp = UI.LeerOpcion(3);
            string tipo = tipoOp == 1 ? "gasto" : tipoOp == 2 ? "ingreso" : "ahorro";

            Console.WriteLine("  Método: 1) efectivo  2) tarjeta debito  3) tarjeta credito  4) transferencia");
            int metOp  = UI.LeerOpcion(4);
            string metodo = metOp switch { 1 => "efectivo", 2 => "tarjeta debito", 3 => "tarjeta credito", _ => "transferencia" };

            string desc = UI.Leer("Nueva descripción");

            if (!int.TryParse(UI.Leer("Año"),        out int anio) ||
                !int.TryParse(UI.Leer("Mes (1-12)"), out int mes))
            { UI.Error("Fecha inválida."); UI.Pausa(); return; }

            DateTime fecha  = DateTime.Now;
            string fechaStr = UI.Leer("Fecha (dd/mm/aaaa, Enter = hoy)");
            if (!string.IsNullOrEmpty(fechaStr))
                DateTime.TryParseExact(fechaStr, "dd/MM/yyyy",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out fecha);

            try
            {
                TransaccionRepo.Actualizar(id, monto, metodo, tipo, fecha, desc, anio, mes);
                UI.Ok("Transacción actualizada.");
            }
            catch (Exception ex) { UI.Error(ex.Message); }
            UI.Pausa();
        }

        private static void Eliminar()
        {
            Console.Clear();
            UI.Titulo("ELIMINAR TRANSACCIÓN");
            if (!int.TryParse(UI.Leer("ID de la transacción"), out int id))
            { UI.Error("ID inválido."); UI.Pausa(); return; }

            Console.Write("  ¿Confirmas eliminar? (s/n): ");
            if (Console.ReadLine()?.ToLower() != "s") { UI.Info("Cancelado."); UI.Pausa(); return; }

            try
            {
                TransaccionRepo.Eliminar(id);
                UI.Ok("Transacción eliminada.");
            }
            catch (Exception ex) { UI.Error(ex.Message); }
            UI.Pausa();
        }

        private static void VerEjecutado()
        {
            Console.Clear();
            UI.Titulo("MONTO EJECUTADO");

            if (!int.TryParse(UI.Leer("ID de subcategoría"), out int idSub) ||
                !int.TryParse(UI.Leer("Año"),                out int anio)  ||
                !int.TryParse(UI.Leer("Mes (1-12)"),         out int mes))
            { UI.Error("Valores inválidos."); UI.Pausa(); return; }

            try
            {
                decimal ejecutado = TransaccionRepo.FnMontoEjecutado(idSub, anio, mes);
                UI.Separador();
                Console.WriteLine($"  Subcategoría  : {idSub}");
                Console.WriteLine($"  Período       : {mes}/{anio}");
                Console.WriteLine($"  Monto ejecutado: L. {ejecutado:N2}");
                UI.Separador();
            }
            catch (Exception ex) { UI.Error(ex.Message); }
            UI.Pausa();
        }

        private static void VerPorcentaje()
        {
            Console.Clear();
            UI.Titulo("PORCENTAJE DE EJECUCIÓN");

            if (!int.TryParse(UI.Leer("ID de subcategoría"), out int idSub)  ||
                !int.TryParse(UI.Leer("ID de presupuesto"),  out int idPres) ||
                !int.TryParse(UI.Leer("Año"),                out int anio)   ||
                !int.TryParse(UI.Leer("Mes (1-12)"),         out int mes))
            { UI.Error("Valores inválidos."); UI.Pausa(); return; }

            try
            {
                decimal pct = TransaccionRepo.FnPorcentajeEjecutado(idSub, idPres, anio, mes);
                string estado = pct < 80 ? "✓ Bajo presupuesto" : pct <= 100 ? "⚠ Cerca del límite" : "✗ Excedido";
                UI.Separador();
                Console.WriteLine($"  Período        : {mes}/{anio}");
                Console.WriteLine($"  % Ejecutado    : {pct:N1}%");
                Console.WriteLine($"  Estado         : {estado}");
                UI.Separador();
            }
            catch (Exception ex) { UI.Error(ex.Message); }
            UI.Pausa();
        }

        private static void VerBalance()
        {
            Console.Clear();
            UI.Titulo("BALANCE DE SUBCATEGORÍA");

            if (!int.TryParse(UI.Leer("ID de presupuesto"),  out int idPres) ||
                !int.TryParse(UI.Leer("ID de subcategoría"), out int idSub)  ||
                !int.TryParse(UI.Leer("Año"),                out int anio)   ||
                !int.TryParse(UI.Leer("Mes (1-12)"),         out int mes))
            { UI.Error("Valores inválidos."); UI.Pausa(); return; }

            try
            {
                decimal balance = TransaccionRepo.FnBalanceSubcategoria(idPres, idSub, anio, mes);
                string estado   = balance >= 0 ? "✓ Disponible" : "✗ Excedido";
                UI.Separador();
                Console.WriteLine($"  Período        : {mes}/{anio}");
                Console.WriteLine($"  Balance        : L. {balance:N2}");
                Console.WriteLine($"  Estado         : {estado}");
                UI.Separador();
            }
            catch (Exception ex) { UI.Error(ex.Message); }
            UI.Pausa();
        }

        private static void VerProyeccion()
        {
            Console.Clear();
            UI.Titulo("PROYECCIÓN DEL MES");

            if (!int.TryParse(UI.Leer("ID de subcategoría"), out int idSub) ||
                !int.TryParse(UI.Leer("Año"),                out int anio)  ||
                !int.TryParse(UI.Leer("Mes (1-12)"),         out int mes))
            { UI.Error("Valores inválidos."); UI.Pausa(); return; }

            try
            {
                decimal proyeccion = TransaccionRepo.FnProyeccionGasto(idSub, anio, mes);
                UI.Separador();
                Console.WriteLine($"  Período        : {mes}/{anio}");
                Console.WriteLine($"  Proyección     : L. {proyeccion:N2}");
                Console.WriteLine($"  (Basado en el ritmo de gasto actual)");
                UI.Separador();
            }
            catch (Exception ex) { UI.Error(ex.Message); }
            UI.Pausa();
        }

        private static void VerPromedio()
        {
            Console.Clear();
            UI.Titulo("PROMEDIO HISTORICO");

            if (!int.TryParse(UI.Leer("ID de subcategoria"),    out int idSub) ||
                !int.TryParse(UI.Leer("Cantidad de meses"),     out int meses))
            { UI.Error("Valores inválidos."); UI.Pausa(); return; }

            try
            {
                decimal promedio = TransaccionRepo.FnPromedioGasto(idSub, meses);
                UI.Separador();
                Console.WriteLine($"  Subcategoría   : {idSub}");
                Console.WriteLine($"  Últimos        : {meses} meses");
                Console.WriteLine($"  Promedio       : L. {promedio:N2}/mes");
                UI.Separador();
            }
            catch (Exception ex) { UI.Error(ex.Message); }
            UI.Pausa();
        }
    }
}