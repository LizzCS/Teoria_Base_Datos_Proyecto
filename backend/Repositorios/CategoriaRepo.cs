using System.Data;
using Microsoft.Data.SqlClient;
using SistemaBancario.Database;
using SistemaBancario.Models;
using SistemaBancario.Menus;

namespace SistemaBancario.Repositories
{
    public class CategoriaRepo
    {
        public static void Insertar(string nombre, string descripcion, string tipo)
        {
            using var conn = Conexion.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("sp_insertar_categoria", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@p_nombre", nombre);
            cmd.Parameters.AddWithValue("@p_descripcion", descripcion);
            cmd.Parameters.AddWithValue("@p_tipo_categoria", tipo);
            cmd.Parameters.AddWithValue("@p_creado_por", Session.IdUsuario);
            cmd.ExecuteNonQuery();
        }

        public static void Actualizar(int id, string nombre, string descripcion)
        {
            using var conn = Conexion.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("sp_actualizar_categoria", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@p_id_categoria", id);
            cmd.Parameters.AddWithValue("@p_nombre", nombre);
            cmd.Parameters.AddWithValue("@p_descripcion", descripcion);
            cmd.Parameters.AddWithValue("@p_modificado_por", Session.IdUsuario);
            cmd.ExecuteNonQuery();
        }

        public static void Eliminar(int id)
        {
            using var conn = Conexion.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("sp_eliminar_categoria", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@p_id_categoria", id);
            cmd.ExecuteNonQuery();
        }

        public static List<Categoria> Listar(string tipo = null)
        {
            var list = new List<Categoria>();
            using var conn = Conexion.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("sp_listar_categorias", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@p_id_usuario", Session.IdUsuario);
            cmd.Parameters.AddWithValue("@p_tipo_categoria", (object)tipo ?? DBNull.Value);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                list.Add(new Categoria
                {
                    IdCategoria   = r.GetInt32(0),
                    Nombre        = r.GetString(1),
                    Descripcion   = r.IsDBNull(2) ? "" : r.GetString(2),
                    TipoCategoria = r.GetString(3)
                });
            return list;
        }

        // ── Subcategorias ─────────────────────────────────────────
        public static void InsertarSubcategoria(int idCategoria, string nombre, string descripcion, bool esDefecto)
        {
            using var conn = Conexion.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("sp_insertar_subcategoria", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@p_id_categoria", idCategoria);
            cmd.Parameters.AddWithValue("@p_nombre", nombre);
            cmd.Parameters.AddWithValue("@p_descripcion", descripcion);
            cmd.Parameters.AddWithValue("@p_es_defecto", esDefecto);
            cmd.Parameters.AddWithValue("@p_creado_por", Session.IdUsuario);
            cmd.ExecuteNonQuery();
        }

        public static List<Subcategoria> ListarSubcategorias(int idCategoria)
        {
            var list = new List<Subcategoria>();
            using var conn = Conexion.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("sp_listar_subcategorias_por_categoria", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@p_id_categoria", idCategoria);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                list.Add(new Subcategoria
                {
                    IdSubcategoria = r.GetInt32(0),
                    Nombre = r.GetString(1),
                    Descripcion = r.IsDBNull(2) ? "" : r.GetString(2),
                    SubcategoriaPorDefecto = Convert.ToBoolean(r.GetValue(3)),
                    EsActivo = Convert.ToBoolean(r.GetValue(4))
                });
            return list;
        }
        public static void ActualizarSubcategoria(int id, string nombre, string descripcion)
        {
            using var conn = Conexion.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("sp_actualizar_subcategoria", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@p_id_subcategoria", id);
            cmd.Parameters.AddWithValue("@p_nombre", nombre);
            cmd.Parameters.AddWithValue("@p_descripcion", descripcion);
            cmd.Parameters.AddWithValue("@p_modificado_por", Session.IdUsuario);
            cmd.ExecuteNonQuery();
        }

        public static void EliminarSubcategoria(int id)
        {
            using var conn = Conexion.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("sp_eliminar_subcategoria", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@p_id_subcategoria", id);
            cmd.ExecuteNonQuery();
        }

        public static Categoria? Consultar(int id)
        {
            using var conn = Conexion.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("sp_consultar_categoria", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@p_id_categoria", id);
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;
            return new Categoria
            {
                IdCategoria = r.GetInt32(0),
                Nombre = r.GetString(1),
                Descripcion = r.IsDBNull(2) ? "" : r.GetString(2),
                TipoCategoria = r.GetString(3)
            };
        }

        public static void ConsultarSubcategoria(int id)
        {
            using var conn = Conexion.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("sp_consultar_subcategoria", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@p_id_subcategoria", id);
            using var r = cmd.ExecuteReader();
            if (!r.Read()) { Console.WriteLine("  No encontrada."); return; }

            UI.Separador();
            Console.WriteLine($"  ID            : {r.GetInt32(0)}");
            Console.WriteLine($"  Categoría     : {r.GetString(2)}");
            Console.WriteLine($"  Tipo          : {r.GetString(3)}");
            Console.WriteLine($"  Nombre        : {r.GetString(4)}");
            Console.WriteLine($"  Descripción   : {r.GetString(5)}");
            Console.WriteLine($"  Estado        : {r.GetString(6)}");
            Console.WriteLine($"  Por defecto   : {r.GetString(7)}");
            UI.Separador();
        }
    }
}
