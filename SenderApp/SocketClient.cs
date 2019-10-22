using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SenderApp
{
    public class SocketClient
    {
        private Socket socket;

        /*public delegate void MessageHandler(TransferPacket packet);

        public event MessageHandler MessageReceived;*/

        public SocketClient(string address, int port)
        {
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                bool connected = false;
                while (!connected)
                {
                    try
                    {
                        Console.Write("Connecting... ");
                        socket.Connect(ipPoint);
                        connected = true;
                        Console.WriteLine("Success");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Failed");
                        Thread.Sleep(1000);
                    }
                }

                //new Task(() => SocketHandler()).Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void Send(TransferPacket packet)
        {
            var serializedResponse = JsonConvert.SerializeObject(packet);

            var bytes = Encoding.ASCII.GetBytes(serializedResponse);

            socket.Send(bytes);
        }

        public TransferPacket Receive()
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

            return packet;
        }

        /*private void SocketHandler()
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
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (socket.Available > 0);

                    string message = builder.ToString();

                    var packet = JsonConvert.DeserializeObject<TransferPacket>(message);

                    MessageReceived.Invoke(packet);
                }
            }
            catch
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
        }*/
    }
}
