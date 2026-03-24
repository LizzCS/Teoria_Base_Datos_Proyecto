using System.Data;
using Microsoft.Data.SqlClient;
using SistemaBancario.Database;
using SistemaBancario.Models;

namespace SistemaBancario.Repositories
{
    public class PresupuestoRepo
    {
        public static void CrearCompleto(string nombre, string descripcion,
            int anioInicio, int anioFin, int mesInicio, int mesFin,
            string listaSubcategoriasJson)
        {
            using var conn = Conexion.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("sp_crear_presupuesto_completo", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@p_id_usuario", Session.IdUsuario);
            cmd.Parameters.AddWithValue("@p_nombre", nombre);
            cmd.Parameters.AddWithValue("@p_descripcion", descripcion);
            cmd.Parameters.AddWithValue("@p_anio_inicio", anioInicio);
            cmd.Parameters.AddWithValue("@p_anio_fin", anioFin);
            cmd.Parameters.AddWithValue("@p_mes_inicio", mesInicio);
            cmd.Parameters.AddWithValue("@p_mes_fin", mesFin);
            cmd.Parameters.AddWithValue("@p_lista_subcategoria_json", listaSubcategoriasJson);
            cmd.Parameters.AddWithValue("@p_creado_por", Session.IdUsuario);
            cmd.ExecuteNonQuery();
        }

        public static void Actualizar(int id, string nombre, string descripcion,
            int mesInicio, int anioInicio, int mesFin, int anioFin)
        {
            using var conn = Conexion.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("sp_actualizar_presupuesto", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@p_id_presupuesto", id);
            cmd.Parameters.AddWithValue("@p_nombre", nombre);
            cmd.Parameters.AddWithValue("@p_descripcion", descripcion);
            cmd.Parameters.AddWithValue("@p_anio_inicio", anioInicio);
            cmd.Parameters.AddWithValue("@p_anio_fin", anioFin);
            cmd.Parameters.AddWithValue("@p_mes_inicio", mesInicio);
            cmd.Parameters.AddWithValue("@p_mes_fin", mesFin);
            cmd.Parameters.AddWithValue("@p_modificado_por", Session.IdUsuario);
            cmd.ExecuteNonQuery();
        }

        public static void Eliminar(int id)
        {
            using var conn = Conexion.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("sp_eliminar_presupuesto", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@p_id_presupuesto", id);
            cmd.ExecuteNonQuery();
        }

        public static Presupuesto? Consultar(int id)
        {
            using var conn = Conexion.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("sp_consultar_presupuesto", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@p_id_presupuesto", id);
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;
            return new Presupuesto
            {
                IdPresupuesto = r.GetInt32(0),
                NombreDescriptivo = r.GetString(2),
                Descripcion = r.IsDBNull(3) ? "" : r.GetString(3),
                AnioInicio = r.GetInt16(4),
                MesInicio = r.GetByte(5),
                AnioFin = r.GetInt16(6),
                MesFin = r.GetByte(7)
            };
        }

        public static List<Presupuesto> Listar()
        {
            var list = new List<Presupuesto>();
            using var conn = Conexion.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("sp_listar_presupuestos_usuario_por_estado", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@p_id_usuario", Session.IdUsuario);
            cmd.Parameters.AddWithValue("@p_estado",     DBNull.Value);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                list.Add(new Presupuesto
                {
                    IdPresupuesto = r.GetInt32(0),
                    NombreDescriptivo = r.GetString(1),
                    Descripcion = r.IsDBNull(2) ? "" : r.GetString(2),
                    AnioInicio = r.GetInt16(3),
                    MesInicio = r.GetByte(4),
                    AnioFin = r.GetInt16(5),
                    MesFin = r.GetByte(6),
                    EstadoPresupuesto = r.IsDBNull(7) ? 0 : Convert.ToInt32(r.GetValue(7))
                });
            return list;
        }

        public static void Cerrar(int id)
        {
            using var conn = Conexion.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("sp_cerrar_presupuesto", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@p_id_presupuesto",  id);
            cmd.Parameters.AddWithValue("@p_mpodificado_por", Session.IdUsuario);
            cmd.ExecuteNonQuery();
        }

        public static (decimal ingresos, decimal gastos, decimal ahorros, decimal balance)
            CalcularBalance(int idPresupuesto, int anio, int mes)
        {
            using var conn = Conexion.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("sp_calcular_balance_mensual", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@p_id_usuario", Session.IdUsuario);
            cmd.Parameters.AddWithValue("@p_id_presupuesto", idPresupuesto);
            cmd.Parameters.AddWithValue("@p_anio", anio);
            cmd.Parameters.AddWithValue("@p_mes", mes);

            var pI = cmd.Parameters.Add("@p_total_ingresos", SqlDbType.Decimal); pI.Direction = ParameterDirection.Output;
            var pG = cmd.Parameters.Add("@p_total_gastos", SqlDbType.Decimal); pG.Direction = ParameterDirection.Output;
            var pA = cmd.Parameters.Add("@p_total_ahorros", SqlDbType.Decimal); pA.Direction = ParameterDirection.Output;
            var pB = cmd.Parameters.Add("@p_balance_final", SqlDbType.Decimal); pB.Direction = ParameterDirection.Output;

            cmd.ExecuteNonQuery();

            return (
                pI.Value == DBNull.Value ? 0 : (decimal)pI.Value,
                pG.Value == DBNull.Value ? 0 : (decimal)pG.Value,
                pA.Value == DBNull.Value ? 0 : (decimal)pA.Value,
                pB.Value == DBNull.Value ? 0 : (decimal)pB.Value
            );
        }

        public static decimal CalcularMontoEjecutado(int idSubcategoria, int idPresupuesto, int anio, int mes)
        {
            using var conn = Conexion.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("sp_calcular_monto_ejecutado", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@p_id_subcategoria", idSubcategoria);
            cmd.Parameters.AddWithValue("@p_id_presupuesto", idPresupuesto);
            cmd.Parameters.AddWithValue("@p_anio", anio);
            cmd.Parameters.AddWithValue("@p_mes", mes);

            var p = cmd.Parameters.Add("@p_monto_ejecutado", SqlDbType.Decimal);
            p.Direction = ParameterDirection.Output;
            cmd.ExecuteNonQuery();

            return p.Value == DBNull.Value ? 0 : (decimal)p.Value;
        }

        public static decimal CalcularPorcentaje(int idSubcategoria, int idPresupuesto, int anio, int mes)
        {
            using var conn = Conexion.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("sp_calcular_porcentaje_ejecucion_mes", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@p_id_subcategoria", idSubcategoria);
            cmd.Parameters.AddWithValue("@p_id_presupuesto",  idPresupuesto);
            cmd.Parameters.AddWithValue("@p_anio", anio);
            cmd.Parameters.AddWithValue("@p_mes", mes);

            var p = cmd.Parameters.Add("@p_porcentaje_ejecutado", SqlDbType.Decimal);
            p.Direction = ParameterDirection.Output;
            cmd.ExecuteNonQuery();

            return p.Value == DBNull.Value ? 0 : (decimal)p.Value;
        }

        public static (decimal presupuestado, decimal ejecutado, decimal porcentaje)
            ResumenCategoriaMes(int idCategoria, int idPresupuesto, int anio, int mes)
        {
            using var conn = Conexion.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("sp_obtener_resumen_categoria_mes", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@p_id_categoria", idCategoria);
            cmd.Parameters.AddWithValue("@p_id_presupuesto", idPresupuesto);
            cmd.Parameters.AddWithValue("@p_anio", anio);
            cmd.Parameters.AddWithValue("@p_mes", mes);

            var pP = cmd.Parameters.Add("@p_monto_presupuestado", SqlDbType.Decimal); pP.Direction = ParameterDirection.Output;
            var pE = cmd.Parameters.Add("@p_monto_ejecutado", SqlDbType.Decimal); pE.Direction = ParameterDirection.Output;
            var pR = cmd.Parameters.Add("@p_porcentaje", SqlDbType.Decimal); pR.Direction = ParameterDirection.Output;

            cmd.ExecuteNonQuery();

            return (
                pP.Value == DBNull.Value ? 0 : (decimal)pP.Value,
                pE.Value == DBNull.Value ? 0 : (decimal)pE.Value,
                pR.Value == DBNull.Value ? 0 : (decimal)pR.Value
            );
        }

        public static void InsertarDetalle(int idPresupuesto, int idSubcategoria,
            decimal monto, string observaciones)
        {
            using var conn = Conexion.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("sp_insertar_presupuesto_detalle", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@p_id_presupuesto",  idPresupuesto);
            cmd.Parameters.AddWithValue("@p_id_subcategoria", idSubcategoria);
            cmd.Parameters.AddWithValue("@p_monto_mensual", monto);
            cmd.Parameters.AddWithValue("@p_observaciones", observaciones ?? "");
            cmd.Parameters.AddWithValue("@p_creado_por", Session.IdUsuario);
            cmd.ExecuteNonQuery();
        }

        public static void ActualizarDetalle(int idDetalle, decimal monto, string observaciones)
        {
            using var conn = Conexion.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("sp_actualizar_presupuesto_detalle", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@p_id_detalle", idDetalle);
            cmd.Parameters.AddWithValue("@p_monto_mensual", monto);
            cmd.Parameters.AddWithValue("@p_observaciones", observaciones ?? "");
            cmd.Parameters.AddWithValue("@p_modificado_por", Session.IdUsuario);
            cmd.ExecuteNonQuery();
        }

        public static void EliminarDetalle(int idDetalle)
        {
            using var conn = Conexion.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("sp_eliminar_presupuesto_detalle", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@p_id_detalle", idDetalle);
            cmd.ExecuteNonQuery();
        }

        public static PresupuestoDetalle? ConsultarDetalle(int idDetalle)
        {
            using var conn = Conexion.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("sp_consultar_presupuesto_detalle", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@p_id_detalle", idDetalle);
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;
            return new PresupuestoDetalle
            {
                Id = r.GetInt32(0),
                IdSubcategoria = r.GetInt32(3),
                NombreSubcategoria = r.GetString(4),
                MontoMensual = r.GetDecimal(8),
                ObservacionMonto = r.IsDBNull(9) ? "" : r.GetString(9)
            };
        }

        public static List<PresupuestoDetalle> ListarDetalles(int idPresupuesto)
        {
            var list = new List<PresupuestoDetalle>();
            using var conn = Conexion.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("sp_listar_detalles_presupuesto", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@p_id_presupuesto", idPresupuesto);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                list.Add(new PresupuestoDetalle
                {
                    Id = r.GetInt32(0),
                    IdSubcategoria = r.GetInt32(1),
                    NombreSubcategoria = r.GetString(2),
                    MontoMensual = r.GetDecimal(4),
                    ObservacionMonto = r.IsDBNull(5) ? "" : r.GetString(5)
                });
            return list;
        }

        public static decimal FnMontoEjecutado(int idSubcategoria, int anio, int mes)
        {
            using var conn = Conexion.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("SELECT dbo.fn_calcular_monto_ejecutado(@s, @a, @m)", conn);
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
            using var cmd = new SqlCommand("SELECT dbo.fn_calcular_porcentaje_ejecutado(@s, @a, @m, @p)", conn);
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
            using var cmd = new SqlCommand("SELECT dbo.fn_obtener_balance_subcategoria(@p, @s, @a, @m)", conn);
            cmd.Parameters.AddWithValue("@p", idPresupuesto);
            cmd.Parameters.AddWithValue("@s", idSubcategoria);
            cmd.Parameters.AddWithValue("@a", anio);
            cmd.Parameters.AddWithValue("@m", mes);
            var r = cmd.ExecuteScalar();
            return r == DBNull.Value || r == null ? 0 : Convert.ToDecimal(r);
        }

        public static decimal FnTotalCategoriaMes(int idCategoria, int idPresupuesto, int anio, int mes)
        {
            using var conn = Conexion.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("SELECT dbo.fn_obtener_total_categoria_mes(@c, @p, @a, @m)", conn);
            cmd.Parameters.AddWithValue("@c", idCategoria);
            cmd.Parameters.AddWithValue("@p", idPresupuesto);
            cmd.Parameters.AddWithValue("@a", anio);
            cmd.Parameters.AddWithValue("@m", mes);
            var r = cmd.ExecuteScalar();
            return r == DBNull.Value || r == null ? 0 : Convert.ToDecimal(r);
        }

        public static decimal FnTotalEjecutadoCategoriaMes(int idCategoria, int anio, int mes)
        {
            using var conn = Conexion.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("SELECT dbo.fn_obtener_total_ejecutado_categoria_mes(@c, @a, @m)", conn);
            cmd.Parameters.AddWithValue("@c", idCategoria);
            cmd.Parameters.AddWithValue("@a", anio);
            cmd.Parameters.AddWithValue("@m", mes);
            var r = cmd.ExecuteScalar();
            return r == DBNull.Value || r == null ? 0 : Convert.ToDecimal(r);
        }

        public static decimal FnProyeccionGastoMensual(int idSubcategoria, int anio, int mes)
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

        public static decimal FnPromedioGastoSubcategoria(int idSubcategoria, int cantidadMeses)
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
            using var cmd = new SqlCommand("SELECT dbo.fn_validar_vigencia_presupuesto(@f, @p)", conn);
            cmd.Parameters.AddWithValue("@f", fecha.Date);
            cmd.Parameters.AddWithValue("@p", idPresupuesto);
            var r = cmd.ExecuteScalar();
            return r != DBNull.Value && r != null && Convert.ToBoolean(r);
        }
    }
}