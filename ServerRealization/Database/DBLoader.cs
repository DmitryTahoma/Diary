using MySql.Data.MySqlClient;
using ServerRealization.Database.Context;
using System.Collections.Generic;

namespace ServerRealization.Database
{
    public class DBLoader
    {
        string connectionString = "server=localhost;user=root;database=diaries;password=password";
        MySqlConnection connection;

        public DBLoader()
        {
            connection = new MySqlConnection(connectionString);
        }

        public object Load(string tableName)
        {
            connection.Open();

            MySqlCommand cmd = new MySqlCommand("select * from " + tableName, connection);
            MySqlDataReader result = cmd.ExecuteReader();
            List<IDBObject> loadedResult = ParseValuesFromDB(tableName, result);
            connection.Close();

            return ConvertToType(loadedResult, tableName);
        }

        private List<IDBObject> ParseValuesFromDB(string tableName, MySqlDataReader dr)
        {
            List<IDBObject> result = new List<IDBObject>();

            while (dr.Read())
            {
                switch(tableName.ToLower())
                {
                    default: throw new System.ArgumentException("Unknown table name");
                    case "images":              result.Add(new Image(dr.GetInt32(           0), (byte[])dr.GetValue(1), dr.GetInt32(    2), dr.GetInt32(    3))); break;
                    case "progresses":          result.Add(new Progress(dr.GetInt32(        0), dr.GetInt32(        1), dr.GetInt32(    2), dr.GetInt32(    3))); break;
                    case "collections":         result.Add(new Collection(dr.GetInt32(      0), dr.GetInt32(        1))); break;
                    case "users":               result.Add(new User(dr.GetInt32(            0), dr.GetString(       1), dr.GetString(   2), dr.GetString(   3), dr.GetDateTime( 4))); break;
                    case "labels":              result.Add(new Label(dr.GetInt32(           0), dr.GetInt32(        1), dr.GetString(   2))); break;
                    case "points":              result.Add(new Point(dr.GetInt32(           0), dr.GetInt32(        1), dr.GetString(   2), dr.GetBoolean(  3))); break;
                    case "notes":               result.Add(new Note(dr.GetInt32(            0), dr.GetInt32(        1), dr.GetInt32(    2), dr.GetString(   3), dr.GetString(   4), dr.GetDateTime(5), dr.GetDateTime(6))); break;
                    case "progressnotes":       result.Add(new ProgressNote(dr.GetInt32(    0), dr.GetInt32(        1), dr.GetInt32(    2), dr.GetInt32(    3))); break;
                    case "actions":             result.Add(new Action(dr.GetInt32(          0), dr.GetInt32(        1), dr.GetDateTime( 2), dr.GetDateTime( 3))); break;
                    case "missions":            result.Add(new Mission(dr.GetInt32(         0), dr.GetInt32(        1), dr.GetBoolean(  2), dr.GetInt32(    3))); break;
                    case "labelcollections":    result.Add(new LabelCollection(dr.GetInt32( 0), dr.GetInt32(        1), dr.GetInt32(    2))); break;
                }
            }
            dr.Close();
            return result;
        }

        private object ConvertToType(List<IDBObject> result, string tableName)
        {
            object res = null;
            for(int i = -1; i < result.Count; ++ i)
                switch(tableName.ToLower())
                {
                    default: throw new System.ArgumentException("Unknown table name");
                    case "images":              if(i == -1) res = new List<Image>();else
                        ((List<Image>)res).Add((Image)result[i]); break;
                    case "progresses":          if(i == -1) res = new List<Progress>();else
                        ((List<Progress>)res).Add((Progress)result[i]);break;
                    case "collections":         if(i == -1) res = new List<Collection>();else
                        ((List<Collection>)res).Add((Collection)result[i]);break;
                    case "users":               if(i == -1) res = new List<User>();else
                        ((List<User>)res).Add((User)result[i]);break;
                    case "labels":              if(i == -1) res = new List<Label>();else
                        ((List<Label>)res).Add((Label)result[i]);break;
                    case "points":              if(i == -1) res = new List<Point>();else
                        ((List<Point>)res).Add((Point)result[i]);break;
                    case "notes":               if(i == -1) res = new List<Note>();else
                        ((List<Note>)res).Add((Note)result[i]);break;
                    case "progressnotes":       if(i == -1) res = new List<ProgressNote>();else
                        ((List<ProgressNote>)res).Add((ProgressNote)result[i]);break;
                    case "actions":             if(i == -1) res = new List<Action>();else
                        ((List<Action>)res).Add((Action)result[i]);break;
                    case "missions":            if(i == -1) res = new List<Mission>();else
                        ((List<Mission>)res).Add((Mission)result[i]);break;
                    case "labelcollections":    if(i == -1) res = new List<LabelCollection>();else
                        ((List<LabelCollection>)res).Add((LabelCollection)result[i]);break;
                }
            return res;
        }
    }
}