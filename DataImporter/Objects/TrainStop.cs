using System;

namespace DataImporter.Objects
{
    public class TrainStop : DataObject
    {
        public DateTime DepartureTime { get; set; }

        public int Duration { get; set; }

        public string RailwayStation { get; set; }

        public string Train { get; set; }

        public static TrainStop Parse(NonNormalizedTicket ticket, int type)
        {
            if (type == 1)
            {
                return new TrainStop
                {
                    PrimaryKey = ticket.DepartureFromPK,
                    DepartureTime = ticket.DepartureDate.Add(ticket.DepartureTimeFrom),
                    Duration = ticket.StopDurationFrom,
                    RailwayStation = ticket.RailwayStationPK1,
                    Train = ticket.TrainPK,
                };
            }
            else
            {
                return new TrainStop
                {
                    PrimaryKey = ticket.ArrivalToPK,
                    DepartureTime = ticket.DepartureDate.Add(ticket.DepartureTimeTo),
                    Duration = ticket.StopDurationTo,
                    RailwayStation = ticket.RailwayStationPK2,
                    Train = ticket.TrainPK,
                };
            }
        }
    }
}
