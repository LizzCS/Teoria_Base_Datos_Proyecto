
using System.Data;
using Microsoft.Data.SqlClient;
using SistemaBancario.Database;
using SistemaBancario.Models;
using SistemaBancario.Menus;

namespace SistemaBancario.Repositories
{
    public class ObligacionRepo
    {
        public static void Insertar(int idSubcategoria, string nombre, string descripcion,
            decimal monto, int diaVencimiento, DateTime fechaInicio, DateTime fechaFin)
        {
            using var conn = Conexion.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("sp_insertar_obligacion", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@p_id_subcategoria", idSubcategoria);
            cmd.Parameters.AddWithValue("@p_nombre", nombre);
            cmd.Parameters.AddWithValue("@p_descripcion", descripcion);
            cmd.Parameters.AddWithValue("@p_monto", monto);
            cmd.Parameters.AddWithValue("@p_dia_vencimiento", diaVencimiento);
            cmd.Parameters.AddWithValue("@p_fecha_inicio", fechaInicio);
            cmd.Parameters.AddWithValue("@p_fecha_fin", fechaFin);
            cmd.Parameters.AddWithValue("@p_creado_por", Session.IdUsuario);
            cmd.ExecuteNonQuery();
        }

        public static void Actualizar(int id, string nombre, string descripcion,
            decimal monto, int diaVencimiento, DateTime fechaFin, bool esVigente)
        {
            using var conn = Conexion.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("sp_actualizar_obligacion", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@p_id_obligacion", id);
            cmd.Parameters.AddWithValue("@p_nombre", nombre);
            cmd.Parameters.AddWithValue("@p_descripcion", descripcion);
            cmd.Parameters.AddWithValue("@p_monto", monto);
            cmd.Parameters.AddWithValue("@p_dia_vencimiento", diaVencimiento);
            cmd.Parameters.AddWithValue("@p_fecha_fin", fechaFin);
            cmd.Parameters.AddWithValue("@p_es_vigente", esVigente);
            cmd.Parameters.AddWithValue("@p_modificado_por", Session.IdUsuario);
            cmd.ExecuteNonQuery();
        }

        public static void Eliminar(int id)
        {
            using var conn = Conexion.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("sp_eliminar_obligacion", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@p_id_obligacion", id);
            cmd.ExecuteNonQuery();
        }

        public static void Consultar(int id)
        {
            using var conn = Conexion.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("sp_consultar_obligacion", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@p_id_obligacion", id);
            using var r = cmd.ExecuteReader();
            if (!r.Read()) { Console.WriteLine("  No encontrada."); return; }

            Console.WriteLine($"  ID            : {r.GetInt32(0)}");
            Console.WriteLine($"  Nombre        : {r.GetString(1)}");
            Console.WriteLine($"  Monto mensual : L. {r.GetDecimal(2):N2}");
            Console.WriteLine($"  Día del mes   : {r.GetByte(3)}");
            Console.WriteLine($"  Fecha inicio  : {r.GetDateTime(4):dd/MM/yyyy}");
            Console.WriteLine($"  Fecha fin     : {r.GetDateTime(5):dd/MM/yyyy}");
            Console.WriteLine($"  Estado        : {(r.GetBoolean(6) ? "Activo" : "Inactivo")}");
            Console.WriteLine($"  Subcategoría  : {r.GetString(12)}");
            Console.WriteLine($"  Categoría     : {r.GetString(14)}");
        }

        public static List<ObligacionFija> Listar(bool? vigente = null)
        {
            var list = new List<ObligacionFija>();
            using var conn = Conexion.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("sp_listar_obligaciones_usuario", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@p_id_usuario", Session.IdUsuario);
            cmd.Parameters.AddWithValue("@p_es_vigente", (object)vigente ?? DBNull.Value);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                list.Add(new ObligacionFija
                    {
                    Id = r.GetInt32(0),
                    Nombre = r.GetString(1),
                    Descripcion = r.IsDBNull(2) ? "" : r.GetString(2),
                    MontoFijoMensual = r.GetDecimal(3),
                    DiaDelMes = r.GetByte(4),
                    FechaInicio  = r.GetDateTime(5),
                    FechaFinalizacion = r.GetDateTime(6),
                    EstaVigente = r.GetString(7) == "Activo",
                    NombreSubcategoria = r.GetString(9)
                    });
            return list;
        }

        public static void ProcesarObligacionesMes(int anio, int mes, int idPresupuesto)
        {
            using var conn = Conexion.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("sp_procesar_obligaciones_mes", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@p_id_usuario", Session.IdUsuario);
            cmd.Parameters.AddWithValue("@p_anio", anio);
            cmd.Parameters.AddWithValue("@p_mes", mes);
            cmd.Parameters.AddWithValue("@p_id_presupuesto", idPresupuesto);
            using var r = cmd.ExecuteReader();

            UI.Separador();
            Console.WriteLine($"  {"ID",-5} {"Nombre",-25} {"Monto",12} {"Día",4} {"Alerta"}");
            UI.Separador();
            while (r.Read())
                Console.WriteLine($"  {r.GetInt32(0),-5} {r.GetString(1),-25} L.{r.GetDecimal(2),11:N2}  {r.GetByte(3),3}  {r.GetString(5)}");
            UI.Separador();
        }
    }
}
