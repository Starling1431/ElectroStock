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
    public partial class frmVentas : Form
    {
        // Cadena de conexión a la base de datos
        string connectionString = ConnectionClass.Conexion;

        // Variables para el precio y el stock del producto seleccionado
        private double _precioProducto;
        private int _stockProducto;
        string _cliente;
        
        private int pedidoCounter = 1; // Contador para IDs únicos de pedidos
        public frmVentas()
        {
            InitializeComponent();
            btnAgregarProducto.Click += btnAgregarProducto_Click;
            dgvVentas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvVentas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvVentas.ReadOnly = true;
        }
        public int GetRowCount()
        {
            return dgvVentas.Rows.Count;
        }
     
        private void CargarProductos()
        {
            try
            {
                string query = "SELECT ID, Nombre FROM Productos";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    if (dataTable.Rows.Count > 0)
                    {
                        cmbProductos.DisplayMember = "Nombre"; // Lo que se mostrará en el ComboBox
                        cmbProductos.ValueMember = "ID"; // El valor asociado al ítem
                        cmbProductos.DataSource = dataTable; // Asigna los datos al ComboBox
                    }
                    else
                    {
                        MessageBox.Show("No hay productos disponibles.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los productos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Método para cargar los clientes en el ComboBox
        private void CargarClientes()
        {
            try
            {
                string query = "SELECT ID_Cliente, Nombre, Estado FROM Clientes WHERE Estado = 'Activo'";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    if (dataTable.Rows.Count > 0)
                    {
                        comboBoxClientes.DisplayMember = "Nombre"; // Lo que se mostrará en el ComboBox
                        comboBoxClientes.ValueMember = "ID_Cliente"; // El valor asociado al ítem
                        comboBoxClientes.DataSource = dataTable; // Asigna los datos al ComboBox
                     
                    }
                    else
                    {
                        MessageBox.Show("No hay clientes disponibles.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los clientes: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Método para cargar la fecha actual en el Label
        private void CargarFecha()
        {
            lblFecha.Text = DateTime.Now.ToString("dd/MM/yyyy");
        }

        // Evento que se ejecuta cuando se selecciona un producto
        private void cmbProductos_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        // Método para agregar el producto al DataGridView
        private void btnAgregarProducto_Click(object sender, EventArgs e)
        {

        }

        // Método para actualizar el stock en la base de datos
        private void ActualizarStock(int productoId, int nuevoStock)
        {
            try
            {
                string query = "UPDATE Productos SET Stock = @Stock WHERE ID = @ProductoId";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@ProductoId", productoId);
                    command.Parameters.AddWithValue("@Stock", nuevoStock);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar el stock: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ConfigurarColumnasDGV()
        {
            // Configurar las columnas del DataGridView si no están configuradas ya.
            if (dgvVentas.Columns.Count == 0)
            {
                dgvVentas.Columns.Add("id", "ID Del Producto");
                dgvVentas.Columns.Add("Producto", "Producto");
                dgvVentas.Columns.Add("Cantidad", "Cantidad");
                dgvVentas.Columns.Add("Precio", "Precio");
                dgvVentas.Columns.Add("Total", "Total");
            }
        }
        // Evento Load del formulario
        private void frmVentas_Load(object sender, EventArgs e)
        {
            ConfigurarColumnasDGV();
            CargarProductos();
            CargarClientes();
            CargarFecha();
        }
        private void CalcularTotalVenta()
        {
            double totalVenta = 0;

            // Recorre todas las filas del DataGridView
            foreach (DataGridViewRow row in dgvVentas.Rows)
            {
                // Evita contar la fila vacía (nueva)
                if (row.IsNewRow)
                    continue;

                // Obtiene el valor de la columna "Total" de cada fila
                double totalProducto = Convert.ToDouble(row.Cells["Total"].Value);

                // Suma el total del producto al total general de la venta
                totalVenta += totalProducto;
            }

            // Muestra el total en el Label
            txtVentaTotal.Text = totalVenta.ToString("C2");  // El formato "C2" es para mostrar en formato moneda
        }
        private void btnAgregarProducto_Click_1(object sender, EventArgs e)
        {
            if (cmbProductos.SelectedItem != null && nudCantidad.Value > 0)
            {
                int productoId = Convert.ToInt32(cmbProductos.SelectedValue);
                string productoSeleccionado = cmbProductos.Text; // Nombre del producto seleccionado
                int cantidad = (int)nudCantidad.Value;

                // Calcular el total (precio * cantidad)
                double total = _precioProducto * cantidad;

                // Verificar si hay suficiente stock
                if (cantidad > _stockProducto)
                {
                    MessageBox.Show("No hay suficiente stock disponible.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Agregar el producto al DataGridView
                dgvVentas.Rows.Add(productoId, productoSeleccionado, cantidad, _precioProducto, total);

                // Actualizar el stock restante si es necesario
                _stockProducto -= cantidad;  // Reducir el stock disponible
                ActualizarStock(productoId, _stockProducto);

                // Actualizar el precio total de la venta
                CalcularTotalVenta();
            }
            else
            {
                MessageBox.Show("Por favor, seleccione un producto y una cantidad válida.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void cmbProductos_SelectedIndexChanged_1(object sender, EventArgs e)
        {
           
        }

        private void iconButton6_Click(object sender, EventArgs e)
        {
            
        }

        private void RestaurarStock(int productoId, int cantidad)
        {
            try
            {
                // Recuperar el stock original del producto desde la base de datos
                string query = "SELECT Stock FROM Productos WHERE ID = @ProductoId";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Primera consulta para obtener el stock original
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@ProductoId", productoId);

                    int stockOriginal = 0;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            stockOriginal = Convert.ToInt32(reader["Stock"]);
                        }
                    } // Aquí se cierra el DataReader automáticamente

                    // Calcular el nuevo stock
                    int nuevoStock = stockOriginal + cantidad;

                    // Segunda consulta para actualizar el stock
                    string updateQuery = "UPDATE Productos SET Stock = @NuevoStock WHERE ID = @ProductoId";
                    SqlCommand updateCommand = new SqlCommand(updateQuery, connection);
                    updateCommand.Parameters.AddWithValue("@ProductoId", productoId);
                    updateCommand.Parameters.AddWithValue("@NuevoStock", nuevoStock);

                    updateCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al restaurar el stock: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
           
        }

        private void dgvVentas_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void iconButton5_Click(object sender, EventArgs e)
        {
            // Verifica si hay alguna fila seleccionada
            if (dgvVentas.SelectedRows.Count > 0)
            {
                // Obtiene la fila seleccionada
                DataGridViewRow row = dgvVentas.SelectedRows[0];

                // Verifica si la fila es nueva (sin confirmar)
                if (row.IsNewRow)
                {
                    MessageBox.Show("No se puede modificar una fila nueva que no ha sido confirmada.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Obtén la cantidad actual de la fila seleccionada
                int cantidadActual = Convert.ToInt32(row.Cells["Cantidad"].Value);

                // Aumenta la cantidad en 1 (o el número que desees sumar)
                int nuevaCantidad = cantidadActual + 1;

                // Obtiene el nombre del producto y el precio
                string productoSeleccionado = row.Cells["Producto"].Value.ToString();
                double precioProducto = Convert.ToDouble(row.Cells["Precio"].Value);

                // Calcula el nuevo total (precio * nueva cantidad)
                double nuevoTotal = precioProducto * nuevaCantidad;

                // Verificar si hay suficiente stock antes de actualizar
                int productoId = GetProductoId(productoSeleccionado); // Asume que tienes una función para obtener el ID del producto
                int stockDisponible = GetStockDisponible(productoId); // Asume que tienes una función para obtener el stock

                if (nuevaCantidad > stockDisponible)
                {
                    MessageBox.Show("No hay suficiente stock disponible.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Actualiza la cantidad en la fila
                row.Cells["Cantidad"].Value = nuevaCantidad;

                // Actualiza el total en la fila
                row.Cells["Total"].Value = nuevoTotal;

                // Actualiza el stock en la base de datos
                ActualizarStock(productoId, stockDisponible - 1); // Resta 1 del stock disponible en la base de datos
            }
            else
            {
                MessageBox.Show("Por favor, selecciona una fila para modificar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        // Función para obtener el ID del producto
        private int GetProductoId(string nombreProducto)
        {
            int productoId = 0;
            string query = "SELECT ID FROM Productos WHERE Nombre = @NombreProducto";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@NombreProducto", nombreProducto);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    productoId = Convert.ToInt32(reader["ID"]);
                }
            }

            return productoId;
        }

        // Función para obtener el stock disponible de un producto
        private int GetStockDisponible(int productoId)
        {
            int stockDisponible = 0;
            string query = "SELECT Stock FROM Productos WHERE ID = @ProductoId";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ProductoId", productoId);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    stockDisponible = Convert.ToInt32(reader["Stock"]);
                }
            }

            return stockDisponible;
        }

        private void iconButton4_Click(object sender, EventArgs e)
        {

        
        }

        private void bunifuLabel5_Click(object sender, EventArgs e)
        {

        }

        private void bunifuLabel6_Click(object sender, EventArgs e)
        {

        }

        private void cmbProductos_SelectedIndexChanged_2(object sender, EventArgs e)
        {
            if (cmbProductos.SelectedItem != null)
            {
                int productoId = Convert.ToInt32(cmbProductos.SelectedValue); // Obtener el ID del producto seleccionado

                try
                {
                    string query = "SELECT Precio, Stock FROM Productos WHERE ID = @ProductoId";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@ProductoId", productoId);

                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            _precioProducto = Convert.ToDouble(reader["Precio"]);
                            _stockProducto = Convert.ToInt32(reader["Stock"]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al obtener el precio y stock: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void iconButton2_Click_1(object sender, EventArgs e)
        {
            int productoId = Convert.ToInt32(cmbProductos.SelectedValue);
            if (dgvVentas.SelectedRows.Count > 0)
            {
                // Obtiene la fila seleccionada
                DataGridViewRow row = dgvVentas.SelectedRows[0];

                // Verifica si la fila es una fila nueva
                if (row.IsNewRow)
                {
                    MessageBox.Show("No se puede eliminar una fila nueva.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Obtiene el total de la fila seleccionada
                double totalProducto = Convert.ToDouble(row.Cells["Total"].Value);
                _stockProducto += 1;  // Reducir el stock disponible
                ActualizarStock(productoId, _stockProducto);
                // Elimina la fila del DataGridView
                dgvVentas.Rows.Remove(row);

                // Actualizar el precio total de la venta después de eliminar un producto
                CalcularTotalVenta();
            }
            else
            {
                MessageBox.Show("Por favor, selecciona una fila para eliminar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
        }

        private void iconButton5_Click_1(object sender, EventArgs e)
        {
            // Verifica si hay alguna fila seleccionada
            if (dgvVentas.SelectedRows.Count > 0)
            {
                // Obtiene la fila seleccionada
                DataGridViewRow row = dgvVentas.SelectedRows[0];

                // Verifica si la fila es nueva (sin confirmar)
                if (row.IsNewRow)
                {
                    MessageBox.Show("No se puede modificar una fila nueva que no ha sido confirmada.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Obtén la cantidad actual de la fila seleccionada
                int cantidadActual = Convert.ToInt32(row.Cells["Cantidad"].Value);

                // Aumenta la cantidad en 1 (o el número que desees sumar)
                int nuevaCantidad = cantidadActual + 1;

                // Obtiene el nombre del producto y el precio
                string productoSeleccionado = row.Cells["Producto"].Value.ToString();
                double precioProducto = Convert.ToDouble(row.Cells["Precio"].Value);

                // Calcula el nuevo total (precio * nueva cantidad)
                double nuevoTotal = precioProducto * nuevaCantidad;

                // Verificar si hay suficiente stock antes de actualizar
                int productoId = GetProductoId(productoSeleccionado); // Asume que tienes una función para obtener el ID del producto
                int stockDisponible = GetStockDisponible(productoId); // Asume que tienes una función para obtener el stock

                if (nuevaCantidad > stockDisponible)
                {
                    MessageBox.Show("No hay suficiente stock disponible.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Actualiza la cantidad en la fila
                row.Cells["Cantidad"].Value = nuevaCantidad;

                // Actualiza el total en la fila
                row.Cells["Total"].Value = nuevoTotal;

                CalcularTotalVenta();

                // Actualiza el stock en la base de datos
                ActualizarStock(productoId, stockDisponible - 1); // Resta 1 del stock disponible en la base de datos
            }
            else
            {
                MessageBox.Show("Por favor, selecciona una fila para modificar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void iconButton4_Click_1(object sender, EventArgs e)
        {
            // Verifica si hay alguna fila seleccionada
            if (dgvVentas.SelectedRows.Count > 0)
            {
                // Obtiene la fila seleccionada
                DataGridViewRow row = dgvVentas.SelectedRows[0];

                // Verifica si la fila es nueva (sin confirmar)
                if (row.IsNewRow)
                {
                    MessageBox.Show("No se puede modificar una fila nueva que no ha sido confirmada.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Obtén la cantidad actual de la fila seleccionada
                int cantidadActual = Convert.ToInt32(row.Cells["Cantidad"].Value);

                // Asegúrate de que la cantidad sea mayor que 1, para no restar a 0 o a un número negativo
                if (cantidadActual <= 1)
                {
                    MessageBox.Show("No puedes restar más, la cantidad no puede ser menor a 1.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Resta la cantidad en 1
                int nuevaCantidad = cantidadActual - 1;
             
                // Obtiene el nombre del producto y el precio
                string productoSeleccionado = row.Cells["Producto"].Value.ToString();
                double precioProducto = Convert.ToDouble(row.Cells["Precio"].Value);

                // Calcula el nuevo total (precio * nueva cantidad)
                double nuevoTotal = precioProducto * nuevaCantidad;

                // Obtén el ID del producto y el stock disponible
                int productoId = GetProductoId(productoSeleccionado); // Asume que tienes una función para obtener el ID del producto
                int stockDisponible = GetStockDisponible(productoId); // Asume que tienes una función para obtener el stock

                // Si es necesario, actualizar el stock de la base de datos
                if (stockDisponible > 0)
                {
                    // Actualiza la cantidad en la fila
                    row.Cells["Cantidad"].Value = nuevaCantidad;

                    // Actualiza el total en la fila
                    row.Cells["Total"].Value = nuevoTotal;
                    CalcularTotalVenta();
                    // Actualiza el stock en la base de datos
                    ActualizarStock(productoId, stockDisponible + 1); // Aumenta 1 al stock disponible en la base de datos
                }
                else
                {
                    MessageBox.Show("No hay suficiente stock para restar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecciona una fila para modificar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void iconButton6_Click_1(object sender, EventArgs e)
        {
            // Confirmación antes de eliminar
            DialogResult confirmacion = MessageBox.Show("¿Está seguro de que desea Cancelar la Venta?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmacion == DialogResult.No)
            {
                return;  // Si el usuario selecciona "No", cancelamos la operación
            }
            // Recorrer todas las filas del DataGridView
            foreach (DataGridViewRow row in dgvVentas.Rows)
            {
                if (row.IsNewRow) continue; // Ignorar la nueva fila (vacía)

                // Obtener el ID del producto (puedes agregarlo en alguna columna del DataGridView al momento de agregar productos)
                int productoId = Convert.ToInt32(row.Cells["ID"].Value);

                // Obtener la cantidad que se agregó para este producto
                int cantidad = Convert.ToInt32(row.Cells["Cantidad"].Value);

                // Recuperar el stock original del producto
                RestaurarStock(productoId, cantidad);
            }

            // Limpiar el DataGridView
            dgvVentas.Rows.Clear();

            // Limpiar el total de la venta
            txtVentaTotal.Text = "$0.00";
        }

        private void cmbProductos_Click(object sender, EventArgs e)
        {

        }

        private void iconButton3_Click(object sender, EventArgs e)
        {
            int idpedido = 0;
            
            // Verifica si se ha seleccionado un cliente
            if (comboBoxClientes.SelectedItem == null)
            {
                MessageBox.Show("Por favor, seleccione un cliente.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Si está usando un DataTable o lista de objetos, usa ValueMember para obtener el valor correcto
            string clienteSeleccionado = (comboBoxClientes.SelectedItem as DataRowView)?["Nombre"].ToString() ?? "";

            if (string.IsNullOrEmpty(clienteSeleccionado))
            {
                MessageBox.Show("No se pudo obtener el nombre del cliente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string totalVenta = txtVentaTotal.Text;
            idpedido++;
            // Asegurarse de que la fecha sea válida y esté en el formato correcto
            DateTime fechaConvertida;
            if (!DateTime.TryParse(lblFecha.Text, out fechaConvertida))
            {
                MessageBox.Show("La fecha ingresada no tiene un formato válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string fechaFormateada = fechaConvertida.ToString("yyyy-MM-dd");

            // Insertar datos en la tabla Ventas
            using (SqlConnection connection = new SqlConnection(ConnectionClass.Conexion))
            {
                try
                {
                    connection.Open();

                    // Recorre el DataGridView para obtener los datos y registrarlos en la base de datos
                    foreach (DataGridViewRow row in dgvVentas.Rows)
                    {
                        if (!row.IsNewRow) // Evitar las filas vacías
                        {
                            // Obtén los valores de la fila actual
                            int idProducto = Convert.ToInt32(row.Cells["ID"].Value);          // ID del producto
                            int cantidad = Convert.ToInt32(row.Cells["Cantidad"].Value);      // Cantidad
                            decimal precioUnitario = Convert.ToDecimal(row.Cells["Precio"].Value); // Precio unitario
                            decimal total = Convert.ToDecimal(row.Cells["Total"].Value);      // Total

                            // Inserción en la tabla Ventas
                            string query = @"
                    INSERT INTO Ventas (ID_Cliente, ID_Producto, Cantidad, Precio_Unitario, Total, Fecha_Venta, ID_Pedido)
                    VALUES (
                        (SELECT ID_Cliente FROM Clientes WHERE Nombre = @ClienteNombre),
                        @ID_Producto,
                        @Cantidad,
                        @Precio_Unitario,
                        @Total,
                        @Fecha_Venta,
                        @ID_Pedido)";

                            using (SqlCommand command = new SqlCommand(query, connection))
                            {
                                // Añadir parámetros
                                command.Parameters.AddWithValue("@ClienteNombre", string.IsNullOrEmpty(clienteSeleccionado) ? DBNull.Value : (object)clienteSeleccionado);
                                command.Parameters.AddWithValue("@ID_Producto", idProducto);
                                command.Parameters.AddWithValue("@Cantidad", cantidad);
                                command.Parameters.AddWithValue("@Precio_Unitario", precioUnitario == 0 ? DBNull.Value : (object)precioUnitario);
                                command.Parameters.AddWithValue("@Total", total == 0 ? DBNull.Value : (object)total);
                                command.Parameters.AddWithValue("@Fecha_Venta", string.IsNullOrEmpty(fechaFormateada) ? DBNull.Value : (object)fechaFormateada);
                                command.Parameters.AddWithValue("@ID_Pedido", idpedido);
                                // Ejecutar la consulta
                                command.ExecuteNonQuery();
                            }
                        }
                    }

                    MessageBox.Show("Las ventas se han registrado correctamente en la base de datos.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al registrar la venta: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            // Convertir la información de dgvVentas a una lista de pedidos
            foreach (DataGridViewRow row in dgvVentas.Rows)
            {
                if (!row.IsNewRow) // Evitar las filas vacías
                {
                    Pedido nuevoPedido = new Pedido
                    {
                        Cliente = clienteSeleccionado,
                        Producto = row.Cells["Producto"].Value.ToString(),
                        PedidoID = PedidoManager.ObtenerNuevoPedidoID(),
                        Cantidad = Convert.ToInt32(row.Cells["Cantidad"].Value),
                        Precio = Convert.ToDouble(row.Cells["Precio"].Value),
                        Total = Convert.ToDouble(row.Cells["Total"].Value),
                        VentaTotal = totalVenta,
                        Fecha = fechaFormateada
                    };
                    PedidoManager.PedidosFinalizados.Add(nuevoPedido);
                }
            }

            // Incrementar el contador de pedidos
            PedidoManager.ContadorPedidos++;
        

            // Abrir frmReportes dentro del panel de MenuPrincipal
            var menuPrincipal = Application.OpenForms["MenuPrincipal"] as MenuPrincipal;
            if (menuPrincipal != null)
            {
                // Usa el FormLoader para cargar frmReportes en el panel de MenuPrincipal
                FormUtils.LoadFormInPanel(menuPrincipal.panelContainer, new frmReportes());
            }
        }

        private DataTable GetDataTableFromDGV(DataGridView dgv)
        {
            DataTable dt = new DataTable();

            // Agrega las columnas al DataTable
            foreach (DataGridViewColumn column in dgv.Columns)
            {
                dt.Columns.Add(column.Name, column.ValueType ?? typeof(string));
            }

            // Agrega las filas al DataTable
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (!row.IsNewRow)
                {
                    DataRow dr = dt.NewRow();
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        dr[cell.OwningColumn.Name] = cell.Value ?? DBNull.Value;
                    }
                    dt.Rows.Add(dr);
                }
            }

            return dt;
        }

        private void comboBoxClientes_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}


