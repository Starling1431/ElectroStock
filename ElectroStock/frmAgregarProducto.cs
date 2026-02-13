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
using System.Windows.Forms;
using System.Windows.Media.Converters;
using Bunifu.UI.WinForms;

namespace Studium
{
    public partial class frmAgregarProducto : Form
    {
        string connectionString = ConnectionClass.Conexion;
        public frmAgregarProducto()
        {
            InitializeComponent();
            txtPrecio.KeyPress += bunifuTextBox3_KeyPress;
            txtPrecio.KeyDown += bunifuTextBox3_KeyDown;
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void bunifuTextBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            if (e.KeyChar == '.' && ((sender as TextBox).Text.Contains(".")))
            {
                e.Handled = true;
            }
        }

        private void bunifuTextBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right || e.KeyCode == Keys.Tab)
            {
                return;
            }

            if ((e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9) || (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9))
            {
                return;
            }

            if (e.KeyCode == Keys.OemPeriod || e.KeyCode == Keys.Decimal)
            {
                if ((sender as TextBox).Text.Contains("."))
                {
                    e.SuppressKeyPress = true;
                }
                return;
            }

            e.SuppressKeyPress = true;
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            string nombre = bunifuTextBox1.Text.Trim();
            string marca = bunifuTextBox2.Text.Trim();
            string modelo = bunifuTextBox3.Text.Trim();
            string descripcion = richTextBox1.Text.Trim();
            int stock = int.Parse(bunifuTextBox5.Text);
            double precio = double.Parse(txtPrecio.Text);

            // Obtener el ID del proveedor seleccionado (es un número)
            int idProveedor = Convert.ToInt32(comboBox1.SelectedValue);

            if (idProveedor == 0) // Verificar si el proveedor no está seleccionado
            {
                MessageBox.Show("Por favor, selecciona un proveedor.");
                return;
            }

            // Validación de campos
            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(marca) ||
                string.IsNullOrWhiteSpace(modelo) || string.IsNullOrWhiteSpace(descripcion) ||
                string.IsNullOrWhiteSpace(stock.ToString()) || string.IsNullOrWhiteSpace(precio.ToString()))
            {
                MessageBox.Show("Por favor, complete todos los campos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO Productos (Nombre, Marca, Modelo, Descripcion, Precio, Stock, Proveedor) " +
                                   "VALUES (@Nombre, @Marca, @Modelo, @Descripcion, @Precio, @Stock, @Proveedor)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Nombre", nombre);
                        command.Parameters.AddWithValue("@Marca", marca);
                        command.Parameters.AddWithValue("@Modelo", modelo);
                        command.Parameters.AddWithValue("@Descripcion", descripcion);
                        command.Parameters.AddWithValue("@Precio", precio);
                        command.Parameters.AddWithValue("@Stock", stock);
                        command.Parameters.AddWithValue("@Proveedor", idProveedor); // Usar el ID del proveedor

                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Producto agregado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Error al agregar el Producto.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void frmAgregarProducto_Load(object sender, EventArgs e)
        {
            // Cargar los proveedores al iniciar el formulario
            CargarProveedores();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Puedes usar este evento si necesitas realizar alguna acción cuando se cambia la selección
        }

        private void comboBox1_Click(object sender, EventArgs e)
        {
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;  // Asegurarse de que el ComboBox sea de solo lectura
        }

        private void bunifuPanel2_Click(object sender, EventArgs e)
        {

        }
    }
}
