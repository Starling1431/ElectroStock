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
using System.Windows.Markup;

namespace Studium
{
    public partial class frmAgregarUsuario : Form
    {
        //Agregar Usuarios
        // string connectionString = "Data Source=DESKTOP-JDLBI3R\\SQLEXPRESS;Initial Catalog=SistemaInventario;User ID=sa;Password=1234;TrustServerCertificate=True";
        string connectionString = ConnectionClass.Conexion;
        //string connectionString = "Data Source=DESKTOP-VR5CCQE\\SQLEXPRESS;Initial Catalog=SistemaInventario;User ID=sa;Password=1935;TrustServerCertificate=True";
        public event Action UsuarioAgregado;
        public frmAgregarUsuario()
        {
            InitializeComponent();
            CargarRoles();
            btnAgregar.Click += BtnAgregar_Click;
        }

        private void CargarRoles()
        {
            // Agregar roles al ComboBox
            comboBox1.Items.Add("Administrador");
            comboBox1.Items.Add("Empleado");
            comboBox1.SelectedIndex = 0; // Selección por defecto
        }

        private void BtnAgregar_Click(object sender, EventArgs e)
        {
          
            string nombreUsuario = bunifuTextBox1.Text.Trim();
            string contrasena = bunifuTextBox2.Text.Trim();
            string rol = comboBox1.SelectedItem.ToString();
            // Verificar si el nombre de usuario y contraseña ya existen

            if (ValidarUsuario(nombreUsuario, contrasena))
            {
                MessageBox.Show("El nombre de usuario y la contraseña ya están en uso. Por favor, use una contraseña diferente.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(nombreUsuario) || string.IsNullOrWhiteSpace(contrasena))
            {
                MessageBox.Show("Por favor, complete todos los campos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
          
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO Usuarios (NombreUsuario, Contrasena, Rol) VALUES (@Nombre, @Contrasena, @Rol)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Nombre", nombreUsuario);
                        command.Parameters.AddWithValue("@Contrasena", contrasena);
                        command.Parameters.AddWithValue("@Rol", rol);

                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Usuario agregado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            UsuarioAgregado?.Invoke(); // Disparar el evento
                
                        }
                        else
                        {
                            MessageBox.Show("Error al agregar el usuario.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimpiarCampos()
        {
            bunifuTextBox1.Text = string.Empty;
            bunifuTextBox2.Text = string.Empty;
            comboBox1.SelectedIndex = 0;
        }
        private bool ValidarUsuario(string nombreUsuario, string contrasena)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Consulta para buscar si el nombre de usuario ya existe con la misma contraseña
                    string query = "SELECT COUNT(*) FROM Usuarios WHERE NombreUsuario = @NombreUsuario AND Contrasena = @Contrasena";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@NombreUsuario", nombreUsuario);
                        command.Parameters.AddWithValue("@Contrasena", contrasena);

                        // Si devuelve más de 0, el usuario y la contraseña ya existen juntos
                        int count = Convert.ToInt32(command.ExecuteScalar());
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al validar el usuario: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        private void iconButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void bunifuPanel2_Click(object sender, EventArgs e)
        {

        }

        private void bunifuTextBox2_TextChanged(object sender, EventArgs e)
        {
           bunifuTextBox2.PasswordChar = '*';
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
