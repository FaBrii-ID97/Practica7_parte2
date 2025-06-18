/*******************************************************************/
/*                            Practica 07                          */
/*                      Nombre: Fabricio Tixe                      */
/*              Fecha de realización: 12/06/2025                   */
/*              Fecha de entrega: 17/06/2025                       */
/*******************************************************************/

/******* RESULTADOS   DE LA PRÁCTICA 
 * Se logro clonar un repositorio de GitHub
 * Se realizan modificaciones en el código de la práctica 07
 * *******/

/*******   CONCLUSIONES
 * El uso de Git y GitHub permite gestionar versiones de un proyecto
 * de manera segura y eficiente, permitiendo trabajar de forma independiente 
 * sobre una copia del repositorio sin modificar el original
 * 
 * Visual Studio 2022 ofrece una integración completa con Git, facilitando
 * las tareas de commit, push y control de cambios desde una interfaz gráfica 
 * amigable, sin necesidad de usar la línea de comandos.
 *  *************/

/******* RECOMENDACIONES
 * Es recomendable realizar commits frecuentes y descriptivos para mantener un 
 * historial claro de los cambios realizados en el proyecto.
 * 
 * Utilizar ramas para desarrollar nuevas características o corregir errores, 
 * permite trabajar de manera aislada sin afectar la rama principal del proyecto.
 * 
 * *************/

using System;
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

    // Clase Protocolo que usa Pedido y Respuesta
    public class Protocolo1
    {
        private Pedido pedido;
        private Respuesta respuesta;

        // Constructor que recibe un Pedido
        public Protocolo1(Pedido pedido)
        {
            this.pedido = pedido;
            this.respuesta = new Respuesta();
        }

        // Método para realizar la operación con el pedido
        public void HazOperacion()
        {
            Console.WriteLine("Realizando operación: " + pedido.Comando);
        }

        // Método para resolver el pedido y generar una respuesta
        public Respuesta ResolverPedido()
        {
            respuesta.Estado = "OK";
            respuesta.Mensaje = "Pedido resuelto: " + pedido.ToString();
            return respuesta;
        }
    }

}
