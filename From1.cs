using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Laboratorio4
{
    public partial class Form1 : Form
    {
        // Tu cadena de conexión a MySQL
        private string conexionString = "Server=localhost;Database=productosdb;Uid=root;Pwd=Cereal123;";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CargarTabla();
        }

        // Carga los datos desde la BD al DataGridView
        private void CargarTabla()
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(conexionString))
                {
                    con.Open();
                    string query = "SELECT id, nombre, precio, cantidad, imagen FROM productos";

                    MySqlDataAdapter da = new MySqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Evita que C# cree columnas nuevas automáticamente
                    dataGridView1.AutoGenerateColumns = false;

                    // Enlaza las columnas de tu interfaz con los campos SQL
                    if (dataGridView1.Columns.Contains("FOLIO"))
                        dataGridView1.Columns["FOLIO"].DataPropertyName = "id";

                    if (dataGridView1.Columns.Contains("Nombre"))
                        dataGridView1.Columns["Nombre"].DataPropertyName = "nombre";

                    if (dataGridView1.Columns.Contains("Precio"))
                        dataGridView1.Columns["Precio"].DataPropertyName = "precio";

                    if (dataGridView1.Columns.Contains("Cantidad"))
                        dataGridView1.Columns["Cantidad"].DataPropertyName = "cantidad";

                    if (dataGridView1.Columns.Contains("Imagen"))
                        dataGridView1.Columns["Imagen"].DataPropertyName = "imagen";

                    // Asigna los datos a la tabla existente
                    dataGridView1.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al conectar con la base de datos: " + ex.Message, "Error de Conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Cargar imagen desde el archivo al PictureBox
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Archivos de imagen|*.jpg;*.jpeg;*.png;*.bmp";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    pictureBox1.Image = Image.FromFile(ofd.FileName);
                }
            }
        }

        // Convertir imagen a arreglo de bytes
        private byte[] ImageToByteArray(Image img)
        {
            if (img == null) return null;
            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
        }

        // Convertir bytes a imagen
        private Image ByteArrayToImage(byte[] byteArray)
        {
            if (byteArray == null || byteArray.Length == 0) return null;
            using (MemoryStream ms = new MemoryStream(byteArray))
            {
                return Image.FromStream(ms);
            }
        }

        // Botón Agregar (INSERT) - El ID es AUTO_INCREMENT en MySQL
        private void button1_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos()) return;

            try
            {
                using (MySqlConnection con = new MySqlConnection(conexionString))
                {
                    string query = "INSERT INTO productos (nombre, precio, cantidad, imagen) VALUES (@nombre, @precio, @cantidad, @imagen)";
                    MySqlCommand cmd = new MySqlCommand(query, con);

                    cmd.Parameters.AddWithValue("@nombre", textBox3.Text);
                    cmd.Parameters.AddWithValue("@precio", decimal.Parse(textBox4.Text));
                    cmd.Parameters.AddWithValue("@cantidad", int.Parse(textBox5.Text));
                    cmd.Parameters.AddWithValue("@imagen", ImageToByteArray(pictureBox1.Image));

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Producto registrado correctamente en MySQL.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarTabla();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar en BD: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Botón Modificar (UPDATE por ID)
        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox2.Text) || !int.TryParse(textBox2.Text, out int idProducto))
            {
                MessageBox.Show("Seleccione un registro válido de la tabla para modificar.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidarCampos()) return;

            try
            {
                using (MySqlConnection con = new MySqlConnection(conexionString))
                {
                    string query = "UPDATE productos SET nombre = @nombre, precio = @precio, cantidad = @cantidad, imagen = @imagen WHERE id = @id";
                    MySqlCommand cmd = new MySqlCommand(query, con);

                    cmd.Parameters.AddWithValue("@id", idProducto);
                    cmd.Parameters.AddWithValue("@nombre", textBox3.Text);
                    cmd.Parameters.AddWithValue("@precio", decimal.Parse(textBox4.Text));
                    cmd.Parameters.AddWithValue("@cantidad", int.Parse(textBox5.Text));
                    cmd.Parameters.AddWithValue("@imagen", ImageToByteArray(pictureBox1.Image));

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Producto actualizado en la BD.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarTabla();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al modificar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Botón Eliminar (DELETE por ID)
        private void button3_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox2.Text) || !int.TryParse(textBox2.Text, out int idProducto))
            {
                MessageBox.Show("Seleccione un registro válido de la tabla para eliminar.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("¿Desea eliminar este producto permanentemente de la base de datos?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    using (MySqlConnection con = new MySqlConnection(conexionString))
                    {
                        string query = "DELETE FROM productos WHERE id = @id";
                        MySqlCommand cmd = new MySqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@id", idProducto);

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }

                    CargarTabla();
                    LimpiarFormulario();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al eliminar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Botón Limpiar
        private void button4_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
        }

        // Botón Salir
        private void button5_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // Al hacer clic en la tabla, carga los campos
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow fila = dataGridView1.Rows[e.RowIndex];
                textBox2.Text = fila.Cells["FOLIO"].Value?.ToString(); // Muestra el ID como Folio
                textBox3.Text = fila.Cells["Nombre"].Value?.ToString();
                textBox4.Text = fila.Cells["Precio"].Value?.ToString();
                textBox5.Text = fila.Cells["Cantidad"].Value?.ToString();

                if (fila.Cells["Imagen"].Value != DBNull.Value && fila.Cells["Imagen"].Value is byte[] bytes)
                {
                    pictureBox1.Image = ByteArrayToImage(bytes);
                }
                else
                {
                    pictureBox1.Image = null;
                }
            }
        }

        // Búsqueda en tiempo real sobre la tabla cargada de MySQL
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (dataGridView1.DataSource is DataTable dt)
            {
                string filtro = textBox1.Text.Trim().Replace("'", "''");
                dt.DefaultView.RowFilter = string.Format("Nombre LIKE '%{0}%' OR Convert(FOLIO, 'System.String') LIKE '%{0}%'", filtro);
            }
        }

        private void LimpiarFormulario()
        {
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            pictureBox1.Image = null;
            textBox2.Focus();
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(textBox3.Text) ||
                !decimal.TryParse(textBox4.Text, out _) ||
                !int.TryParse(textBox5.Text, out _))
            {
                MessageBox.Show("Por favor, llene Nombre, Precio y Cantidad con valores numéricos válidos.", "Campos Requeridos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Ya que la columna 'imagen' en MySQL es NOT NULL, verificamos que haya una cargada
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("Debes seleccionar una imagen para el producto (la columna en la base de datos no acepta valores nulos).", "Imagen Requerida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void label1_Click(object sender, EventArgs e) { }
        private void label6_Click(object sender, EventArgs e) { }
    }
}
