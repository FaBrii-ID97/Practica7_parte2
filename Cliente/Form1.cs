using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using Protocolo;

namespace Cliente
{
    public partial class FrmValidador : Form
    {
        private TcpClient remoto;
        private NetworkStream flujo;

        public FrmValidador()
        {
            InitializeComponent();
        }

        // Evento que se ejecuta al cargar el formulario
        private void FrmValidador_Load(object sender, EventArgs e)
        {
            try
            {
                // Intentar establecer conexión con el servidor en localhost:8080
                remoto = new TcpClient("127.0.0.1", 8080);
                flujo = remoto.GetStream();
            }
            catch (SocketException ex)
            {
                // Mostrar mensaje de error si no se puede conectar
                MessageBox.Show("No se puedo establecer conexión " + ex.Message,
                    "ERROR");
            }
            /*
            finally 
            {
                flujo?.Close();
                remoto?.Close();
            }
            */

            // Deshabilitar panel de placa y controles de días de la semana al inicio
            panPlaca.Enabled = false;
            chkLunes.Enabled = false;
            chkMartes.Enabled = false;
            chkMiercoles.Enabled = false;
            chkJueves.Enabled = false;
            chkViernes.Enabled = false;
            chkDomingo.Enabled = false;
            chkSabado.Enabled = false;
        }

        private void btnIniciar_Click(object sender, EventArgs e)
        {
            string usuario = txtUsuario.Text;
            string contraseña = txtPassword.Text;
            // Validar que los campos de usuario y contraseña no estén vacíos
            if (usuario == "" || contraseña == "")
            {
                MessageBox.Show("Se requiere el ingreso de usuario y contraseña",
                    "ADVERTENCIA");
                return;
            }

            // Crear un objeto Pedido con el comando de ingreso y los parámetros
            Pedido pedido = new Pedido
            {
                Comando = "INGRESO",
                Parametros = new[] { usuario, contraseña }
            };

            // Enviar el pedido al servidor y recibir la respuesta
            Respuesta respuesta = HazOperacion(pedido);
            if (respuesta == null)
            {
                MessageBox.Show("Hubo un error", "ERROR");
                return;
            }

            // Procesar la respuesta del servidor
            if (respuesta.Estado == "OK" && respuesta.Mensaje == "ACCESO_CONCEDIDO")
            {
                panPlaca.Enabled = true;
                panLogin.Enabled = false;
                MessageBox.Show("Acceso concedido", "INFORMACIÓN");
                txtModelo.Focus();
            }
            // Si el acceso es denegado, mostrar mensaje y habilitar el panel de login
            else if (respuesta.Estado == "NOK" && respuesta.Mensaje == "ACCESO_NEGADO")
            {
                panPlaca.Enabled = false;
                panLogin.Enabled = true;
                MessageBox.Show("No se pudo ingresar, revise credenciales",
                    "ERROR");
                txtUsuario.Focus();
            }
        }

        // Método que realiza la operación de envío y recepción de datos al servidor
        private Respuesta HazOperacion(Pedido pedido)
        {
            // Verificar si el flujo de datos está disponible
            if (flujo == null)
            {
                MessageBox.Show("No hay conexión", "ERROR");
                return null;
            }
            try
            {
                // Convertir el pedido a bytes y enviarlo al servidor
                byte[] bufferTx = Encoding.UTF8.GetBytes(
                    pedido.Comando + " " + string.Join(" ", pedido.Parametros));
                
                flujo.Write(bufferTx, 0, bufferTx.Length);
                // Limpiar el buffer de recepción
                byte[] bufferRx = new byte[1024];
                // Leer la respuesta del servidor
                int bytesRx = flujo.Read(bufferRx, 0, bufferRx.Length);
                // Verificar si se recibieron datos
                string mensaje = Encoding.UTF8.GetString(bufferRx, 0, bytesRx);
                // Mostrar mensaje de depuración
                var partes = mensaje.Split(' ');
                // Crear un objeto Respuesta con el estado y mensaje recibido
                return new Respuesta
                {
                    Estado = partes[0],
                    Mensaje = string.Join(" ", partes.Skip(1).ToArray())
                };
            }
            catch (SocketException ex)
            {
                MessageBox.Show("Error al intentar transmitir " + ex.Message,
                    "ERROR");
            }
            /*
            finally 
            {
                flujo?.Close();
                remoto?.Close();
            }*/
            return null;
        }

