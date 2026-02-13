using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Bunifu.UI.WinForms;

namespace Studium
{
    public partial class frmEditarClientes : Form
    {
        string connectionString = ConnectionClass.Conexion;
        private int _clienteId;
        public frmEditarClientes(int clienteId, string nombre, string apellido, string telefono, string correoElectronico, string direccion, string estado)
        {
            _clienteId = clienteId;
            InitializeComponent();
            bunifuTextBox1.Text = nombre;
            bunifuTextBox2.Text = apellido;
            txtTelefono.Text = telefono;
            bunifuTextBox4.Text = correoElectronico;
            bunifuTextBox5.Text = direccion;
            comboBox1.SelectedItem = estado;
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
            string estadooo = comboBox1.SelectedItem.ToString();

            if (string.IsNullOrWhiteSpace(bunifuTextBox1.Text) || string.IsNullOrWhiteSpace(bunifuTextBox2.Text)
              || string.IsNullOrWhiteSpace(txtTelefono.Text) || string.IsNullOrWhiteSpace(bunifuTextBox4.Text)
              || string.IsNullOrWhiteSpace(bunifuTextBox5.Text) || string.IsNullOrWhiteSpace(estadooo))
            
            {
                MessageBox.Show("Por favor, complete todos los campos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Consulta SQL para actualizar un cliente en la base de datos
                    string query = "UPDATE Clientes SET Nombre = @Nombre, Apellido = @Apellido, Telefono = @Telefono, Correo_Electronico = @CorreoElectronico, Direccion = @Direccion, Estado = @Estado WHERE ID_Cliente = @Id";
                    SqlCommand command = new SqlCommand(query, connection);

                    // Agregar los parámetros de la consulta con los valores de los controles del formulario
                    command.Parameters.AddWithValue("@Nombre", bunifuTextBox1.Text);
                    command.Parameters.AddWithValue("@Apellido", bunifuTextBox2.Text);
                    command.Parameters.AddWithValue("@Telefono", txtTelefono.Text);
                    command.Parameters.AddWithValue("@CorreoElectronico", bunifuTextBox4.Text);
                    command.Parameters.AddWithValue("@Direccion", bunifuTextBox5.Text);
                    command.Parameters.AddWithValue("@Estado", estadooo);

                    command.Parameters.AddWithValue("@Id", _clienteId);

                    // Abrir la conexión y ejecutar la consulta
                    connection.Open();
                    int result = command.ExecuteNonQuery();

                    if (result > 0)
                    {
                        // Si la actualización fue exitosa, mostrar un mensaje y cerrar el formulario
                        MessageBox.Show("Cliente actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                    }
                    else
                    {
                        // Si la actualización no fue exitosa, mostrar un mensaje de error
                        MessageBox.Show("Error al actualizar al Cliente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                // Si ocurre un error en la actualización, mostrar el mensaje de excepción
                MessageBox.Show($"Error al actualizar al Cliente: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void txtTelefono_Click(object sender, EventArgs e)
        {
            txtTelefono.SelectionStart = 0;
            txtTelefono.SelectionLength = 0;
        }
    }
}
