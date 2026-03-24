using System.Data;
using Microsoft.Data.SqlClient;
using SistemaBancario.Database;
using SistemaBancario.Models;
using SistemaBancario.Menus;

namespace SistemaBancario.Repositories
{
    public class TransaccionRepo
    {
        public static void Insertar(int idPresupuesto, int idSubcategoria,
            string tipo, string descripcion, decimal monto, DateTime fecha,
            string metodoPago, int anio, int mes)
        {
            using var conn = Conexion.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("sp_registrar_transaccion_completa", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@p_id_usuario",      Session.IdUsuario);
            cmd.Parameters.AddWithValue("@p_id_presupuesto",  idPresupuesto);
            cmd.Parameters.AddWithValue("@p_anio",            anio);
            cmd.Parameters.AddWithValue("@p_mes",             mes);
            cmd.Parameters.AddWithValue("@p_id_subcategoria", idSubcategoria);
            cmd.Parameters.AddWithValue("@p_tipo",            tipo);
            cmd.Parameters.AddWithValue("@p_descripcion",     descripcion);
            cmd.Parameters.AddWithValue("@p_monto",           monto);
            cmd.Parameters.AddWithValue("@p_fecha",           fecha);
            cmd.Parameters.AddWithValue("@p_metodo_pago",     metodoPago);
            cmd.Parameters.AddWithValue("@p_creado_por",      Session.IdUsuario);
            cmd.ExecuteNonQuery();
        }

        public static void Actualizar(int id, decimal monto, string metodoPago,
            string tipo, DateTime fecha, string descripcion, int anio, int mes)
        {
            using var conn = Conexion.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("sp_actualizar_transaccion", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@p_id_transaccion",  id);
            cmd.Parameters.AddWithValue("@p_monto",           monto);
            cmd.Parameters.AddWithValue("@p_metodo_pago",     metodoPago);
            cmd.Parameters.AddWithValue("@p_tipo_transaccion",tipo);
            cmd.Parameters.AddWithValue("@p_fecha",           fecha);
            cmd.Parameters.AddWithValue("@p_descripcion",     descripcion);
            cmd.Parameters.AddWithValue("@p_anio",            anio);
            cmd.Parameters.AddWithValue("@p_mes",             mes);
            cmd.Parameters.AddWithValue("@p_modificado_por",  Session.IdUsuario);
            cmd.ExecuteNonQuery();
        }

        public static void Eliminar(int id)
        {
            using var conn = Conexion.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("sp_eliminar_transaccion", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@p_id_transaccion", id);
            cmd.ExecuteNonQuery();
        }

        public static void Consultar(int id)
        {
            using var conn = Conexion.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("sp_consultar_transaccion", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@p_id_transaccion", id);
            using var r = cmd.ExecuteReader();
            if (!r.Read()) { Console.WriteLine("  No encontrada."); return; }

            UI.Separador();
            Console.WriteLine($"  ID            : {r.GetInt32(0)}");
            Console.WriteLine($"  Monto         : L. {r.GetDecimal(2):N2}");
            Console.WriteLine($"  Tipo          : {r.GetString(3)}");
            Console.WriteLine($"  Método pago   : {r.GetString(4)}");
            Console.WriteLine($"  Fecha         : {r.GetDateTime(5):dd/MM/yyyy HH:mm}");
            Console.WriteLine($"  Descripción   : {r.GetString(6)}");
            Console.WriteLine($"  Período       : {r.GetInt16(7)}/{r.GetByte(8)}");
            UI.Separador();
        }

        public static List<Transaccion> Listar(int idPresupuesto,
            int? anio = null, int? mes = null, string tipo = null)
        {
            var list = new List<Transaccion>();
            using var conn = Conexion.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("sp_listar_transacciones_presupuesto", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@p_id_presupuesto", idPresupuesto);
            cmd.Parameters.AddWithValue("@p_anio", (object)anio ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@p_mes",  (object)mes  ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@p_tipo", (object)tipo ?? DBNull.Value);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                list.Add(new Transaccion
                {
                    IdTransaccion        = r.GetInt32(0),
                    Monto                = r.GetDecimal(1),
                    TipoTransaccion      = r.GetString(2),
                    FechaHoraRegistro    = r.GetDateTime(3),
                    MetodoPago           = r.GetString(4),
                    AnioTransaccion      = r.GetInt16(5),
                    MesTransaccion       = r.GetByte(6),
                    IdPresupuestoDetalle = r.GetInt32(7)
                });
            return list;
        }

        public static decimal FnMontoEjecutado(int idSubcategoria, int anio, int mes)
        {
            using var conn = Conexion.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(
                "SELECT dbo.fn_calcular_monto_ejecutado(@s, @a, @m)", conn);
            cmd.Parameters.AddWithValue("@s", idSubcategoria);
            cmd.Parameters.AddWithValue("@a", anio);
            cmd.Parameters.AddWithValue("@m", mes);
            var r = cmd.ExecuteScalar();
            return r == DBNull.Value || r == null ? 0 : Convert.ToDecimal(r);
        }

        public static decimal FnPorcentajeEjecutado(int idSubcategoria, int idPresupuesto, int anio, int mes)
        {
            using var conn = Conexion.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(
                "SELECT dbo.fn_calcular_porcentaje_ejecutado(@s, @a, @m, @p)", conn);
            cmd.Parameters.AddWithValue("@s", idSubcategoria);
            cmd.Parameters.AddWithValue("@a", anio);
            cmd.Parameters.AddWithValue("@m", mes);
            cmd.Parameters.AddWithValue("@p", idPresupuesto);
            var r = cmd.ExecuteScalar();
            return r == DBNull.Value || r == null ? 0 : Convert.ToDecimal(r);
        }

        public static decimal FnBalanceSubcategoria(int idPresupuesto, int idSubcategoria, int anio, int mes)
        {
            using var conn = Conexion.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(
                "SELECT dbo.fn_obtener_balance_subcategoria(@p, @s, @a, @m)", conn);
            cmd.Parameters.AddWithValue("@p", idPresupuesto);
            cmd.Parameters.AddWithValue("@s", idSubcategoria);
            cmd.Parameters.AddWithValue("@a", anio);
            cmd.Parameters.AddWithValue("@m", mes);
            var r = cmd.ExecuteScalar();
            return r == DBNull.Value || r == null ? 0 : Convert.ToDecimal(r);
        }

        public static decimal FnProyeccionGasto(int idSubcategoria, int anio, int mes)
        {
            using var conn = Conexion.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(
                "SELECT dbo.fn_calcular_proyeccion_gasto_mensual(@s, @a, @m)", conn);
            cmd.Parameters.AddWithValue("@s", idSubcategoria);
            cmd.Parameters.AddWithValue("@a", anio);
            cmd.Parameters.AddWithValue("@m", mes);
            var r = cmd.ExecuteScalar();
            return r == DBNull.Value || r == null ? 0 : Convert.ToDecimal(r);
        }

        public static decimal FnPromedioGasto(int idSubcategoria, int cantidadMeses)
        {
            using var conn = Conexion.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(
                "SELECT dbo.fn_obtener_promedio_gasto_subcategoria(@u, @s, @n)", conn);
            cmd.Parameters.AddWithValue("@u", Session.IdUsuario);
            cmd.Parameters.AddWithValue("@s", idSubcategoria);
            cmd.Parameters.AddWithValue("@n", cantidadMeses);
            var r = cmd.ExecuteScalar();
            return r == DBNull.Value || r == null ? 0 : Convert.ToDecimal(r);
        }

        public static bool FnValidarVigencia(DateTime fecha, int idPresupuesto)
        {
            using var conn = Conexion.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(
                "SELECT dbo.fn_validar_vigencia_presupuesto(@f, @p)", conn);
            cmd.Parameters.AddWithValue("@f", fecha.Date);
            cmd.Parameters.AddWithValue("@p", idPresupuesto);
            var r = cmd.ExecuteScalar();
            return r != DBNull.Value && r != null && Convert.ToBoolean(r);
        }
    }
}