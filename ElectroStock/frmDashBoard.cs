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
    public partial class frmDashBoard : Form
    {
        private Timer timer;
        public frmDashBoard()
        {
            InitializeComponent();
            this.Controls.Add(bunifuLabel2);

            // Configurar el Timer para mostrar la hora actual
            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += (sender, e) => bunifuLabel2.Text = DateTime.Now.ToString("HH:mm:ss");
            timer.Start();

            // Connection String
            string connectionString = ConnectionClass.Conexion;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Consultas para obtener los totales
                    string queryUsuarios = "SELECT COUNT(*) FROM Usuarios";
                    string queryProductos = "SELECT COUNT(*) FROM Productos";
                    string queryProveedores = "SELECT COUNT(*) FROM Proveedores";
                    string queryClientes = "SELECT COUNT(*) FROM Clientes";
                    string queryTotalVentas = "SELECT SUM(Total) FROM Ventas"; // Suponiendo que 'Total' es el campo del total en la tabla Ventas
                    string querypedidos = "SELECT ID_Pedido FROM Ventas";

                    // Totales
                    lblTotalUsuario.Text = EjecutarConsultaScalar(queryUsuarios, connection).ToString() + " Usuarios Creados";
                    lblTotalProductos.Text = EjecutarConsultaScalar(queryProductos, connection).ToString() + " Productos Almacenados";
                    lblTotalProveedores.Text = EjecutarConsultaScalar(queryProveedores, connection).ToString() + " Proveedores Activos";
                    lblTotalClientes.Text = EjecutarConsultaScalar(queryClientes, connection).ToString() + " Clientes Almacenados";
                    lblTotalReportes.Text = EjecutarConsultaScalar(querypedidos, connection).ToString() + " Pedidos Hechos";
                    // Obtener total de dinero vendido en ventas (manejar null si no hay datos)
                    object totalVentas = EjecutarConsultaScalar(queryTotalVentas, connection);
                    lblTotalVentas.Text = (totalVentas != DBNull.Value ? Convert.ToDecimal(totalVentas).ToString("C") : "$0.00");

                    // Obtener total de reportes creados
                 
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al cargar los totales: " + ex.Message);
                }
            }
        }

        // Método para ejecutar consultas con ExecuteScalar
        private object EjecutarConsultaScalar(string query, SqlConnection connection)
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                return command.ExecuteScalar();
            }
        }

        private void frmDashBoard_Load(object sender, EventArgs e)
        {

        }
    }
}
