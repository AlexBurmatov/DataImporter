using System;
using System.Collections.Generic;
using System.Text;
using Npgsql;
using DataImporter.Objects;
using System.Data;
using System.Linq;

namespace DataImporter
{
    public static class DataService
    {
        private static NpgsqlConnection connection { get; set; }

        private static string SqlBody { get; set; }

        public static void Connect(string connString)
        {
            connection = new NpgsqlConnection(connString);
            connection.Open();
        }

        public static void InsertOrUpdate<T>(IEnumerable<T> objects)
            where T : DataObject
        {
            Type objType = typeof(T);

            string sql = string.Empty;

            string columns = string.Join(',', objType.GetProperties().Select(x => x.Name));

            foreach (var obj in objects)
            {
                var propValues = objType.GetProperties().Select(x => x.GetValue(obj));

                var stringValues = new List<string>();
                propValues.ToList().ForEach((x) => { 
                    if (x == null) 
                        stringValues.Add("null"); 
                    else if(x is DateTime dt)
                        stringValues.Add("'" + dt.ToString("yyyy-MM-dd hh:mm:ss") + "'");
                    else 
                        stringValues.Add("'" + x + "'"); });

                string sqlValues = string.Join(',', stringValues);

                sql += $"INSERT INTO {objType.Name}({columns}) VALUES({sqlValues}) ON CONFLICT (primarykey) DO NOTHING; ";
            }

            SqlBody += sql;
        }

        public static void SaveChanges()
        {
            SqlBody = "begin; " + SqlBody;
            SqlBody += " commit;";

            var cmd = new NpgsqlCommand(SqlBody, connection);

            using (var reader = cmd.ExecuteReader()) { };

            SqlBody = string.Empty;
        }
    }
}
