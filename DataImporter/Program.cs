using DataImporter.Objects;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataImporter
{
    class Program
    {
        public static void Main()
        {
            DataService.Connect("Server=localhost;Port=5432;Database=train-db;Username=postgres;Password=masterkey");

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
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

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }

        private static void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            var body = e.Body;
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine(" [x] Received {0}", message);

            var receivedObjects = JsonConvert.DeserializeObject<List<NonNormalizedTicket>>(message);

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
