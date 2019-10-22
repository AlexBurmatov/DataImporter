using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DataImporter
{
    public class SocketServer
    {
        public delegate void MessageHandler(Socket socket, TransferPacket packet);

        public event MessageHandler MessageReceived;

        private static int _counter = 1;
        public int Counter
        {
            get
            {
                _counter++;
                return _counter - 1;
            }
        }

        public SocketServer(string address, int port)
        {
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);

            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listenSocket.Bind(ipPoint);
                listenSocket.Listen(10);

                Console.WriteLine("Server is listening... ");

                new Task(() => Listener(listenSocket)).Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void Send(Socket socket, TransferPacket packet)
        {
            var serializedResponse = JsonConvert.SerializeObject(packet);

            var bytes = Encoding.ASCII.GetBytes(serializedResponse);

            socket.Send(bytes);
        }

        private void Listener(Socket listenSocket)
        {
            try
            {
                while (true)
                {
                    Socket handler = listenSocket.Accept();

                    Console.WriteLine("Register new connection.");

                    new Task(() => SocketHandler(handler)).Start();
                }
            }
            catch
            {
                listenSocket.Shutdown(SocketShutdown.Both);
                listenSocket.Close();
            }
        }

        private void SocketHandler(Socket socket)
        {
            try
            {
                while (true)
                {
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    byte[] data = new byte[256];

                    do
                    {
                        bytes = socket.Receive(data);
                        builder.Append(Encoding.ASCII.GetString(data, 0, bytes));
                    }
                    while (socket.Available > 0);

                    string message = builder.ToString();

                    var packet = JsonConvert.DeserializeObject<TransferPacket>(message);

                    MessageReceived.Invoke(socket, packet);
                }
            }
            catch
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
        }
    }
}
