using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Studium
{
    public partial class frmAgregarProveedores : Form
    {
        //el conection string waza XxD
        string connectionString = ConnectionClass.Conexion;
        // string connectionString = "Data Source=DESKTOP-JDLBI3R\\SQLEXPRESS;Initial Catalog=SistemaInventario;User ID=sa;Password=1234;TrustServerCertificate=True";
        // string connectionString = "Data Source=DESKTOP-IHGJ1TS\\SQLEXPRESS;Initial Catalog=SistemaInventario;Integrated Security=True;TrustServerCertificate=True";
        //  string connectionString = "Data Source=DESKTOP-VR5CCQE\\SQLEXPRESS;Initial Catalog=SistemaInventario;User ID=sa;Password=1935;TrustServerCertificate=True";
        public frmAgregarProveedores()
        {
            InitializeComponent();
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            string nombre = bunifuTextBox1.Text.Trim();
            string contacto = bunifuTextBox2.Text.Trim();
            string telefono = txtTelefono.Text.Trim();
            string correoelectronico = bunifuTextBox4.Text.Trim();
            string producto = bunifuTextBox6.Text.Trim();
            string direccion = bunifuTextBox4.Text.Trim();
            string estado = "Activo";
            // Verificar si el nombre de usuario y contraseña ya existen


            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(contacto) 
                || string.IsNullOrWhiteSpace(telefono) || string.IsNullOrWhiteSpace(correoelectronico)
                || string.IsNullOrWhiteSpace(producto) || string.IsNullOrWhiteSpace(direccion) ||
                string.IsNullOrWhiteSpace(estado))
            {
                MessageBox.Show("Por favor, complete todos los campos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO Proveedores ( Nombre_Proveedor, Contacto, Telefono, Correo_Electronico, Direccion, Productos_Servicios, Estado) VALUES (@Nombre_Proveedor, @Contacto, @Telefono, @Correo_Electronico, @Direccion, @Productos_Servicios, @Estado)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Nombre_Proveedor", nombre);
                        command.Parameters.AddWithValue("@Contacto", contacto);
                        command.Parameters.AddWithValue("@Telefono", telefono);
                        command.Parameters.AddWithValue("@Correo_Electronico", correoelectronico);
                        command.Parameters.AddWithValue("@Direccion", direccion);
                        command.Parameters.AddWithValue("@Productos_Servicios", producto);
                        command.Parameters.AddWithValue("@Estado", estado);
                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Proveedor agregado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                          

                        }
                        else
                        {
                            MessageBox.Show("Error al agregar el Proveedor.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtTelefono_Click(object sender, EventArgs e)
        {
            txtTelefono.SelectionStart = 0;
            txtTelefono.SelectionLength = 0;
        }

        private void bunifuPanel2_Click(object sender, EventArgs e)
        {

        }
    }
}