        // Evento que se ejecuta al hacer clic en el botón de consultar
        private void btnConsultar_Click(object sender, EventArgs e)
        {
            string modelo = txtModelo.Text;
            string marca = txtMarca.Text;
            string placa = txtPlaca.Text;

            // Validar que los campos de modelo, marca y placa no estén vacíos
            Pedido pedido = new Pedido
            {
                Comando = "CALCULO",
                Parametros = new[] { modelo, marca, placa }
            };
            // Verificar si los campos de modelo, marca y placa están vacíos
            Respuesta respuesta = HazOperacion(pedido);
            if (respuesta == null)
            {
                MessageBox.Show("Hubo un error", "ERROR");
                return;
            }
            // Procesar la respuesta del servidor
            if (respuesta.Estado == "NOK")
            {
                MessageBox.Show("Error en la solicitud.", "ERROR");
                chkLunes.Checked = false;
                chkMartes.Checked = false;
                chkMiercoles.Checked = false;
                chkJueves.Checked = false;
                chkViernes.Checked = false;
            }
            else
            {
                // Dividir el mensaje recibido en partes y mostrar la información
                var partes = respuesta.Mensaje.Split(' ');
                MessageBox.Show("Se recibió: " + respuesta.Mensaje,
                    "INFORMACIÓN");
                byte resultado = Byte.Parse(partes[1]);
                // Actualizar los checkboxes según el resultado recibido
                switch (resultado)
                {
                    case 0b00100000:
                        chkLunes.Checked = true;
                        chkMartes.Checked = false;
                        chkMiercoles.Checked = false;
                        chkJueves.Checked = false;
                        chkViernes.Checked = false;
                        break;
                    case 0b00010000:
                        chkMartes.Checked = true;
                        chkLunes.Checked = false;
                        chkMiercoles.Checked = false;
                        chkJueves.Checked = false;
                        chkViernes.Checked = false;
                        break;
                    case 0b00001000:
                        chkMiercoles.Checked = true;
                        chkLunes.Checked = false;
                        chkMartes.Checked = false;
                        chkJueves.Checked = false;
                        chkViernes.Checked = false;
                        break;
                    case 0b00000100:
                        chkJueves.Checked = true;
                        chkLunes.Checked = false;
                        chkMartes.Checked = false;
                        chkMiercoles.Checked = false;
                        chkViernes.Checked = false;
                        break;
                    case 0b00000010:
                        chkViernes.Checked = true;
                        chkLunes.Checked = false;
                        chkMartes.Checked = false;
                        chkMiercoles.Checked = false;
                        chkJueves.Checked = false;
                        break;
                    default:
                        chkLunes.Checked = false;
                        chkMartes.Checked = false;
                        chkMiercoles.Checked = false;
                        chkJueves.Checked = false;
                        chkViernes.Checked = false;
                        break;
                }
            }
        }
        // Evento que se ejecuta al hacer clic en el botón de consultar número de pedidos
        private void btnNumConsultas_Click(object sender, EventArgs e)
        {
            String mensaje = "hola";
            // Validar que el mensaje no esté vacío
            Pedido pedido = new Pedido
            {
                Comando = "CONTADOR",
                Parametros = new[] { mensaje }
            };
            // Enviar el pedido al servidor y recibir la respuesta
            Respuesta respuesta = HazOperacion(pedido);
            if (respuesta == null)
            {
                MessageBox.Show("Hubo un error", "ERROR");
                return;
            }
            // Procesar la respuesta del servidor
            if (respuesta.Estado == "NOK")
            {
                MessageBox.Show("Error en la solicitud.", "ERROR");

            }
           
            else
            {
                var partes = respuesta.Mensaje.Split(' ');
                MessageBox.Show("El número de pedidos recibidos en este cliente es " + partes[0],
                    "INFORMACIÓN");
            }
        }

        // Evento que se ejecuta al cerrar el formulario
        private void FrmValidador_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Cerrar el flujo de datos y el cliente remoto al cerrar el formulario
            if (flujo != null)
                flujo.Close();
            if (remoto != null)
                if (remoto.Connected)
                    remoto.Close();
        }
    }
}
