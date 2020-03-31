using System;
using System.Data.SQLite;
using System.IO;
using System.Threading;

namespace Bank
{
    class Program
    {
        static void AllOutput(SQLiteConnection connect)
        {
            SQLiteCommand comandSQL;
            connect.Open();
            comandSQL = new SQLiteCommand("SELECT * FROM BankAccounts", connect);
            SQLiteDataReader reader = comandSQL.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine($@"
ID       :   {reader["id"]}
Login    :   {reader["Login"]}
Password :   {reader["Password"]}
Money    :   {reader["Money"]}");
            }
            reader.Close();
            connect.Close();
        }
        static void Log(SQLiteConnection connect, ref string Login, ref string Password)
        {
            //string Login = "", Password;
            SQLiteCommand comandSQL;
            SQLiteDataReader reader;
            bool Log = false, Pas = false;
            connect.Open();
            do
            {
                try
                {
                    Console.Clear();
                    if (!Log)
                    {
                        Console.Write("Введите Login : ");
                        Login = Console.ReadLine();
                        comandSQL = new SQLiteCommand($"SELECT (Login) FROM \"BankAccounts\"", connect);
                        reader = comandSQL.ExecuteReader();
                        while (reader.Read()) if ((string)reader["Login"] == Login) Log = true;
                        if (!Log) throw new Exception("Не существует пользователя с таким Login");
                    }
                    else Console.WriteLine($"Введите Login : {Login}");
                    comandSQL = new SQLiteCommand($"SELECT * FROM \"BankAccounts\" WHERE \"Login\" = \"{Login}\"", connect);
                    reader = comandSQL.ExecuteReader();
                    reader.Read();
                    Console.Write("Введите Password : ");
                    Password = Console.ReadLine();
                    if (Password != (string)reader["Password"]) throw new Exception("Неправильный пароль");
                    Pas = false;
                }
                catch (Exception Error)
                {
                    Console.WriteLine($@"Ошибка : {Error.Message}
Пожалуйста, повторите ввод");
                    Pas = true;
                    Thread.Sleep(500);
                }
            }
            while (Pas || !Log);
            connect.Close();
        }
        static string[] InfInput()
        {
            string[] information;
            try
            {
                Console.Clear();
                Console.WriteLine(@"Введите логин, пароль и начальный счет клиента
Пример : Deezbec MyBirthday 2000");
                information = Console.ReadLine().Split();
                if (information.Length != 3) throw new Exception("Было введено не 3 значения");
                if (!Int32.TryParse(information[2], out int x)) throw new Exception("Счёт не может состоять из букв");
            }
            catch (Exception Error)
            {
                Console.WriteLine($@"Ошибка : {Error.Message}
Пожалуйста, повторите ввод");
                Thread.Sleep(500);
                information = InfInput();
            }
            return information;
        }
        static void IfNoBD(string directory, SQLiteConnection connect)
        {
            int n = 0;
            bool action = false;
            SQLiteConnection.CreateFile(@$"{directory}");
            string[] information;
            Console.WriteLine($@"К сожалению, в пути {directory} не была найдена БД
So нам нужно создать новую");
            Console.WriteLine("Введите количество клиентов банка");
            do
            {
                try { if (!Int32.TryParse(Console.ReadLine(), out n) || n <= 0) throw new Exception("Неправильно введено количество клиентов"); action = false; }
                catch (Exception Error) { Console.WriteLine($"Ошибка : {Error.Message} \n Повторите ввод"); action = true; }
            }
            while (action);
            connect.Open();
            SQLiteCommand comandSQL = new SQLiteCommand("CREATE TABLE IF NOT EXISTS \"BankAccounts\"" + "(\"id\" INTEGER PRIMARY KEY AUTOINCREMENT, \"Login\" TEXT, \"Password\" TEXT, \"Money\" INTEGER);", connect);
            for (int i = 0; i < n; i++)
            {
                comandSQL.ExecuteNonQuery();
                information = InfInput();
                comandSQL = new SQLiteCommand($"INSERT INTO \"BankAccounts\" (\"Login\", \"Password\", \"Money\") " + $"VALUES (\"{information[0]}\", \"{information[1]}\", {Convert.ToInt32(information[2])})", connect);
                Console.Clear();
            }
            comandSQL.ExecuteNonQuery();
            connect.Close();
        }
        static void Main(string[] args)
        {
            string directory = "C:\\User\\!Kirill\\GitHub\\SQL_Classes\\Bank.db", Login = "", Password = ""; //Дерикторию сам поменяешь
            SQLiteConnection connect = new SQLiteConnection(@$"Data Source = {directory}; Version = 3");
            if (!File.Exists(directory)) IfNoBD(directory, connect);
            Log(connect, ref Login, ref Password);
        }
    }
}
