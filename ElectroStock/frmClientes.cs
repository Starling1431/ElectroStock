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

namespace Studium
{
    public partial class frmClientes : Form
    {
        string connectionString = ConnectionClass.Conexion;
        public frmClientes()
        {
            InitializeComponent();
            CargarDatosEnDataGridView();
            dgvClientes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvClientes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvClientes.ReadOnly = true;
        }
        private void CargarDatosEnDataGridView()
        {
            try
            {
                // Consulta SQL para obtener los datos de la tabla Clientes
                string query = "SELECT ID_Cliente, Nombre, Apellido, Correo_Electronico, Telefono, Direccion, Fecha_Registro, Estado FROM Clientes";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();

                    // Llenar el DataTable con los datos de la consulta
                    adapter.Fill(dataTable);

                    if (dataTable.Rows.Count > 0)
                    {
                        dgvClientes.AutoGenerateColumns = true; // Generar automáticamente las columnas
                        dgvClientes.DataSource = dataTable;

                        // Cambiar los títulos de las columnas para que sean más descriptivos
                        dgvClientes.Columns["ID_Cliente"].HeaderText = "ID del Cliente";
                        dgvClientes.Columns["Nombre"].HeaderText = "Nombre";
                        dgvClientes.Columns["Apellido"].HeaderText = "Apellido";
                        dgvClientes.Columns["Correo_Electronico"].HeaderText = "Correo Electrónico";
                        dgvClientes.Columns["Telefono"].HeaderText = "Teléfono";
                        dgvClientes.Columns["Direccion"].HeaderText = "Dirección";
                        dgvClientes.Columns["Fecha_Registro"].HeaderText = "Fecha de Registro";
                        dgvClientes.Columns["Estado"].HeaderText = "Estado";
                    }
                    else
                    {
                        // Mostrar mensaje si no se encuentran datos en la tabla
                        MessageBox.Show("No se encontraron datos en la tabla Clientes.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                // Mostrar mensaje de error si ocurre una excepción
                MessageBox.Show($"Error al cargar datos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void frmClientes_Load(object sender, EventArgs e)
        {

        }

        private void dgvClientes_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            frmAgregarClientes frmAgregarCliente = new frmAgregarClientes();
            iconButton1.Enabled = false;
            frmAgregarCliente.ShowDialog();
            CargarDatosEnDataGridView();
            // frmAgregarUsuario.UsuarioAgregado += CargarDatosEnDataGridView;
            iconButton1.Enabled = true;
        }

        private void iconButton3_Click(object sender, EventArgs e)
        {
            iconButton3.Enabled = false;

            // Verificar si hay filas en el DataGridView
            if (dgvClientes.Rows.Count == 0)
            {
                MessageBox.Show("No hay Clientes para eliminar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Verificar si se seleccionó alguna fila
            if (dgvClientes.SelectedRows.Count == 0)
            {
                MessageBox.Show("Por favor, seleccione un Cliente para eliminar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Obtener el índice de la fila seleccionada
            int indiceSeleccionado = dgvClientes.SelectedRows[0].Index;

            // Verificar si el valor de "Id" es nulo o DBNull
            object idValor = dgvClientes.SelectedRows[0].Cells["ID_Cliente"].Value;

            if (idValor == null || idValor == DBNull.Value)
            {
                MessageBox.Show("El Cliente seleccionado no tiene un ID válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Convertir el valor de "Id" a entero
            int clienteId;
            try
            {
                clienteId = Convert.ToInt32(idValor);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al convertir el ID del Cliente: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Confirmación antes de eliminar
            DialogResult confirmacion = MessageBox.Show("¿Está seguro de que desea eliminar este Cliente?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmacion == DialogResult.No)
            {
                iconButton3.Enabled = true;
                return;  // Si el usuario selecciona "No", cancelamos la operación
            }

            try
            {
                // Conexión y eliminación en la base de datos
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM Clientes WHERE ID_Cliente = @Id";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", clienteId);
                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Cliente eliminado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Limpiar selección y recargar datos
                            dgvClientes.ClearSelection();
                            CargarDatosEnDataGridView();

                            // Restaurar la selección después de actualizar el DataGridView
                            if (dgvClientes.Rows.Count > 0)
                            {
                                int nuevoIndice = Math.Min(indiceSeleccionado, dgvClientes.Rows.Count - 1);
                                dgvClientes.Rows[nuevoIndice].Selected = true;
                                dgvClientes.CurrentCell = dgvClientes.Rows[nuevoIndice].Cells[0];
                            }
                        }
                        else
                        {
                            MessageBox.Show("No se pudo eliminar el Cliente. Intente nuevamente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                // Manejo de excepciones específicas de SQL
                MessageBox.Show($"Error al intentar eliminar el Cliente en la base de datos: {sqlEx.Message}", "Error de Base de Datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                // Manejo de excepciones generales
                MessageBox.Show($"Error al intentar eliminar el Cliente: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                iconButton3.Enabled = true;
            }

        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            // Verificar si hay una fila seleccionada en el DataGridView
            if (dgvClientes.SelectedRows.Count == 0)
            {
                MessageBox.Show("Por favor, selecciona un Cliente para editar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Obtener los datos de la fila seleccionada
            DataGridViewRow filaSeleccionada = dgvClientes.SelectedRows[0];
            int clienteId = Convert.ToInt32(filaSeleccionada.Cells["ID_Cliente"].Value);
            string nombre = filaSeleccionada.Cells["Nombre"].Value.ToString();
            string apellido = filaSeleccionada.Cells["Apellido"].Value.ToString();
            string telefono = filaSeleccionada.Cells["Telefono"].Value.ToString();
            string correoElectronico = filaSeleccionada.Cells["Correo_Electronico"].Value.ToString();
            string direccion = filaSeleccionada.Cells["Direccion"].Value.ToString();
            string estado = filaSeleccionada.Cells["Estado"].Value.ToString();

            // Abrir el formulario de edición y pasarle los datos
            frmEditarClientes frmEditar = new frmEditarClientes(clienteId, nombre, apellido, telefono, correoElectronico, direccion, estado);
            iconButton2.Enabled = false;
            frmEditar.ShowDialog();
            CargarDatosEnDataGridView();
            iconButton2.Enabled = true;
            // Recargar los datos del DataGridView después de cerrar el formulario
        }
    }
}
