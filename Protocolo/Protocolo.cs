/*******************************************************************/
/*                            Practica 07                          */
/*                      Nombre: Fabricio Tixe                      */
/*              Fecha de realización: 12/06/2025                   */
/*              Fecha de entrega: 17/06/2025                       */
/*******************************************************************/

/******* RESULTADOS   DE LA PRÁCTICA 
 * Se logro clonar un repositorio de GitHub
 * 
 * 
 * 
 * 
 * 
 * *******/

using System.Linq;

namespace Protocolo
{
    public class Pedido
    {
        public string Comando { get; set; }
        public string[] Parametros { get; set; }

        public static Pedido Procesar(string mensaje)
        {
            var partes = mensaje.Split(' ');
            return new Pedido
            {
                Comando = partes[0].ToUpper(),
                Parametros = partes.Skip(1).ToArray()
            };
        }

        public override string ToString()
        {
            return $"{Comando} {string.Join(" ", Parametros)}";
        }
    }

    public class Respuesta
    {
        public string Estado { get; set; }
        public string Mensaje { get; set; }

        public override string ToString()
        {
            return $"{Estado} {Mensaje}";
        }
    }

}
