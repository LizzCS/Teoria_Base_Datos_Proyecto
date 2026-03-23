
using System.Data;
using Microsoft.Data.SqlClient;
using SistemaBancario.Database;
using SistemaBancario.Models;

namespace SistemaBancario.Repositories
{
    public class ReportesRepo
    {
        // Reporte 1: Ingresos vs Gastos vs Ahorros
        public static async Task Reporte1(int idPresupuesto, int anio, int mes)
        {
            // Implementation for Reporte1
        }

        // Reporte 3: Cumplimiento de Presupuesto
        public static async Task Reporte3(int idPresupuesto, int anio, int mes)
        {
            // Implementation for Reporte3
        }
    }
}