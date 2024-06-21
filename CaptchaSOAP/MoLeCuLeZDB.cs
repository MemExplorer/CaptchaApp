using System;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;

namespace CaptchaSOAP
{
    public class MoLeCuLeZDB : IDisposable
    {
        // Regex pattern that captures all single question mark that is not inside a double single-quote statement
        private static Regex PLACEHOLDER_REPLACE_PATTERN = new Regex("(?<!\\?)\\?(?!\\?)(?=(?:[^']*'[^']*')*[^']*$)", RegexOptions.Compiled);
        private MySqlConnection _connection;
        private MoLeCuLeZDB()
        {
            // initialize database connection
            string fConnStr = "server=localhost;uid=root;database=moleculezdb";
            _connection = new MySqlConnection(fConnStr);
            _connection.Open();
            if (_connection.State != System.Data.ConnectionState.Open)
                throw new Exception("Database connection Error!");
        }

        internal static MoLeCuLeZDB GetTransient() => new MoLeCuLeZDB();

        private MySqlCommand PrepareCommand(string query, params object[] sqlArgs)
        {
            int replaceCounter = 0;
            string preprocessedCmd = PLACEHOLDER_REPLACE_PATTERN.Replace(query, m => "@" + replaceCounter++);
            MySqlCommand sqlCmd = new MySqlCommand(query, _connection);
            for (int i = 0; i < sqlArgs.Length; i++)
                sqlCmd.Parameters.AddWithValue("@" + i, sqlArgs[i]);

            return sqlCmd;
        }

        public int ExecuteNonQuery(string query, params object[] sqlArgs)
        {
            using (MySqlCommand _command = PrepareCommand(query, sqlArgs))
                return _command.ExecuteNonQuery();
        }

        public MySqlDataReader ExecuteReader(string query, params object[] sqlArgs)
        {
            using (MySqlCommand _command = PrepareCommand(query, sqlArgs))
                return _command.ExecuteReader();
        }

        // Explicitly use the appropriate data type
        public SqlDataType ExecuteScalar<SqlDataType>(string query, params object[] sqlArgs)
        {
            using (MySqlCommand _command = PrepareCommand(query, sqlArgs))
                return (SqlDataType)_command.ExecuteScalar();
        }

        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}