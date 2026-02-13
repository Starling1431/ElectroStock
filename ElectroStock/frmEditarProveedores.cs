using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using System.Windows.Forms;
using Bunifu.UI.WinForms;

namespace Studium
{
    public partial class frmEditarProveedores : Form
    {
        string connectionString = ConnectionClass.Conexion;
        private int _usuarioId;
        public frmEditarProveedores(int usuarioId, string nombre, string contacto, string telefono, string correoelectronico, string producto, string direccion, string estado)
        {
            _usuarioId = usuarioId;
            InitializeComponent();
            bunifuTextBox1.Text = nombre;
            bunifuTextBox2.Text = contacto;
            txtTelefono.Text = telefono;
            bunifuTextBox4.Text = correoelectronico;
            bunifuTextBox6.Text = producto;
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

        private void txtTelefono_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }

        private void bunifuTextBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            string estadoo = comboBox1.SelectedItem.ToString();

            if (string.IsNullOrWhiteSpace(bunifuTextBox1.Text) || string.IsNullOrWhiteSpace(bunifuTextBox2.Text)
              || string.IsNullOrWhiteSpace(txtTelefono.Text) || string.IsNullOrWhiteSpace(bunifuTextBox4.Text)
              || string.IsNullOrWhiteSpace(bunifuTextBox5.Text) || string.IsNullOrWhiteSpace(bunifuTextBox6.Text) ||
              string.IsNullOrWhiteSpace(estadoo))
            {
                MessageBox.Show("Por favor, complete todos los campos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

         
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Consulta SQL para actualizar el usuario en la base de datos.@Nombre_Proveedor, @Contacto, @Telefono, @Correo_Electronico, @Direccion, @Productos_Servicios, @Estado
                    string query = "UPDATE Proveedores SET Nombre_Proveedor = @Nombre_Proveedor, Contacto = @Contacto, Telefono = @Telefono, Correo_Electronico = @Correo_Electronico,Direccion = @Direccion, Productos_Servicios = @Productos_Servicios, Estado = @Estado WHERE ID_Proveedor = @Id";
                    SqlCommand command = new SqlCommand(query, connection);
                 
                    // Agregar los parámetros de la consulta con los valores de los controles del formulario.
                    command.Parameters.AddWithValue("@Nombre_Proveedor", bunifuTextBox1.Text);
                    command.Parameters.AddWithValue("@Contacto", bunifuTextBox2.Text);
                    command.Parameters.AddWithValue("@Telefono", txtTelefono.Text);
                    command.Parameters.AddWithValue("@Correo_Electronico", bunifuTextBox4.Text);
                    command.Parameters.AddWithValue("@Direccion", bunifuTextBox5.Text);
                    command.Parameters.AddWithValue("@Productos_Servicios", bunifuTextBox6.Text);
                    command.Parameters.AddWithValue("@Estado", estadoo);

                    command.Parameters.AddWithValue("@Id", _usuarioId);

                    // Abrir la conexión y ejecutar la consulta.
                    connection.Open();
                    int result = command.ExecuteNonQuery();

                    if (result > 0)
                    {
                        // Si la actualización fue exitosa, mostrar un mensaje y cerrar el formulario.
                        MessageBox.Show("Proveedor actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                    }
                    else
                    {
                        // Si la actualización no fue exitosa, mostrar un mensaje de error.
                        MessageBox.Show("Error al actualizar al Proveedor.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                // Si ocurre un error en la actualización, mostrar el mensaje de excepción.
                MessageBox.Show($"Error al actualizar al Proveedor: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtTelefono_Click(object sender, EventArgs e)
        {
            txtTelefono.SelectionStart = 0;
            txtTelefono.SelectionLength = 0;
        }
    }
}

