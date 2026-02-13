using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Studium
{
    public class Pedido
    {
      
        public string Cliente { get; set; }
        public string Producto { get; set; }

        public string PedidoID { get; set; }  // Identificador del pedido ("Pedido 1", "Pedido 2", etc.)
        public int Cantidad { get; set; }
        public double Precio { get; set; }
        public double Total { get; set; }
        public string VentaTotal { get; set; }
        public string Fecha { get; set; }
    }

    public static class PedidoManager
    {
        // Lista de pedidos finalizados
        public static List<Pedido> PedidosFinalizados = new List<Pedido>();
        public static int ContadorPedidos { get; set; } = 0; // Contador de pedidos realizados
                                                             // Contador estático para el PedidoID
        private static int contadorPedido = 1;

        // Método para obtener un nuevo PedidoID
        public static string ObtenerNuevoPedidoID()
        {
            return "Pedido " + contadorPedido++;
        }
    }
}
