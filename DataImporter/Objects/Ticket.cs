using System;

namespace DataImporter.Objects
{
    public class Ticket : DataObject
    {
        public int Coach { get; set; }

        public string Class { get; set; }

        public int Seat { get; set; }

        public bool Food { get; set; }

        public string DepartureFrom { get; set; }

        public string ArrivalTo { get; set; }

        public string Trip { get; set; }

        public static Ticket Parse(NonNormalizedTicket ticket)
        {
            return new Ticket
            {
                PrimaryKey = ticket.TicketPK,
                Coach = ticket.Coach,
                Class = ticket.Class,
                Seat = ticket.Seat,
                Food = ticket.Food,
                DepartureFrom = ticket.DepartureFromPK,
                ArrivalTo = ticket.ArrivalToPK,
                Trip = ticket.TripPK,
            };
        }
    }
}
