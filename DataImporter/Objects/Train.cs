using System;

namespace DataImporter.Objects
{
    public class Train : DataObject
    {
        public int Number { get; set; }

        public string Category { get; set; }

        public string RouteFrom { get; set; }

        public string RouteTo { get; set; }

        public string Timeline { get; set; }

        public static Train Parse(NonNormalizedTicket ticket)
        {
            return new Train
            {
                PrimaryKey = ticket.TrainPK,
                Number = ticket.TrainNumber,
                Category = ticket.Category,
                RouteFrom = ticket.RouteFrom,
                RouteTo = ticket.RouteTo,
                Timeline = ticket.TimelinePK,
            };
        }
    }
}
