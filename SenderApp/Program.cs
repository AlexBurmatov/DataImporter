using System;
using RabbitMQ.Client;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SenderApp
{
    class Program
    {
        private static Random rand = new Random();

        public static void Main()
        {
            while (true)
            {
                var factory = new ConnectionFactory() { HostName = "localhost" };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "hello",
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    var obj = GenerateObject();
                    var list = new List<Ticket>() { obj };

                    string message = JsonConvert.SerializeObject(list);
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "",
                                         routingKey: "hello",
                                         basicProperties: null,
                                         body: body);
                    Console.WriteLine(" [x] Sent {0}", message);
                }

                var cmd = Console.ReadLine();

                if (cmd.Contains("exit"))
                    break;
            }
        }

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
    }
}
