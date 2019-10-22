using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DataImporter.Objects
{
    public class NonNormalizedTicket
    {
        private static readonly SHA1CryptoServiceProvider sha = new SHA1CryptoServiceProvider();

        public int Id { get; set; }

        public int TrainNumber { get; set; }

        public string Category { get; set; }

        public string RouteFrom { get; set; }

        public string RouteTo { get; set; }

        public string Timeline { get; set; }

        public string Days { get; set; }

        public string CoachType { get; set; }

        public double TicketRate { get; set; }

        public DateTime PeriodFrom { get; set; }

        public DateTime PeriodTo { get; set; }

        public int Coach { get; set; }

        public string Class { get; set; }

        public int Seat { get; set; }

        public DateTime DepartureDate { get; set; }

        public string DepartureFrom { get; set; }

        public int StationKmFrom { get; set; }

        public TimeSpan DepartureTimeFrom { get; set; }

        public int StopDurationFrom { get; set; }

        public string ArrivalTo { get; set; }

        public int StationKmTo { get; set; }

        public TimeSpan DepartureTimeTo { get; set; }

        public int StopDurationTo { get; set; }

        public bool Food { get; set; }

        public double Price { get; set; }

        // Ключи для новых таблиц:
        public string TrainPK { get; private set; }

        public string TimelinePK { get; private set; }

        public string TicketPricePK { get; private set; }

        public string TripPK { get; private set; }

        public string RailwayStationPK1 { get; private set; }

        public string RailwayStationPK2 { get; private set; }

        public string DepartureFromPK { get; private set; }

        public string ArrivalToPK { get; private set; }

        public string TicketPK { get; private set; }

        public void ComputeForeignKeys()
        {
            TrainPK = GetHash(TrainNumber, Category, RouteFrom, RouteTo);
            TimelinePK = GetHash(Timeline, Days);
            TicketPricePK = GetHash(PeriodFrom, PeriodTo, TicketRate, CoachType);
            TripPK = GetHash(DepartureDate, TrainPK);
            RailwayStationPK1 = GetHash(DepartureFrom, StationKmFrom);
            RailwayStationPK2 = GetHash(ArrivalTo, StationKmTo);
            DepartureFromPK = GetHash(DepartureFrom, StopDurationFrom, RailwayStationPK1);
            ArrivalToPK = GetHash(DepartureTimeTo, StopDurationTo, RailwayStationPK2);
            TicketPK = GetHash(Coach, Class, Seat, Food, Price, DepartureFromPK, ArrivalToPK, TripPK);
        }

        private string GetHash(params object[] relatedEntities)
        {
            byte[] hash;

            string str = string.Concat(relatedEntities);

            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(str);
                    writer.Flush();
                    stream.Position = 0;

                    hash = sha.ComputeHash(stream);
                }
            }

            return BitConverter.ToString(hash).Replace("-", "");
        }
    }
}
