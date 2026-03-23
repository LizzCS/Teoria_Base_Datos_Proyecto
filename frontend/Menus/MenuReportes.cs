using SistemaBancario.Repositories;
using SistemaBancario.Models;
using SistemaBancario;

namespace SistemaBancario.Menus
{
    public class MenuReportes
    {
        public static async Task Mostrar()
        {
            Console.Clear();
            UI.Titulo("REPORTES");
            UI.Info("Generando todos los reportes automáticamente...");

            try
            {
                var presupuestos = PresupuestoRepo.Listar();
                var activo       = presupuestos.FirstOrDefault(p => p.EstadoPresupuesto == 1);

                if (activo == null)
                {
                    UI.Error("No tienes un presupuesto activo.");
                    UI.Pausa();
                    return;
                }

                Console.WriteLine($"  Presupuesto : {activo.NombreDescriptivo}");
                Console.WriteLine($"  Período     : {DateTime.Now.Month}/{DateTime.Now.Year}");
                Console.WriteLine();

                await Graficos.GenerarTodos();
            }
            catch (Exception ex) { UI.Error(ex.Message); }
            UI.Pausa();
        }
    }
}