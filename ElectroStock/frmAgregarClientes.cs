using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Studium
{
    public partial class frmAgregarClientes : Form
    {
        string connectionString = ConnectionClass.Conexion;
        public frmAgregarClientes()
        {
            InitializeComponent();
          
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void bunifuPanel2_Click(object sender, EventArgs e)
        {

        }

        private void txtTelefono_Click(object sender, EventArgs e)
        {
            txtTelefono.SelectionStart = 0;
            txtTelefono.SelectionLength = 0;
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            string nombre = bunifuTextBox1.Text.Trim();
            string apellido = bunifuTextBox2.Text.Trim();
            string telefono = txtTelefono.Text.Trim();
            string correoElectronico = bunifuTextBox4.Text.Trim();
            string direccion = bunifuTextBox5.Text.Trim();
            string estado = "Activo";

            // Verificar si todos los campos requeridos están llenos
            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellido)
                || string.IsNullOrWhiteSpace(telefono) || string.IsNullOrWhiteSpace(correoElectronico)
                || string.IsNullOrWhiteSpace(direccion) || string.IsNullOrWhiteSpace(estado))
            {
                MessageBox.Show("Por favor, complete todos los campos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Consulta SQL para insertar un cliente en la base de datos
                    string query = "INSERT INTO Clientes (Nombre, Apellido, Telefono, Correo_Electronico, Direccion, Fecha_Registro, Estado) " +
                                   "VALUES (@Nombre, @Apellido, @Telefono, @Correo_Electronico, @Direccion, GETDATE(), @Estado)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Agregar parámetros a la consulta
                        command.Parameters.AddWithValue("@Nombre", nombre);
                        command.Parameters.AddWithValue("@Apellido", apellido);
                        command.Parameters.AddWithValue("@Telefono", telefono);
                        command.Parameters.AddWithValue("@Correo_Electronico", correoElectronico);
                        command.Parameters.AddWithValue("@Direccion", direccion);
                        command.Parameters.AddWithValue("@Estado", estado);

                        // Ejecutar la consulta
                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Cliente agregado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LimpiarCampos(); // Limpia los campos del formulario (opcional, si tienes un método para ello)
                        }
                        else
                        {
                            MessageBox.Show("Error al agregar el cliente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejar errores y mostrar el mensaje de excepción
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
          
        }
        private void LimpiarCampos() { 
            bunifuTextBox1.Clear();
            bunifuTextBox2.Clear();
          
            txtTelefono.Clear();
            bunifuTextBox4.Clear();
            bunifuTextBox5.Clear();
        }
    }
}
