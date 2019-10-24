using System;
using RabbitMQ.Client;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;
using Common;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Configuration;

namespace SenderApp
{
    class Program
    {
        private static Random rand = new Random();

        private static SocketClient client;

        private static int ClientId;

        public static void Main()
        {
            InitSocket();

            KeyExchange();

            bool exit = false;
            while (!exit)
            {
                var key = Console.ReadKey().Key;

                switch (key)
                {
                    case ConsoleKey.Enter:
                    case ConsoleKey.Spacebar:
                        var obj = GenerateObject();
                        var list = new List<Ticket>() { obj };

                        string message = JsonConvert.SerializeObject(list);

                        var encryptMsg = CryptoService.EncryptMessage(message);

                        var packet = new TransferPacket
                        {
                            type = PacketType.ImportData,
                            clientId = ClientId,
                            data = encryptMsg
                        };

                        if (key == ConsoleKey.Enter) SendToMessageQueue(packet);
                        else client.Send(packet);

                        Console.WriteLine(" [x] {0} \n [x] Sent {1}", DateTime.Now.ToLongTimeString(), message);
                        break;
                    case ConsoleKey.Escape:
                        exit = true;
                        break;
                }
            }
        }
        private static void InitSocket()
        {
            var port = ConfigurationManager.AppSettings.Get("PortForSocket");

            client = new SocketClient("servicehost", int.Parse(port));
        }

        private static void KeyExchange()
        {
            try
            {
                // запрос на получение открытого ключа шифрования
                var request = new TransferPacket
                {
                    type = PacketType.PublicKeyRequest,
                    data = string.Empty
                };

                client.Send(request);
                var response = client.Receive();

                ClientId = response.clientId;
                CryptoService.SetPublicKey(response.data);

                // отправляем ключ шифрования данных
                request = new TransferPacket
                {
                    type = PacketType.TransferKey,
                    clientId = ClientId,
                    data = CryptoService.GetEncryptedSymmetricKey()
                };
                client.Send(request);

                Console.WriteLine("RSA key: {0}\nDES key: {1}\n", response.data, CryptoService.GetSymmetricKey());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void SendToMessageQueue(TransferPacket packet)
        {
            var factory = new ConnectionFactory() { HostName = ConfigurationManager.AppSettings.Get("RabbitMQAddress") };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "hello",
                                        durable: false,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);

                var serializedPacket = JsonConvert.SerializeObject(packet);

                var body = Encoding.UTF8.GetBytes(serializedPacket);

                channel.BasicPublish(exchange: "",
                                        routingKey: "hello",
                                        basicProperties: null,
                                        body: body);
            }
        }

        #region Generate Object
        private static Ticket GenerateObject()
        {
            return new Ticket
            {
                ArrivalTo = GenerateString(),
                Category = GenerateString(),
                Class = GenerateString(),
                Coach = GenerateNumber(),
                CoachType = GenerateString(),
                Days = GenerateString(),
                DepartureDate = GenRandomDateTime(),
                DepartureFrom = GenerateString(),
                DepartureTimeFrom = GenRandomTimeSpan(),
                DepartureTimeTo = GenRandomTimeSpan(),
                Food = false,
                Id = GenerateNumber(),
                PeriodFrom = GenRandomDateTime(),
                PeriodTo = GenRandomDateTime(),
                Price = GenerateNumber(),
                RouteFrom = GenerateString(),
                RouteTo = GenerateString(),
                Seat = GenerateNumber(),
                StationKmFrom = GenerateNumber(),
                StationKmTo = GenerateNumber(),
                StopDurationFrom = GenerateNumber(),
                StopDurationTo = GenerateNumber(),
                TicketRate = GenerateNumber(),
                Timeline = GenerateString(),
                TrainNumber = GenerateNumber()
            };
        }

        private static string GenerateString()
        {
            string result = string.Empty;
            int length = GenerateNumber(7, 15);

            for (int i = 0; i < length; i++)
            {
                result += (char)rand.Next(65, 122);
            }

            return result;
        }

        private static int GenerateNumber(int from, int to)
        {
            return rand.Next(from, to);
        }

        private static int GenerateNumber()
        {
            return GenerateNumber(1, 200);
        }

        private static DateTime GenRandomDateTime(DateTime from, DateTime to)
        {
            if (from >= to)
            {
                throw new Exception("Параметр \"from\" должен быть меньше параметра \"to\"!");
            }
            TimeSpan range = to - from;
            var randts = new TimeSpan((long)(rand.NextDouble() * range.Ticks));
            return from + randts;
        }

        private static DateTime GenRandomDateTime()
        {
            return GenRandomDateTime(DateTime.Today.AddDays(-60), DateTime.Today.AddDays(90));
        }

        private static TimeSpan GenRandomTimeSpan()
        {
            return new TimeSpan((long)(rand.NextDouble() * 1000000));
        }
        #endregion
    }
}
