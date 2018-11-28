using System.Data;
using System.Data.SQLite;

namespace ThreePM.MusicLibrary
{
    public static class SQLiteHelper
    {
        public static DataSet ExecuteDataSet(string connString, string commandText)
        {
            using (var conn = new SQLiteConnection(connString))
            {
                conn.Open();

                DataSet result;

                var command = new SQLiteCommand(commandText, conn);
                result = new DataSet();
                var adapter = new SQLiteDataAdapter(command);
                adapter.Fill(result);

                conn.Close();
                return result;
            }
        }

        public static object ExecuteScalar(string conn, string commandText)
        {
            return ExecuteScalar(conn, commandText, null);
        }

        public static object ExecuteScalar(string connString, string commandText, SQLiteParameter[] parameters)
        {
            using (var conn = new SQLiteConnection(connString))
            {
                conn.Open();

                object result = null;

                var command = new SQLiteCommand(commandText, conn);
                if (parameters != null && parameters.Length > 0)
                {
                    command.Parameters.AddRange(parameters);
                }
                result = command.ExecuteScalar();

                conn.Close();
                return result;
            }
        }

        public static int ExecuteNonQuery(string conn, string commandText)
        {
            return ExecuteNonQuery(conn, commandText, null);
        }

        public static int ExecuteNonQuery(string connString, string commandText, SQLiteParameter[] parameters)
        {
            using (var conn = new SQLiteConnection(connString))
            {
                conn.Open();

                int result;

                var command = new SQLiteCommand(commandText, conn);
                if (parameters != null && parameters.Length > 0)
                {
                    command.Parameters.AddRange(parameters);
                }
                result = command.ExecuteNonQuery();

                conn.Close();
                return result;
            }
        }

        public static SQLiteDataReader ExecuteReader(string connString, string commandText)
        {
            using (var conn = new SQLiteConnection(connString))
            {
                conn.Open();

                var command = new SQLiteCommand(commandText, conn);
                SQLiteDataReader reader = command.ExecuteReader();
                conn.Close();
                return reader;
            }
        }
    }
}
