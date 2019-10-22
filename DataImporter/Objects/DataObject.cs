using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataImporter.Objects
{
    public class DataObject
    {
        [Key]
        public string PrimaryKey { get; set; }
    }

    public class DataObjectEqualityComparer : IEqualityComparer<DataObject>
    {
        public bool Equals(DataObject x, DataObject y)
        {
            return x.PrimaryKey == y.PrimaryKey;
        }

        public int GetHashCode(DataObject obj)
        {
            return obj.PrimaryKey.GetHashCode();
        }
    }
}
