using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocolo
{
    internal class newProtocolo
    {
        // Método principal para probar las clases Pedido, Respuesta y ProtocoloAdd commentMore actions
        public static void Main(string[] args)
        {
            // Crear un mensaje de prueba (en este caso un pedido)
            string mensaje = "HACER_COMPRA item1 item2 item3";

            // Procesar el mensaje para obtener un objeto Pedido
            Pedido pedido = Pedido.Procesar(mensaje);
            Console.WriteLine("Pedido procesado: " + pedido.ToString());

            // Crear un objeto Protocolo utilizando el Pedido procesado
            Protocolo1 protocolo = new Protocolo1(pedido);

            // Realizar la operación asociada al pedido
            protocolo.HazOperacion();

            // Resolver el pedido y obtener la respuesta
            Respuesta respuesta = protocolo.ResolverPedido();

            // Mostrar la respuesta en la consola
            Console.WriteLine("Respuesta: " + respuesta.ToString());

            // Pausar para ver la salida en la consola
            Console.ReadLine();
        }
    }
}
