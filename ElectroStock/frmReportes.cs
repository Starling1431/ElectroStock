using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
namespace Studium
{
    public partial class frmReportes : Form
    {
    
        public frmReportes()
        {
            InitializeComponent();
           
            
        }
        private void ConfigurarDataGridViewReportes()
        {
            // Limpia las columnas actuales
            dgvReportes.Columns.Clear();

            // Agregar columnas
            dgvReportes.Columns.Add("Cliente", "Cliente");
            dgvReportes.Columns.Add("Producto", "Producto");
            dgvReportes.Columns.Add("Cantidad", "Cantidad");
            dgvReportes.Columns.Add("Precio", "Precio");
            dgvReportes.Columns.Add("Total", "Total");
            dgvReportes.Columns.Add("VentaTotal", "VentaTotal");
            dgvReportes.Columns.Add("Fecha", "Fecha");
            // Configura el formato de la columna Precio y Total si es necesario
            dgvReportes.Columns["Precio"].DefaultCellStyle.Format = "C2"; // Moneda
            dgvReportes.Columns["Total"].DefaultCellStyle.Format = "C2";  // Moneda
            dgvReportes.Columns["VentaTotal"].DefaultCellStyle.Format = "C2";  // Moneda
        }
        private void CargarPedidoEnReportes(Pedido pedidoSeleccionado)
        {
            // Configura las columnas si no se han añadido ya
            if (dgvReportes.Columns.Count == 0)
            {
                ConfigurarDataGridViewReportes();
            }

            // Verifica si el pedido ya está en el DataGridView
            bool pedidoExistente = false;
            foreach (DataGridViewRow row in dgvReportes.Rows)
            {
                // Asegura que no estamos comparando las filas vacías
                if (!row.IsNewRow)
                {
                    // Verifica si los valores no son nulos antes de hacer la comparación
                    var clienteValor = row.Cells["Cliente"].Value?.ToString();
                    var productoValor = row.Cells["Producto"].Value?.ToString();
                    var fechaValor = row.Cells["Fecha"].Value?.ToString();

                    // Compara los valores de Cliente, Producto y Fecha para evitar duplicados
                    if (clienteValor == pedidoSeleccionado.Cliente &&
                        productoValor == pedidoSeleccionado.Producto &&
                        fechaValor == pedidoSeleccionado.Fecha)
                    {
                        pedidoExistente = true;
                        break;  // Sale del ciclo si encuentra un pedido duplicado
                    }
                }
            }

            // Si el pedido no existe, lo agregamos al DataGridView
            if (!pedidoExistente)
            {
                dgvReportes.Rows.Add(
                    pedidoSeleccionado.Cliente,
                    pedidoSeleccionado.Producto,
                    pedidoSeleccionado.Cantidad,
                    pedidoSeleccionado.Precio,
                    pedidoSeleccionado.Total,
                    pedidoSeleccionado.VentaTotal,
                    pedidoSeleccionado.Fecha
                );
            }
            else
            {
                MessageBox.Show("Este pedido ya está registrado en el reporte.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public void ActualizarComboBoxPedidos()
        {
            // Agrupar los pedidos por PedidoID para eliminar duplicados
            comboBoxPedidos.DataSource = PedidoManager.PedidosFinalizados
                .GroupBy(p => p.PedidoID)  // Agrupar por PedidoID
                .Select(g => g.First())     // Seleccionar solo un pedido de cada grupo
                .ToList();

            // Configurar lo que se muestra y el valor seleccionado
            comboBoxPedidos.DisplayMember = "PedidoID"; // Lo que el usuario ve
            comboBoxPedidos.ValueMember = "PedidoID";   // El valor que se selecciona
            comboBoxPedidos.SelectedItem = null;        // No seleccionar nada al inicio
        }

        private void frmReportes_Load(object sender, EventArgs e)
        {
            // Llamar a ActualizarComboBoxPedidos() al cargar el formulario
            ActualizarComboBoxPedidos();

            // Configuración del DataGridView
            dgvReportes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvReportes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvReportes.ReadOnly = true;
            ConfigurarDataGridViewReportes(); // Configura las columnas
        }

    
        private void iconButton1_Click(object sender, EventArgs e)
        {

            // Verifica si la DataGridView tiene filas con datos válidos
            bool tieneDatos = dgvReportes.Rows.Cast<DataGridViewRow>()
                .Any(row => !row.IsNewRow && row.Cells.Cast<DataGridViewCell>().Any(cell => cell.Value != null));

            if (!tieneDatos)
            {
                MessageBox.Show("La tabla no contiene datos para exportar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Salir del método si no hay datos
            }

            using (ExcelPackage excel = new ExcelPackage())
            {
                excel.Workbook.Worksheets.Add("Reporte");
                var worksheet = excel.Workbook.Worksheets[0];

                // Agregar encabezados
                worksheet.Cells[1, 1].Value = "Cliente";
                worksheet.Cells[1, 2].Value = "Producto";
                worksheet.Cells[1, 3].Value = "Cantidad";
                worksheet.Cells[1, 4].Value = "Precio";
                worksheet.Cells[1, 5].Value = "Total";
                worksheet.Cells[1, 6].Value = "VentaTotal";
                worksheet.Cells[1, 7].Value = "Fecha";

                // Agregar datos
                int row = 2;
                foreach (DataGridViewRow dgvRow in dgvReportes.Rows)
                {
                    if (!dgvRow.IsNewRow)
                    {
                        worksheet.Cells[row, 1].Value = dgvRow.Cells["Cliente"].Value;
                        worksheet.Cells[row, 2].Value = dgvRow.Cells["Producto"].Value;
                        worksheet.Cells[row, 3].Value = dgvRow.Cells["Cantidad"].Value;
                        worksheet.Cells[row, 4].Value = dgvRow.Cells["Precio"].Value;
                        worksheet.Cells[row, 5].Value = dgvRow.Cells["Total"].Value;
                        worksheet.Cells[row, 6].Value = dgvRow.Cells["VentaTotal"].Value;
                        worksheet.Cells[row, 7].Value = dgvRow.Cells["Fecha"].Value;
                        row++;
                    }
                }

                // Guardar el archivo
                using (SaveFileDialog saveFile = new SaveFileDialog { Filter = "Excel Files|*.xlsx" })
                {
                    if (saveFile.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllBytes(saveFile.FileName, excel.GetAsByteArray());
                        MessageBox.Show("Reporte exportado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            comboBoxPedidos.DataSource = null; // Desvincula el DataSource
            comboBoxPedidos.Items.Clear(); // Elimina todas las opciones del ComboBox
            dgvReportes.Rows.Clear();
        }

        private void comboBoxPedidos_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Obtiene el pedido seleccionado del ComboBox
              Pedido pedidoSeleccionado = (Pedido)comboBoxPedidos.SelectedItem;

            
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            // Obtén la lista de pedidos seleccionados
            List<Pedido> pedidosSeleccionados = ObtenerPedidosSeleccionados();

            if (pedidosSeleccionados == null || pedidosSeleccionados.Count == 0)
            {
                MessageBox.Show("No se ha seleccionado un pedido válido o el pedido no contiene productos.",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Carga todos los pedidos seleccionados al DataGridView
            foreach (var pedido in pedidosSeleccionados)
            {
                CargarPedidoEnReportes(pedido);
            }
        }
        private List<Pedido> ObtenerPedidosSeleccionados()
        {
            // Comprueba si un elemento está seleccionado en el ComboBox
            if (comboBoxPedidos.SelectedItem != null)
            {
                string pedidoIDSeleccionado = comboBoxPedidos.SelectedValue.ToString();

                // Filtra todos los pedidos con el mismo PedidoID
                return PedidoManager.PedidosFinalizados
                    .Where(p => p.PedidoID == pedidoIDSeleccionado)
                    .ToList();
            }

            return new List<Pedido>();  // Si no hay un pedido seleccionado, devuelve una lista vacía
        }

        private void iconButton3_Click(object sender, EventArgs e)
        {
            try
            {
                // Crear el cuadro de diálogo de guardar archivo
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Archivos PDF (*.pdf)|*.pdf"; // Filtro para solo archivos PDF
                saveFileDialog.DefaultExt = "pdf"; // Extensión predeterminada
                saveFileDialog.FileName = "ElectroStockPedido.pdf"; // Nombre predeterminado del archivo

                // Mostrar el cuadro de diálogo y verificar si el usuario seleccionó una ruta
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Ruta completa seleccionada por el usuario
                    string outputPath = saveFileDialog.FileName;

                    // Verificar si el archivo ya existe y eliminarlo
                    if (File.Exists(outputPath))
                    {
                        File.Delete(outputPath); // Eliminar el archivo si ya existe
                    }

                    // Crear el documento PDF
                    PdfDocument pdfDocument = new PdfDocument();
                    PdfPage page = pdfDocument.AddPage(); // Añadir una página al documento
                    XGraphics gfx = XGraphics.FromPdfPage(page); // Obtener el objeto para dibujar en la página

                    // Usamos una fuente predeterminada de PdfSharp (sin necesidad de XFontStyle)
                    XFont font = new XFont("Arial", 12); // Usar la fuente predeterminada sin estilo

                    // Establecer el punto de inicio para dibujar texto
                    double xPoint = 10;
                    double yPoint = 10;

                    // Dibujar los encabezados de columna
                    foreach (DataGridViewColumn column in dgvReportes.Columns)
                    {
                        gfx.DrawString(column.HeaderText, font, XBrushes.Black, new XPoint(xPoint, yPoint));
                        yPoint += 20; // Avanzar hacia abajo para el siguiente encabezado
                    }

                    // Dibujar los datos de las filas
                    foreach (DataGridViewRow row in dgvReportes.Rows)
                    {
                        if (!row.IsNewRow) // Evitar filas vacías
                        {
                            yPoint += 20; // Dejar espacio para la fila
                            foreach (DataGridViewCell cell in row.Cells)
                            {
                                string cellValue = cell.Value?.ToString() ?? "N/A";
                                gfx.DrawString(cellValue, font, XBrushes.Black, new XPoint(xPoint, yPoint));
                                yPoint += 20; // Avanzar hacia abajo para la siguiente celda
                            }
                        }
                    }

                    // Intentar guardar el documento PDF
                    try
                    {
                        pdfDocument.Save(outputPath); // Guarda el archivo en la ruta proporcionada
                                                      // Cerrar el documento PDF
                        pdfDocument.Close();

                        // Confirmación de éxito
                        MessageBox.Show($"Archivo PDF generado correctamente en: {outputPath}", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        MessageBox.Show($"Error de acceso: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (IOException ex)
                    {
                        MessageBox.Show($"Error de archivo: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ocurrió un error al guardar el PDF: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error general: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    
    }
}
