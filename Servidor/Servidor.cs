using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using Protocolo;

namespace Servidor
{
    class Servidor
    {
        private static TcpListener escuchador;
        private static Dictionary<string, int> listadoClientes
            = new Dictionary<string, int>();

        static void Main(string[] args)
        {
            // Iniciar el servidor en el puerto 8080
            try
            {
                escuchador = new TcpListener(IPAddress.Any, 8080);
                escuchador.Start();
                Console.WriteLine("Servidor inició en el puerto 5000...");
                // Aceptar conexiones de clientes en un bucle infinito
                while (true)
                {
                    TcpClient cliente = escuchador.AcceptTcpClient();
                    Console.WriteLine("Cliente conectado, puerto: {0}", cliente.Client.RemoteEndPoint.ToString());
                    Thread hiloCliente = new Thread(ManipuladorCliente);
                    hiloCliente.Start(cliente);
                }
            }
            // Capturar excepciones de socket al iniciar el servidor
            catch (SocketException ex)
            {
                Console.WriteLine("Error de socket al iniciar el servidor: " +
                    ex.Message);
            }
            finally 
            {
                escuchador?.Stop();
            }
        }

        // Método que maneja la comunicación con el cliente
        private static void ManipuladorCliente(object obj)
        {
            TcpClient cliente = (TcpClient)obj;
            NetworkStream flujo = null;
            try
            {
                // Obtener el flujo de datos del cliente
                flujo = cliente.GetStream();
                byte[] bufferTx;
                byte[] bufferRx = new byte[1024];
                int bytesRx;

                // Leer datos del cliente en un bucle
                while ((bytesRx = flujo.Read(bufferRx, 0, bufferRx.Length)) > 0)
                {
                    string mensajeRx =
                        Encoding.UTF8.GetString(bufferRx, 0, bytesRx);
                    Pedido pedido = Pedido.Procesar(mensajeRx);
                    Console.WriteLine("Se recibio: " + pedido);
                    // Mostrar la dirección del cliente
                    string direccionCliente =
                        cliente.Client.RemoteEndPoint.ToString();
                    Respuesta respuesta = ResolverPedido(pedido, direccionCliente);
                    Console.WriteLine("Se envió: " + respuesta);

                    bufferTx = Encoding.UTF8.GetBytes(respuesta.ToString());
                    flujo.Write(bufferTx, 0, bufferTx.Length);
                }

            }
            // Capturar excepciones de socket al manejar el cliente
            catch (SocketException ex)
            {
                Console.WriteLine("Error de socket al manejar el cliente: " + ex.Message);
            }
            finally
            {
                flujo?.Close();
                cliente?.Close();
            }
        }
        // Método que resuelve el pedido del cliente y devuelve una respuesta
        private static Respuesta ResolverPedido(Pedido pedido, string direccionCliente)
        {
            Respuesta respuesta = new Respuesta
            { Estado = "NOK", Mensaje = "Comando no reconocido" };
            // Validar el comando del pedido y generar la respuesta adecuada
            switch (pedido.Comando)
            {
                case "INGRESO":
                    if (pedido.Parametros.Length == 2 &&
                        pedido.Parametros[0] == "root" &&
                        pedido.Parametros[1] == "admin20")
                    {
                        respuesta = new Random().Next(2) == 0
                            ? new Respuesta 
                            { Estado = "OK", 
                                Mensaje = "ACCESO_CONCEDIDO" }
                            : new Respuesta 
                            { Estado = "NOK", 
                                Mensaje = "ACCESO_NEGADO" };
                    }
                    else
                    {
                        respuesta.Mensaje = "ACCESO_NEGADO";
                    }
                    break;
                // Comando para hacer una compra
                case "CALCULO":
                    if (pedido.Parametros.Length == 3)
                    {
                        string modelo = pedido.Parametros[0];
                        string marca = pedido.Parametros[1];
                        string placa = pedido.Parametros[2];
                        if (ValidarPlaca(placa))
                        {
                            byte indicadorDia = ObtenerIndicadorDia(placa);
                            respuesta = new Respuesta
                            { Estado = "OK", 
                                Mensaje = $"{placa} {indicadorDia}" };
                            ContadorCliente(direccionCliente);
                        }
                        else
                        {
                            respuesta.Mensaje = "Placa no válida";
                        }
                    }
                    break;
                // Comando para hacer una compra
                case "CONTADOR":
                    if (listadoClientes.ContainsKey(direccionCliente))
                    {
                        respuesta = new Respuesta
                        { Estado = "OK",
                            Mensaje = listadoClientes[direccionCliente].ToString() };
                    }
                    else
                    {
                        respuesta.Mensaje = "No hay solicitudes previas";
                    }
                    break;
            }

            return respuesta;
        }
        // Método para validar el formato de la placa del vehículo
        private static bool ValidarPlaca(string placa)
        {
            return Regex.IsMatch(placa, @"^[A-Z]{3}[0-9]{4}$");
        }
        // Método para obtener el indicador del día de la semana basado en el último dígito de la placa
        private static byte ObtenerIndicadorDia(string placa)
        {
            int ultimoDigito = int.Parse(placa.Substring(6, 1));
            switch (ultimoDigito)
            {
                
                case 1: 
                case 2: 
                    return 0b00100000; // Lunes
                case 3: 
                case 4: 
                    return 0b00010000; // Martes
                case 5: 
                case 6: 
                    return 0b00001000; // Miércoles
                case 7: 
                case 8: 
                    return 0b00000100; // Jueves
                case 9: 
                case 0: 
                    return 0b00000010; // Viernes
                default: 
                    return 0;
            }
        }
        // Método para contar las solicitudes de un cliente
        private static void ContadorCliente(string direccionCliente)
        {
            // Si el cliente ya existe en el diccionario, incrementar su contador
            if (listadoClientes.ContainsKey(direccionCliente))
            {
                listadoClientes[direccionCliente]++;
            }
            // Si no existe, agregarlo con un contador inicial de 1
            else
            {
                listadoClientes[direccionCliente] = 1;
            }
        }

    }
}
