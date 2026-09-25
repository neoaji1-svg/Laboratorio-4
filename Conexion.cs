using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
namespace Laboratorio4
{
    public class Conexion
    {
        // Cambia estos datos según tu servidor de MySQL
        private static string cadenaConexion = "Server=localhost;Database=productosdb;Uid=root;Pwd=Cereal123;";

        public static MySqlConnection ObtenerConexion()
        {
            try
            {
                MySqlConnection conexion = new MySqlConnection(cadenaConexion);
                conexion.Open();
                return conexion;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error de conexión: " + ex.Message);
                return null;
            }
        }

        // Consulta de productos con filtro
        public static List<Producto> GetProductos(string filtro = "")
        {
            List<Producto> lista = new List<Producto>();
            string query = "SELECT id, nombre, precio, cantidad, imagen FROM productos";

            if (!string.IsNullOrEmpty(filtro))
            {
                query += " WHERE id LIKE @filtro OR nombre LIKE @filtro OR precio LIKE @filtro OR cantidad LIKE @filtro";
            }

            using (MySqlConnection conn = ObtenerConexion())
            {
                if (conn == null) return lista;

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    if (!string.IsNullOrEmpty(filtro))
                    {
                        cmd.Parameters.AddWithValue("@filtro", "%" + filtro + "%");
                    }

                    using (MySqlDataReader mReader = cmd.ExecuteReader())
                    {
                        while (mReader.Read())
                        {
                            Producto prod = new Producto
                            {
                                Id = Convert.ToInt32(mReader["id"]),
                                Nombre = mReader["nombre"].ToString(),
                                Precio = Convert.ToDecimal(mReader["precio"]),
                                Cantidad = Convert.ToInt32(mReader["cantidad"]),
                                Imagen = mReader["imagen"] != DBNull.Value ? (byte[])mReader["imagen"] : null
                            };
                            lista.Add(prod);
                        }
                    }
                }
            }
            return lista;
        }

        // Insertar registro
        public static bool InsertSeguro(string tbName, Dictionary<string, object> data)
        {
            var columns = string.Join(", ", data.Keys);
            var placeholders = "@" + string.Join(", @", data.Keys);
            string sql = $"INSERT INTO {tbName} ({columns}) VALUES ({placeholders})";

            return EjecutarComando(sql, data);
        }

        // Actualizar registro
        public static bool Actualizar(string tbName, Dictionary<string, object> data, int id)
        {
            List<string> setClause = new List<string>();
            foreach (var key in data.Keys)
            {
                setClause.Add($"{key} = @{key}");
            }

            string sql = $"UPDATE {tbName} SET {string.Join(", ", setClause)} WHERE id = @id_where";
            data["id_where"] = id;

            return EjecutarComando(sql, data);
        }

        // Eliminar registro
        public static bool Eliminar(string tbName, int id)
        {
            string sql = $"DELETE FROM {tbName} WHERE id = @id";
            var data = new Dictionary<string, object> { { "id", id } };

            return EjecutarComando(sql, data);
        }

        private static bool EjecutarComando(string sql, Dictionary<string, object> data)
        {
            try
            {
                using (MySqlConnection conn = ObtenerConexion())
                {
                    if (conn == null) return false;

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        foreach (var kvp in data)
                        {
                            cmd.Parameters.AddWithValue("@" + kvp.Key, kvp.Value ?? DBNull.Value);
                        }
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error SQL: " + ex.Message);
                return false;
            }
        }
    }
}
