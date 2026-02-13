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
    public partial class frmProveedores : Form
    {
        //el conection string waza XxD
        // string connectionString = "Data Source=DESKTOP-JDLBI3R\\SQLEXPRESS;Initial Catalog=SistemaInventario;User ID=sa;Password=1234;TrustServerCertificate=True";
        string connectionString = ConnectionClass.Conexion;
        // string connectionString = "Data Source=DESKTOP-VR5CCQE\\SQLEXPRESS;Initial Catalog=SistemaInventario;User ID=sa;Password=1935;TrustServerCertificate=True";
        public frmProveedores()
        {
            InitializeComponent();
            CargarDatosEnDataGridView();
            dgvProveedores.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvProveedores.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProveedores.ReadOnly = true;
        }
        private void CargarDatosEnDataGridView()
        {
            try
            {
                string query = "SELECT ID_Proveedor, Nombre_Proveedor, Contacto, Telefono, Correo_Electronico, Direccion, Productos_Servicios, Fecha_Registro, Estado FROM Proveedores";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();

                    adapter.Fill(dataTable);

                    if (dataTable.Rows.Count > 0)
                    {
                        dgvProveedores.AutoGenerateColumns = true;
                        dgvProveedores.DataSource = dataTable;
                        // Cambiar los títulos de las columnas
                        dgvProveedores.Columns["ID_Proveedor"].HeaderText = "ID del Proveedor";
                        dgvProveedores.Columns["Nombre_Proveedor"].HeaderText = "Nombre del Proveedor";
                        dgvProveedores.Columns["Contacto"].HeaderText = "Contacto";
                        dgvProveedores.Columns["Telefono"].HeaderText = "Telefono";
                        dgvProveedores.Columns["Correo_Electronico"].HeaderText = "Correo Electronico";
                        dgvProveedores.Columns["Direccion"].HeaderText = "Direccion";
                        dgvProveedores.Columns["Productos_Servicios"].HeaderText = "Productos";
                        dgvProveedores.Columns["Fecha_Registro"].HeaderText = "Fecha de Registro";
                        dgvProveedores.Columns["Estado"].HeaderText = "Estado?";

                    }
                    else
                    {
                        MessageBox.Show("No se encontraron datos en la tabla Proveedores.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar datos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            frmAgregarProveedores frmagregarproveedore = new frmAgregarProveedores();
            iconButton1.Enabled = false;
            frmagregarproveedore.ShowDialog();
            CargarDatosEnDataGridView();
            // frmAgregarUsuario.UsuarioAgregado += CargarDatosEnDataGridView;
            iconButton1.Enabled = true;
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            // Verificar si hay una fila seleccionada en el DataGridView
            if (dgvProveedores.SelectedRows.Count == 0)
            {
                MessageBox.Show("Por favor, selecciona un Proveedor para editar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        
            // Obtener los datos de la fila seleccionada
            DataGridViewRow filaSeleccionada = dgvProveedores.SelectedRows[0];
            int usuarioId = Convert.ToInt32(filaSeleccionada.Cells["ID_Proveedor"].Value);
            string nombre = filaSeleccionada.Cells["Nombre_Proveedor"].Value.ToString();
            string contacto = filaSeleccionada.Cells["Contacto"].Value.ToString();
            string telefono = filaSeleccionada.Cells["Telefono"].Value.ToString();
            string correoelectronico = filaSeleccionada.Cells["Correo_Electronico"].Value.ToString();
            string producto = filaSeleccionada.Cells["Productos_Servicios"].Value.ToString();
            string direccion = filaSeleccionada.Cells["Direccion"].Value.ToString();
            string estado = filaSeleccionada.Cells["Estado"].Value.ToString();


            // Abrir el formulario de edición y pasarle los datos
            frmEditarProveedores frmEditar = new frmEditarProveedores(usuarioId, nombre, contacto, telefono, correoelectronico, producto, direccion, estado);
            iconButton2.Enabled = false;
            frmEditar.ShowDialog();
            CargarDatosEnDataGridView();
            iconButton2.Enabled = true;
            // Recargar los datos del DataGridView después de cerrar el formulario
      
        }

        private void iconButton3_Click(object sender, EventArgs e)
        {
            iconButton3.Enabled = false;
            // Verificar si hay filas en el DataGridView
            if (dgvProveedores.Rows.Count == 0)
            {
                MessageBox.Show("No hay Proveedores para eliminar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Verificar si se seleccionó alguna fila
            if (dgvProveedores.SelectedRows.Count == 0)
            {
                MessageBox.Show("Por favor, seleccione un Proveedor para eliminar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Obtener el índice de la fila seleccionada
            int indiceSeleccionado = dgvProveedores.SelectedRows[0].Index;

            // Verificar si el valor de "Id" es nulo o DBNull
            object idValor = dgvProveedores.SelectedRows[0].Cells["ID_Proveedor"].Value;

            if (idValor == null || idValor == DBNull.Value)
            {
                MessageBox.Show("El Proveedor seleccionado no tiene un ID válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Convertir el valor de "Id" a entero
            int proveedorid;
            try
            {
                proveedorid = Convert.ToInt32(idValor);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al convertir el ID del Proveedor: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Confirmación antes de eliminar
            DialogResult confirmacion = MessageBox.Show("¿Está seguro de que desea eliminar este Proovedor?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
                    string query = "DELETE FROM Proveedores WHERE ID_Proveedor = @Id";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", proveedorid);
                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Proveedor eliminado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Limpiar selección y recargar datos
                            dgvProveedores.ClearSelection();
                            CargarDatosEnDataGridView();

                            // Restaurar la selección después de actualizar el DataGridView
                            if (dgvProveedores.Rows.Count > 0)
                            {
                                int nuevoIndice = Math.Min(indiceSeleccionado, dgvProveedores.Rows.Count - 1);
                                dgvProveedores.Rows[nuevoIndice].Selected = true;
                                dgvProveedores.CurrentCell = dgvProveedores.Rows[nuevoIndice].Cells[0];
                            }
                        }
                        else
                        {
                            MessageBox.Show("No se pudo eliminar al Proveedor. Intente nuevamente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                // Manejo de excepciones específicas de SQL
                MessageBox.Show($"Error al intentar eliminar al Proveedor en la base de datos: {sqlEx.Message}", "Error de Base de Datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                // Manejo de excepciones generales
                MessageBox.Show($"Error al intentar eliminar al Proveedor: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                iconButton3.Enabled = true;
            }
        }

        private void frmProveedores_Load(object sender, EventArgs e)
        {

        }
    }
}
