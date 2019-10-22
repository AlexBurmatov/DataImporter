using System;
using System.Collections.Generic;
using System.Text;

namespace SenderApp
{
    public class Ticket
    {
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
    }
}
