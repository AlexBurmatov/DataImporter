using System;

namespace DataImporter.Objects
{
    public class RailwayStation : DataObject
    {
        public string Name { get; set; }

        public int Kilometer { get; set; }

        public static RailwayStation Parse(NonNormalizedTicket ticket, int type)
        {
            if (type == 1)
            {
                return new RailwayStation
                {
                    PrimaryKey = ticket.RailwayStationPK1,
                    Name = ticket.DepartureFrom,
                    Kilometer = ticket.StationKmFrom,
                };
            }
            else
            {
                return new RailwayStation
                {
                    PrimaryKey = ticket.RailwayStationPK2,
                    Name = ticket.ArrivalTo,
                    Kilometer = ticket.StationKmTo,
                };
            }
        }
    }
}
