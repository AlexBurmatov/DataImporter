using System;

namespace DataImporter.Objects
{
    public class Trip : DataObject
    {
        public DateTime DepartureDate { get; set; }

        public string Train { get; set; }

        public static Trip Parse(NonNormalizedTicket ticket)
        {
            return new Trip
            {
                PrimaryKey = ticket.TripPK,
                DepartureDate = ticket.DepartureDate,
                Train = ticket.TrainPK,
            };
        }
    }
}
