using System;

namespace DataImporter.Objects
{
    public class TicketPrice : DataObject
    {
        public DateTime PeriodFrom { get; set; }

        public DateTime PeriodTo { get; set; }

        public string CoachType { get; set; }

        public double Rate { get; set; }

        public string Train { get; set; }

        public static TicketPrice Parse(NonNormalizedTicket ticket)
        {
            return new TicketPrice
            {
                PrimaryKey = ticket.TicketPricePK,
                PeriodFrom = ticket.PeriodFrom,
                PeriodTo = ticket.PeriodTo,
                CoachType = ticket.CoachType,
                Rate = ticket.TicketRate,
                Train = ticket.TrainPK,
            };
        }
    }
}
