using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Studium
{
    public partial class frmProductos : Form
    {
        string connectionString = ConnectionClass.Conexion;
        public frmProductos()
        {
            InitializeComponent();
            CargarDatosEnDataGridView();
            dgvProductos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvProductos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProductos.ReadOnly = true;

        }
        private void CargarDatosEnDataGridView()
        {
            try
            {
                string query = @"SELECT p.ID, p.Nombre, p.Marca, p.Modelo, p.Descripcion, p.Precio, p.Stock, 
                         pr.Nombre_Proveedor AS Proveedor
                         FROM Productos p
                         INNER JOIN Proveedores pr ON p.Proveedor = pr.ID_Proveedor";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();

                    adapter.Fill(dataTable);

                    if (dataTable.Rows.Count > 0)
                    {
                        dgvProductos.AutoGenerateColumns = true;
                        dgvProductos.DataSource = dataTable;
                        // Cambiar los títulos de las columnas
                        dgvProductos.Columns["ID"].HeaderText = "ID del Producto";
                        dgvProductos.Columns["Nombre"].HeaderText = "Nombre del Producto";
                        dgvProductos.Columns["Marca"].HeaderText = "Marca";
                        dgvProductos.Columns["Modelo"].HeaderText = "Modelo";
                        dgvProductos.Columns["Descripcion"].HeaderText = "Descripcion";
                        dgvProductos.Columns["Precio"].HeaderText = "Precio";
                        dgvProductos.Columns["Stock"].HeaderText = "Stock";
                        dgvProductos.Columns["Proveedor"].HeaderText = "Proveedor";

                    }
                    else
                    {
                        MessageBox.Show("No se encontraron datos en la tabla Productos.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar datos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void frmProductos_Load(object sender, EventArgs e)
        {

        }

        private void iconButton4_Click(object sender, EventArgs e)
        {
            bunifuTextBox1.Text = "";
            if (dgvProductos.DataSource is DataTable dataTable)
            {
                dataTable.DefaultView.RowFilter = string.Empty; // Eliminar el filtro
            }
        }

        private void bunifuTextBox1_TextChanged(object sender, EventArgs e)
        {
            string filtro = bunifuTextBox1.Text;

            string query = "SELECT ID, Nombre, Marca, Modelo, Descripcion, Precio, Stock, FechaCreacion, Proveedor " +
                           "FROM Productos WHERE Nombre LIKE @Filtro OR Marca LIKE @Filtro OR Proveedor LIKE @Filtro";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                adapter.SelectCommand.Parameters.AddWithValue("@Filtro", $"%{filtro}%");
                DataTable dataTable = new DataTable();

                adapter.Fill(dataTable);

                dgvProductos.DataSource = dataTable;
            }
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {

            frmAgregarProducto frmAgregarProducto = new frmAgregarProducto();
            iconButton1.Enabled = false;
            frmAgregarProducto.ShowDialog();
            CargarDatosEnDataGridView();
            // frmAgregarUsuario.UsuarioAgregado += CargarDatosEnDataGridView;
            iconButton1.Enabled = true;
        }

        private void iconButton3_Click(object sender, EventArgs e)
        {
            iconButton3.Enabled = false;
            // Verificar si hay filas en el DataGridView
            if (dgvProductos.Rows.Count == 0)
            {
                MessageBox.Show("No hay Productos para eliminar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Verificar si se seleccionó alguna fila
            if (dgvProductos.SelectedRows.Count == 0)
            {
                MessageBox.Show("Por favor, seleccione un Producto para eliminar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Obtener el índice de la fila seleccionada
            int indiceSeleccionado = dgvProductos.SelectedRows[0].Index;

            // Verificar si el valor de "Id" es nulo o DBNull
            object idValor = dgvProductos.SelectedRows[0].Cells["ID"].Value;

            if (idValor == null || idValor == DBNull.Value)
            {
                MessageBox.Show("El Producto seleccionado no tiene un ID válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show($"Error al convertir el ID del Producto: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Confirmación antes de eliminar
            DialogResult confirmacion = MessageBox.Show("¿Está seguro de que desea eliminar este Producto?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
                    string query = "DELETE FROM Productos WHERE ID = @Id";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", proveedorid);
                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Proveedor eliminado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Limpiar selección y recargar datos
                            dgvProductos.ClearSelection();
                            CargarDatosEnDataGridView();

                            // Restaurar la selección después de actualizar el DataGridView
                            if (dgvProductos.Rows.Count > 0)
                            {
                                int nuevoIndice = Math.Min(indiceSeleccionado, dgvProductos.Rows.Count - 1);
                                dgvProductos.Rows[nuevoIndice].Selected = true;
                                dgvProductos.CurrentCell = dgvProductos.Rows[nuevoIndice].Cells[0];
                            }
                        }
                        else
                        {
                            MessageBox.Show("No se pudo eliminar el Prducto. Intente nuevamente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                // Manejo de excepciones específicas de SQL
                MessageBox.Show($"Error al intentar eliminar el Producto en la base de datos: {sqlEx.Message}", "Error de Base de Datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                // Manejo de excepciones generales
                MessageBox.Show($"Error al intentar eliminar el Producto: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                iconButton3.Enabled = true;
            }
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            // Verificar si hay una fila seleccionada en el DataGridView
            if (dgvProductos.SelectedRows.Count == 0)
            {
                MessageBox.Show("Por favor, selecciona un Producto para editar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Obtener los datos de la fila seleccionada
            DataGridViewRow filaSeleccionada = dgvProductos.SelectedRows[0];
            int productoId = Convert.ToInt32(filaSeleccionada.Cells["ID"].Value);
            string nombreProducto = filaSeleccionada.Cells["Nombre"].Value.ToString();
            string marca = filaSeleccionada.Cells["Marca"].Value.ToString();
            string modelo = filaSeleccionada.Cells["Modelo"].Value.ToString();
            string descripcion = filaSeleccionada.Cells["Descripcion"].Value.ToString();
            double precio = Convert.ToDouble(filaSeleccionada.Cells["Precio"].Value);
            int stock = Convert.ToInt32(filaSeleccionada.Cells["Stock"].Value);
            string proveedor = filaSeleccionada.Cells["Proveedor"].Value.ToString();

            // Abrir el formulario de edición y pasarle los datos
            frmEditarProductos frmEditar = new frmEditarProductos(productoId, nombreProducto, marca, modelo, descripcion, precio, stock, proveedor);
            iconButton2.Enabled = false;
            frmEditar.ShowDialog();
            CargarDatosEnDataGridView();
            iconButton2.Enabled = true;
            // Recargar los datos del DataGridView después de cerrar el formulario

        }

        private void dgvProductos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
