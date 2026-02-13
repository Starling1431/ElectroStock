using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using Bunifu.UI.WinForms;

namespace Studium
{
    public partial class frmEditarProductos : Form
    {
        string connectionString = ConnectionClass.Conexion;
        private int _productoId;
        public frmEditarProductos(int productoId, string nombre, string marca, string modelo, string descripcion, double precio, int stock, string proveedor)
        {
            InitializeComponent();

            // Asignar los valores a los controles del formulario
            _productoId = productoId; // ID del producto (se usa para actualizaciones en la base de datos)
            bunifuTextBox1.Text = nombre;        // Nombre del producto
            bunifuTextBox2.Text = marca;         // Marca del producto
            bunifuTextBox3.Text = modelo;        // Modelo del producto
            richTextBox1.Text = descripcion;     // Descripción del producto
            txtPrecio.Text = precio.ToString("F2"); // Precio del producto, formato decimal con 2 dígitos
            bunifuTextBox5.Text = stock.ToString(); // Stock del producto
          

            // Si usas un ComboBox para seleccionar el proveedor
            comboBox1.SelectedValue = proveedor;
        }


        private void bunifuPanel2_Click(object sender, EventArgs e)
        {

        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            int idProveedor = Convert.ToInt32(comboBox1.SelectedValue); // ID del proveedor seleccionado

            if (string.IsNullOrWhiteSpace(bunifuTextBox1.Text) || string.IsNullOrWhiteSpace(bunifuTextBox2.Text)
                || string.IsNullOrWhiteSpace(bunifuTextBox3.Text)
         || string.IsNullOrWhiteSpace(richTextBox1.Text) || string.IsNullOrWhiteSpace(txtPrecio.Text)
         || string.IsNullOrWhiteSpace(bunifuTextBox5.Text) || comboBox1.SelectedValue == null || idProveedor <= 0)

            {
                MessageBox.Show("Por favor, complete todos los campos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Consulta SQL para actualizar el producto en la base de datos
                    string query = "UPDATE Productos " +
                                   "SET Nombre = @Nombre, Marca = @Marca, Modelo = @Modelo, Descripcion = @Descripcion, " +
                                   "Precio = @Precio, Stock = @Stock, Proveedor = @Proveedor " +
                                   "WHERE ID = @Id";
                    SqlCommand command = new SqlCommand(query, connection);

                    // Agregar los parámetros de la consulta con los valores de los controles del formulario
                    command.Parameters.AddWithValue("@Nombre", bunifuTextBox1.Text);
                    command.Parameters.AddWithValue("@Marca", bunifuTextBox2.Text);
                    command.Parameters.AddWithValue("@Modelo", bunifuTextBox3.Text);
                    command.Parameters.AddWithValue("@Descripcion", richTextBox1.Text);
                    command.Parameters.AddWithValue("@Precio", Convert.ToDecimal(txtPrecio.Text));
                    command.Parameters.AddWithValue("@Stock", Convert.ToInt32(bunifuTextBox5.Text));
                    command.Parameters.AddWithValue("@Proveedor", idProveedor);

                    // Este parámetro contiene el ID del producto que se va a editar
                    command.Parameters.AddWithValue("@Id", _productoId);

                    // Abrir la conexión y ejecutar la consulta
                    connection.Open();
                    int result = command.ExecuteNonQuery();

                    if (result > 0)
                    {
                        // Si la actualización fue exitosa, mostrar un mensaje y cerrar el formulario
                        MessageBox.Show("Producto actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                    }
                    else
                    {
                        // Si la actualización no fue exitosa, mostrar un mensaje de error
                        MessageBox.Show("Error al actualizar el producto.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                // Si ocurre un error en la actualización, mostrar el mensaje de excepción
                MessageBox.Show($"Error al actualizar el producto: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void CargarProveedores()
        {
            // Crear la consulta con un JOIN para obtener el Nombre_Proveedor
            string query = "SELECT ID_Proveedor, Nombre_Proveedor FROM Proveedores";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable proveedores = new DataTable();
                    adapter.Fill(proveedores);

                    // Configura el ComboBox con el ID como valor y el nombre como texto visible
                    comboBox1.DataSource = proveedores;
                    comboBox1.DisplayMember = "Nombre_Proveedor"; // Muestra el nombre
                    comboBox1.ValueMember = "ID_Proveedor";      // Usa el ID como valor
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al cargar proveedores: {ex.Message}");
                }
            }
        }

        // Método para limpiar los campos del formulario después de agregar un producto
        private void LimpiarCampos()
        {
            bunifuTextBox1.Clear();      // Nombre del producto
            bunifuTextBox2.Clear();         // Marca del producto
            bunifuTextBox3.Clear();        // Modelo del producto
            richTextBox1.Clear();     // Descripción del producto
            txtPrecio.Clear(); // Precio del producto, formato decimal con 2 dígitos
            bunifuTextBox5.Clear(); // Stock del producto


        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void frmEditarProductos_Load(object sender, EventArgs e)
        {
            CargarProveedores();
        }
    }
}
