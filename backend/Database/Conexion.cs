using Microsoft.Data.SqlClient;

namespace SistemaBancario.Database
{
    public class Conexion
    {
        private static readonly string _connectionString = "Server=localhost;Database=sistema_bancario;Trusted_Connection=True;TrustServerCertificate=True;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}