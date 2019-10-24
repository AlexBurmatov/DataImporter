using Common;
using DataImporter.Objects;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DataImporter
{
    class Program
    {
        private static SocketServer server;
        public static void Main()
        {
            var connString = ConfigurationManager.AppSettings.Get("DbConnectionString");
            DataService.Connect(connString);

            InitSocket();

            InitMessageQueue();

            Console.ReadKey();
        }

        private static void InitSocket()
        {
            var port = ConfigurationManager.AppSettings.Get("PortForSocket");

            server = new SocketServer("servicehost", int.Parse(port));

            server.MessageReceived += Server_MessageReceived;
        }
        
        private static void InitMessageQueue()
        {
            var factory = new ConnectionFactory() { HostName = ConfigurationManager.AppSettings.Get("RabbitMQAddress") };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.QueueDeclare(queue: "hello",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += Consumer_Received;
            channel.BasicConsume(queue: "hello",
                                 autoAck: true,
                                 consumer: consumer);
        }

        private static void Server_MessageReceived(Socket socket, TransferPacket packet)
        {
            switch (packet.type)
            {
                case PacketType.PublicKeyRequest:
                    var response = new TransferPacket
                    {
                        type = PacketType.PublicKeyResponse,
                        clientId = server.Counter,
                        data = CryptoService.GetPublicKey()
                    };
                    server.Send(socket, response);
                    break;
                case PacketType.TransferKey:
                    CryptoService.SetSymmetricKey(packet.clientId, packet.data);
                    Console.WriteLine("RSA key: {0}\nDES key: {1}\n", CryptoService.GetPublicKey(), CryptoService.GetSymmetricKey());
                    break;
                case PacketType.ImportData:
                    ProcessReceivedMessage(packet.clientId, packet.data);
                    break;
            }
        }

        private static void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            var body = e.Body;
            var message = Encoding.UTF8.GetString(body);

            var packet = JsonConvert.DeserializeObject<TransferPacket>(message);

            ProcessReceivedMessage(packet.clientId, packet.data);
        }

        private static void ProcessReceivedMessage(int id, string msg)
        {
            var decryptMsg = CryptoService.DecryptMessage(id, msg);

            var receivedObjects = JsonConvert.DeserializeObject<List<NonNormalizedTicket>>(decryptMsg);

            Console.WriteLine(" [x] {0} \n [x] Received {1}", DateTime.Now.ToLongTimeString(), decryptMsg);

            var normObjs = NormalizeObjects(receivedObjects);

            DataService.InsertOrUpdate(normObjs.OfType<Timeline>());
            DataService.InsertOrUpdate(normObjs.OfType<RailwayStation>());
            DataService.InsertOrUpdate(normObjs.OfType<Train>());
            DataService.InsertOrUpdate(normObjs.OfType<Trip>());
            DataService.InsertOrUpdate(normObjs.OfType<TrainStop>());
            DataService.InsertOrUpdate(normObjs.OfType<TicketPrice>());
            DataService.InsertOrUpdate(normObjs.OfType<Ticket>());

            DataService.SaveChanges();
        }

        public static List<DataObject> NormalizeObjects(List<NonNormalizedTicket> nonNormalizedTickets)
        {
            var normObjs = new List<DataObject>();

            foreach (var row in nonNormalizedTickets)
            {
                row.ComputeForeignKeys();

                normObjs.Add(RailwayStation.Parse(row, 1));
                normObjs.Add(RailwayStation.Parse(row, 2));
                normObjs.Add(Ticket.Parse(row));
                normObjs.Add(TicketPrice.Parse(row));
                normObjs.Add(Timeline.Parse(row));
                normObjs.Add(Train.Parse(row));
                normObjs.Add(TrainStop.Parse(row, 1));
                normObjs.Add(TrainStop.Parse(row, 2));
                normObjs.Add(Trip.Parse(row));
            }

            return normObjs;
        }
    }
}
