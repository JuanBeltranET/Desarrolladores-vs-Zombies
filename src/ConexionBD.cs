using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;

namespace Project2
{
    public class ConexionBD
    {
        private string cadenaConexion =
            "Server=localhost;" +
            "Port=3306;" +
            "Database=zombie_escape;" +
            "User ID=root;" +
            "Password=14405255;";

        public void Conectar()
        {
            try
            {
                using (MySqlConnection conexion =
                    new MySqlConnection(cadenaConexion))
                {
                    conexion.Open();

                    Console.WriteLine("Conexion exitosa a MySQL");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error de conexion: " + ex.Message);
            }
        }
    }
}