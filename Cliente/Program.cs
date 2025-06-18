using Protocolo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cliente
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Crear un mensaje de prueba para procesar
            string mensaje = "HACER_COMPRA item1 item2 item3";

            // Procesar el mensaje utilizando la clase Pedido
            Pedido pedido = Pedido.Procesar(mensaje);

            // Crear un objeto Protocolo con el pedido procesado
            Protocolo1 protocolo = new Protocolo1(pedido);  // Asegúrate de que la clase Protocolo esté definida correctamente

            // Realizar la operación asociada al pedido
            protocolo.HazOperacion();

            // Resolver el pedido y obtener la respuesta
            Respuesta respuesta = protocolo.ResolverPedido();

            // Mostrar la respuesta en consola (puedes también mostrarla en un cuadro de texto o label en la UI)
            Console.WriteLine("Respuesta: " + respuesta.ToString());

            // Lanzar la aplicación de Windows Forms
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmValidador());
        }
    }
}
