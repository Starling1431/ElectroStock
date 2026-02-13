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
    public partial class frmEditarUsuario : Form
    {
        //el conection string waza XxD
        string connectionString = ConnectionClass.Conexion;
        // string connectionString = "Data Source=DESKTOP-JDLBI3R\\SQLEXPRESS;Initial Catalog=SistemaInventario;User ID=sa;Password=1234;TrustServerCertificate=True";
        // string connectionString = "Data Source=DESKTOP-IHGJ1TS\\SQLEXPRESS;Initial Catalog=SistemaInventario;Integrated Security=True;TrustServerCertificate=True";
        //  string connectionString = "Data Source=DESKTOP-VR5CCQE\\SQLEXPRESS;Initial Catalog=SistemaInventario;User ID=sa;Password=1935;TrustServerCertificate=True";

        private int usuarioId;
       
        public frmEditarUsuario(int id, string nombreUsuario, string contrasena)
        {
           
            InitializeComponent();  // Inicializar los componentes del formulario.
            usuarioId = id;  // Asignar el ID del usuario al campo de la clase.
            CargarRoles();   
            bunifuTextBox1.Text = nombreUsuario; // Llenar el campo de NombreUsuario
            bunifuTextBox2.Text = contrasena; // Llenar el campo de Rol
           
       
        }


        private void iconButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void bunifuPanel2_Click(object sender, EventArgs e)
        {

        }
        private void frmEditarUsuario_Load(object sender, EventArgs e)
        {
         
            CargarDatosUsuario();
        }

        private void CargarDatosUsuario()
        {
            
            try
            {
                // Establecer la conexión a la base de datos.
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Consulta SQL para obtener los datos del usuario por su ID.
                    string query = "SELECT Id, NombreUsuario, FechaCreacion, Rol FROM Usuarios WHERE Id = @Id";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Id", usuarioId);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    // Si se encuentra el usuario, cargar los datos en los controles del formulario.
                    if (dataTable.Rows.Count > 0)
                    {
                        DataRow row = dataTable.Rows[0];

                        bunifuTextBox1.Text = row["NombreUsuario"].ToString(); // Cargar NombreUsuario
                        bunifuTextBox2.Text = row["Contrasena"].ToString(); // Cargar Contrasena
                      
                        
                    }
                    else
                    {
                        MessageBox.Show("Usuario no encontrado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                // Si ocurre un error, mostrar el mensaje de excepción.
                MessageBox.Show($"Error al cargar los datos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void CargarRoles() {

            // Agregar roles al ComboBox
            cbRol.Items.Add("Administrador");
            cbRol.Items.Add("Empleado");
            cbRol.SelectedIndex = 0; // Selección por defecto
         
        }

   
        private void ActualizarUsuario()
        {


            string roles = cbRol.SelectedItem.ToString();

            if (string.IsNullOrWhiteSpace(bunifuTextBox1.Text) || string.IsNullOrWhiteSpace(bunifuTextBox2.Text)
               || string.IsNullOrWhiteSpace(roles)) { 

                MessageBox.Show("Por favor, complete todos los campos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Consulta SQL para actualizar el usuario en la base de datos.
                    string query = "UPDATE Usuarios SET NombreUsuario = @NombreUsuario, Rol = @Rol, Contrasena = @Contrasena WHERE Id = @Id";
                    SqlCommand command = new SqlCommand(query, connection);

                    // Agregar los parámetros de la consulta con los valores de los controles del formulario.
                    command.Parameters.AddWithValue("@NombreUsuario", bunifuTextBox1.Text);
                    command.Parameters.AddWithValue("@Contrasena", bunifuTextBox2.Text);
                    command.Parameters.AddWithValue("@ROL", roles);
              
                    command.Parameters.AddWithValue("@Id", usuarioId);

                    // Abrir la conexión y ejecutar la consulta.
                    connection.Open();
                    int result = command.ExecuteNonQuery();

                    if (result > 0)
                    {
                        // Si la actualización fue exitosa, mostrar un mensaje y cerrar el formulario.
                        MessageBox.Show("Usuario actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                    }
                    else
                    {
                        // Si la actualización no fue exitosa, mostrar un mensaje de error.
                        MessageBox.Show("Error al actualizar el usuario.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                // Si ocurre un error en la actualización, mostrar el mensaje de excepción.
                MessageBox.Show($"Error al actualizar el usuario: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
         
            ActualizarUsuario();
        }

        private void bunifuTextBox2_TextChanged(object sender, EventArgs e)
        {
           
        }

        private void bunifuLabel5_Click(object sender, EventArgs e)
        {

        }
    }
}
