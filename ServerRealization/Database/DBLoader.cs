using MySql.Data.MySqlClient;
using ServerRealization.Database.Context;
using System.Collections.Generic;

namespace ServerRealization.Database
{
    class DBLoader
    {
        string connectionString = "server=localhost;user=root;database=diaries;password=password";
        MySqlConnection connection;

        public DBLoader()
        {
            connection = new MySqlConnection(connectionString);
        }

        public List<IDBObject> Load(string tableName)
        {
            List<IDBObject> objs = null;

            connection.Open();

            MySqlCommand cmd = new MySqlCommand("select * from " + tableName, connection);
            MySqlDataReader result = cmd.ExecuteReader();
            objs = ParseValuesFromDB(tableName, result);

            connection.Close();

            return objs;
        }

        private List<IDBObject> ParseValuesFromDB(string tableName, MySqlDataReader dataReader)
        {
            List<IDBObject> objects = new List<IDBObject>();

            while (dataReader.Read())
            {
                switch (tableName)
                {
                    case "time_span": 
                        objects.Add(new TimeSpan(dataReader.GetInt32(0), dataReader.GetDateTime(1), dataReader.GetDateTime(2))); break;
                    case "names_and_comments": 
                        objects.Add(new NameAndComment(dataReader.GetInt32(0), dataReader.GetString(1), dataReader.GetString(2))); break;
                    case "names":
                        objects.Add(new Name(dataReader.GetInt32(0), dataReader.GetString(1), dataReader.GetString(2), dataReader.GetString(3))); break;
                    case "progress": 
                        objects.Add(new Progress(dataReader.GetInt32(0), dataReader.GetDouble(1), dataReader.GetDouble(2), dataReader.GetDouble(3))); break;
                    case "users": 
                        objects.Add(new User(dataReader.GetInt32(0), dataReader.GetInt32(1), dataReader.GetString(2), dataReader.GetString(3), dataReader.GetDateTime(4))); break;
                    case "missions":
                        objects.Add(new Mission(dataReader.GetInt32(0), dataReader.GetInt32(1), dataReader.GetInt32(2))); break;
                    case "action": 
                        objects.Add(new Action(dataReader.GetInt32(0), dataReader.GetInt32(1), dataReader.GetInt32(2), dataReader.GetInt32(3))); break;
                    case "records": 
                        objects.Add(new Record(dataReader.GetInt32(0), dataReader.GetInt32(1), dataReader.GetDouble(2))); break;
                    case "calculated_actions":
                        objects.Add(new CalculatedAction(dataReader.GetInt32(0), dataReader.GetInt32(1), dataReader.GetString(2), dataReader.GetInt32(3))); break;
                }
            }
            dataReader.Close();

            return objects;
        }
    }
}
