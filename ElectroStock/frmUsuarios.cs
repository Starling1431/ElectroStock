using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;
using FontAwesome.Sharp;

namespace Studium
{
    public partial class frmUsuarios : Form
    {
        //el conection string waza XxD
        string connectionString = ConnectionClass.Conexion;
        string _usuarioiniciado;
        // string connectionString = "Data Source=DESKTOP-JDLBI3R\\SQLEXPRESS;Initial Catalog=SistemaInventario;User ID=sa;Password=1234;TrustServerCertificate=True";
        // string connectionString = "Data Source=DESKTOP-IHGJ1TS\\SQLEXPRESS;Initial Catalog=SistemaInventario;Integrated Security=True;TrustServerCertificate=True";
        //  string connectionString = "Data Source=DESKTOP-VR5CCQE\\SQLEXPRESS;Initial Catalog=SistemaInventario;User ID=sa;Password=1935;TrustServerCertificate=True";
        public frmUsuarios(string usuarioiniciado)
        {
            InitializeComponent();
            CargarDatosEnDataGridView();
            dgvUsuarios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvUsuarios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUsuarios.ReadOnly = true;
            _usuarioiniciado = usuarioiniciado;
            
        }

        private void CargarDatosEnDataGridView()
        {
            try
            {
                string query = "SELECT Id, NombreUsuario, Contrasena, FechaCreacion, ROL FROM Usuarios";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();

                    adapter.Fill(dataTable);

                    if (dataTable.Rows.Count > 0)
                    {
                        dgvUsuarios.AutoGenerateColumns = true;
                        dgvUsuarios.DataSource = dataTable;
                        // Cambiar los títulos de las columnas
                        dgvUsuarios.Columns["Id"].HeaderText = "ID del Usuario";
                        dgvUsuarios.Columns["NombreUsuario"].HeaderText = "Nombre de Usuario";
                        dgvUsuarios.Columns["FechaCreacion"].HeaderText = "Fecha de Creación";
                        dgvUsuarios.Columns["Rol"].HeaderText = "Rol del Usuario";
                  
                    }
                    else
                    {
                        MessageBox.Show("No se encontraron datos en la tabla Usuarios.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar datos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void frmUsuarios_Load(object sender, EventArgs e)
        {
            lblUsuario.Text = UserClass.Username;
            CargarDatosEnDataGridView();
        }


        // _________________________________________ //
        //                AGREGAR                   //
        // ----------------------------------------//

        private void iconButton1_Click(object sender, EventArgs e)
        {
         
            frmAgregarUsuario frmAgregarUsuario = new frmAgregarUsuario();
            btnAbrirAgregar.Enabled = false;
            frmAgregarUsuario.ShowDialog();
            CargarDatosEnDataGridView();
           // frmAgregarUsuario.UsuarioAgregado += CargarDatosEnDataGridView;
            btnAbrirAgregar.Enabled = true;
           
        }



        // _________________________________________ //
        //                 ELIMINAR                 //
        // ----------------------------------------//
        private void btnEliminar_Click(object sender, EventArgs e)
        {   
            
            btnEliminar.Enabled = false;
            // Verificar si hay filas en el DataGridView
            if (dgvUsuarios.Rows.Count == 0)
            {
                MessageBox.Show("No hay usuarios para eliminar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Verificar si se seleccionó alguna fila
            if (dgvUsuarios.SelectedRows.Count == 0)
            {
                MessageBox.Show("Por favor, seleccione un usuario para eliminar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Obtener el índice de la fila seleccionada
            int indiceSeleccionado = dgvUsuarios.SelectedRows[0].Index;
            DataGridViewRow filaSeleccionada = dgvUsuarios.SelectedRows[0];
            string nombreUsuario = filaSeleccionada.Cells["NombreUsuario"].Value.ToString();
            // Verificar si el valor de "Id" es nulo o DBNull
            object idValor = dgvUsuarios.SelectedRows[0].Cells["Id"].Value;

            if (idValor == null || idValor == DBNull.Value)
            {
                MessageBox.Show("El usuario seleccionado no tiene un ID válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Convertir el valor de "Id" a entero
            int usuarioId;
            try
            {
                usuarioId = Convert.ToInt32(idValor);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al convertir el ID del usuario: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Confirmación antes de eliminar
            DialogResult confirmacion = MessageBox.Show("¿Está seguro de que desea eliminar este usuario?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmacion == DialogResult.No)
            {
                btnEliminar.Enabled = true;
                return;  // Si el usuario selecciona "No", cancelamos la operación
            }
            if (nombreUsuario == _usuarioiniciado) {

                MessageBox.Show($"No se puede eliminar el Usuario con el que iniciaste sesion.", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnEliminar.Enabled = true;
                return;
            }
            try
            {
                // Conexión y eliminación en la base de datos
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM Usuarios WHERE Id = @Id";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", usuarioId);
                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Usuario eliminado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Limpiar selección y recargar datos
                            dgvUsuarios.ClearSelection();
                            CargarDatosEnDataGridView();

                            // Restaurar la selección después de actualizar el DataGridView
                            if (dgvUsuarios.Rows.Count > 0)
                            {
                                int nuevoIndice = Math.Min(indiceSeleccionado, dgvUsuarios.Rows.Count - 1);
                                dgvUsuarios.Rows[nuevoIndice].Selected = true;
                                dgvUsuarios.CurrentCell = dgvUsuarios.Rows[nuevoIndice].Cells[0];
                            }
                        }
                        else
                        {
                            MessageBox.Show("No se pudo eliminar el usuario. Intente nuevamente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                // Manejo de excepciones específicas de SQL
                MessageBox.Show($"Error al intentar eliminar el usuario en la base de datos: {sqlEx.Message}", "Error de Base de Datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                // Manejo de excepciones generales
                MessageBox.Show($"Error al intentar eliminar el usuario: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally {
                btnEliminar.Enabled = true;
            }
        }

        // _________________________________________ //
        //                 EDITAR                   //
        // ----------------------------------------//
        private void iconButton2_Click(object sender, EventArgs e)
        {
            // Verificar si hay una fila seleccionada en el DataGridView
            if (dgvUsuarios.SelectedRows.Count == 0)
            {
                MessageBox.Show("Por favor, selecciona un usuario para editar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Obtener los datos de la fila seleccionada
            DataGridViewRow filaSeleccionada = dgvUsuarios.SelectedRows[0];
            int usuarioId = Convert.ToInt32(filaSeleccionada.Cells["Id"].Value);
            string nombreUsuario = filaSeleccionada.Cells["NombreUsuario"].Value.ToString();
            string contrasena = filaSeleccionada.Cells["Contrasena"].Value.ToString();
          

            // Abrir el formulario de edición y pasarle los datos
            frmEditarUsuario frmEditar = new frmEditarUsuario(usuarioId, nombreUsuario, contrasena);
            iconButton2.Enabled = false;
            frmEditar.ShowDialog();
            iconButton2.Enabled = true;
            // Recargar los datos del DataGridView después de cerrar el formulario
            CargarDatosEnDataGridView();
        }

        private void dgvUsuarios_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvUsuarios.Columns[e.ColumnIndex].Name == "Contrasena" && e.Value != null)
            {
                // Muestra caracteres en lugar del texto real
                e.Value = new string('•', e.Value.ToString().Length); // Muestra un número de "•" igual al largo del texto
                e.FormattingApplied = true;
            }
        }

        private void dgvUsuarios_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
               
        }

        private void lblUsuario_Click(object sender, EventArgs e)
        {

        }
    }
}

