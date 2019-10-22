using System;

namespace DataImporter.Objects
{
    public class Timeline : DataObject
    {
        public string Kind { get; set; }

        public string Days { get; set; }

        public static Timeline Parse(NonNormalizedTicket ticket)
        {
            return new Timeline
            {
                PrimaryKey = ticket.TimelinePK,
                Kind = ticket.Timeline,
                Days = ticket.Days,
            };
        }
    }
}
