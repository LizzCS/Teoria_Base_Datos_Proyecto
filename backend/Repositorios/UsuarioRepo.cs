using System.Data;
using Microsoft.Data.SqlClient;
using SistemaBancario.Database;
using SistemaBancario.Models;

namespace SistemaBancario.Repositories
{
    public class UsuarioRepo
    {
        public static bool Login(string correo, string contrasenia)
        {
            using var conn = Conexion.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(
                "SELECT usuario_id, nombre, apellido, correo_electronico " +
                "FROM usuario WHERE correo_electronico = @c AND contrasenia = @p AND estado_usuario = 1", conn);
            cmd.Parameters.AddWithValue("@c", correo);
            cmd.Parameters.AddWithValue("@p", contrasenia);
            using var r = cmd.ExecuteReader();
            if (r.Read())
            {
                Session.IdUsuario = r.GetInt32(0);
                Session.Nombre = r.GetString(1);
                Session.Apellido  = r.GetString(2);
                Session.Correo = r.GetString(3);
                return true;
            }
            return false;
        }

        public static void Insertar(string nombre, string apellido, string correo,
            string contrasenia, decimal salario)
        {
            using var conn = Conexion.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("sp_insertar_usuario", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@p_nombre", nombre);
            cmd.Parameters.AddWithValue("@p_apellido", apellido);
            cmd.Parameters.AddWithValue("@p_correo_electronico", correo);
            cmd.Parameters.AddWithValue("@p_contrasenia", contrasenia);
            cmd.Parameters.AddWithValue("@p_salario_mensual", salario);
            cmd.Parameters.AddWithValue("@p_creado_por",  1);
            cmd.ExecuteNonQuery();
        }

        public static void Actualizar(int id, string nombre, string apellido, decimal salario)
        {
            using var conn = Conexion.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("sp_actualizar_usuario", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@p_id_usuario", id);
            cmd.Parameters.AddWithValue("@p_nombre", nombre);
            cmd.Parameters.AddWithValue("@p_apellido", apellido);
            cmd.Parameters.AddWithValue("@p_salario_mensual", salario);
            cmd.Parameters.AddWithValue("@p_modificado_por",  Session.IdUsuario);
            cmd.ExecuteNonQuery();
        }

        public static void Eliminar(int id)
        {
            using var conn = Conexion.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("sp_eliminar_usuario", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@p_id_usuario",     id);
            cmd.Parameters.AddWithValue("@p_modificado_por", Session.IdUsuario);
            cmd.ExecuteNonQuery();
        }

        public static Usuario? Consultar(int id)
        {
            using var conn = Conexion.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("sp_consultar_usuario", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@p_id_usuario", id);
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;
            return new Usuario
            {
                IdUsuario = r.GetInt32(0),
                Nombre = r.GetString(1),
                Apellido = r.GetString(2),
                CorreoElectronico = r.GetString(3),
                SalarioMensual  = r.GetDecimal(6),
                EstadoUsuario = Convert.ToBoolean(r.GetValue(7))
            };
        }

        public static List<Usuario> Listar()
        {
            var list = new List<Usuario>();
            using var conn = Conexion.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("sp_listar_usuarios", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            using var r = cmd.ExecuteReader();
            while (r.Read())
                list.Add(new Usuario
                {
                    IdUsuario = r.GetInt32(0),
                    Nombre = r.GetString(1),
                    Apellido = r.GetString(2),
                    CorreoElectronico = r.GetString(3),
                    SalarioMensual = r.GetDecimal(4),
                    EstadoUsuario = r.GetString(5) == "Activo"
                });
            return list;
        }
    }
}
