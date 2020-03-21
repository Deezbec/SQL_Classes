using System;
using System.Data.SQLite;
using System.IO;

namespace SQL
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!File.Exists(@"C:\User\!Kirill\SQL_Classes\Test.db"))
            {
                SQLiteConnection.CreateFile(@"C:\User\!Kirill\SQL_Classes\Test.db");
                Console.WriteLine("Создана БД");
            }
            else Console.WriteLine("БД уже создана");
            SQLiteConnection connect = new SQLiteConnection(@"Data Source=C:\User\!Kirill\SQL_Classes\Test.db; Version=3;");
            connect.Open();

            SQLiteCommand comandSQL;
            comandSQL = new SQLiteCommand("CREATE TABLE IF NOT EXISTS \"Employees\"" + "(\"id\" INTEGER PRIMARY KEY AUTOINCREMENT, \"name\" TEXT, \"section\" TEXT);", connect);
            comandSQL.ExecuteNonQuery();
            comandSQL = new SQLiteCommand("INSERT INTO \"Employees\" (\"name\", \"section\") " + "VALUES (\"KIRILL\", \"RemoteControl\")", connect);
            comandSQL.ExecuteNonQuery();
            comandSQL = new SQLiteCommand("SELECT * FROM \"Employees\"", connect);
            SQLiteDataReader reader = comandSQL.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine("ID " + reader["id"] + " NAME " + reader["name"]
                    + " INFO " + reader["section"]);
            }
            reader.Close();
            connect.Close(); 
            Console.ReadLine();
        }
    }
}
