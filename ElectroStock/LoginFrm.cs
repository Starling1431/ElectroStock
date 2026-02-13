using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Studium
{
    public partial class LoginFrm : Form
    {
        //el conection string waza XxD
        private string connectionString = ConnectionClass.Conexion;
        // string connectionString = "Data Source=DESKTOP-JDLBI3R\\SQLEXPRESS;Initial Catalog=SistemaInventario;User ID=sa;Password=1234;TrustServerCertificate=True";
        // string connectionString = "Data Source=DESKTOP-IHGJ1TS\\SQLEXPRESS;Initial Catalog=SistemaInventario;Integrated Security=True;TrustServerCertificate=True";
        //  string connectionString = "Data Source=DESKTOP-VR5CCQE\\SQLEXPRESS;Initial Catalog=SistemaInventario;User ID=sa;Password=1935;TrustServerCertificate=True";
        public LoginFrm()
        {
            InitializeComponent();
            bunifuTextBox2.KeyDown += new KeyEventHandler(bunifuTextBox2_KeyDown);
        }
     
      
        private void bunifuPanel1_Click(object sender, EventArgs e)
        {

        }

        private void LoginFrm_Load(object sender, EventArgs e)
        {
            
        }

        private void bunifuTextBox2_TextChanged(object sender, EventArgs e)
        {
                if (string.IsNullOrEmpty(bunifuTextBox2.Text))
                {
                    bunifuTextBox2.PasswordChar = '\0'; // Quita el PasswordChar cuando el texto está vacío
                }
                else
                {
                    bunifuTextBox2.PasswordChar = '*';
                }
         }

        private void bunifuTextBox2_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(bunifuTextBox2.Text))
            {
                bunifuTextBox2.PasswordChar = '\0'; // Quita el PasswordChar cuando el texto está vacío
            }
            else
            {
                bunifuTextBox2.PasswordChar = '*';
            }
        }


        private void bunifuPictureBox1_Click(object sender, EventArgs e)
        {

        }
 
        private void bunifuButton21_Click(object sender, EventArgs e)
        {
       
            string username = bunifuTextBox1.Text;
            string password = bunifuTextBox2.Text;
            string rol = ObtenerRolDesdeBaseDeDatos(username, password);

            using (SqlConnection conexion = new SqlConnection(connectionString))
                try
                {
                    conexion.Open();

                    // Crear la consulta con parámetros para evitar inyección SQL
                    string consulta = "SELECT COUNT(1) FROM Usuarios WHERE NombreUsuario = @usuario AND Contrasena = @contrasena";

                    // Usamos SqlCommand para evitar la inyección SQL
                    SqlCommand comando = new SqlCommand(consulta, conexion);

                    // Agregamos los parámetros con los valores proporcionados por el usuario
                    comando.Parameters.AddWithValue("@usuario", bunifuTextBox1.Text);

                    // Aquí debes aplicar el hash de la contraseña proporcionada, igual que lo hiciste al almacenar la contraseña

                    comando.Parameters.AddWithValue("@contrasena", bunifuTextBox2.Text);

                    // Ejecutar la consulta y obtener el número de filas
                    int usuarioValido = (int)comando.ExecuteScalar();

                    if (usuarioValido > 0)
                    {
                        // El usuario y la contraseña son correctos, abrir el menú principal
                        UserClass.Username = username;
                        MenuPrincipal menu = new MenuPrincipal(rol,username);
                        menu.Show();
                        this.Hide();
                    }
                    else
                    {
                        // Si no hay filas, significa que el login falló
                        MessageBox.Show("Usuario o contraseña incorrectos");
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
                finally
                {
                    conexion.Close();
                }
            }


        private string ObtenerRolDesdeBaseDeDatos(string username, string password)
        {
           
            string rol = null;

            using(SqlConnection conexion = new SqlConnection(connectionString))
            {
               conexion.Open();

                string query = "SELECT Rol FROM Usuarios WHERE NombreUsuario = @Usuario AND Contrasena = @Contrasena";
                using (SqlCommand command = new SqlCommand(query, conexion))
                {
                    command.Parameters.AddWithValue("@Usuario", username);
                    command.Parameters.AddWithValue("@Contrasena", password);

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        rol = reader["Rol"].ToString();
                    }
                }
            }

            return rol;
        }
        private void bunifuTextBox2_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Enter)
            {
                bunifuButton21.PerformClick();
            }
        }
    }
}
