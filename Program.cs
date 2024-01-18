using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace CompanySQL
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string connectionString = "Server=DESKTOP-5BD88QO\\SQLEXPRESS;Database=CompanyDB;Trusted_Connection=True;";

            // Создание подключения
            SqlConnection connection = new SqlConnection(connectionString);
            SqlDataReader reader = null;
            try
            {
                // Открываем подключение
                await connection.OpenAsync();
                Console.WriteLine("Подключение открыто");
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }

            ConsoleKeyInfo key;
            do
            {
                Console.Clear();
                Console.WriteLine("R - ReadInfo from DB\nU - Update salary\nD - Delete employee\nI - Insert new employee\nQ - Exit");
                key = Console.ReadKey();
                Console.Clear();
                switch (key.Key)
                {
                    case ConsoleKey.R:
                        reader = await GetGETCommand(connection).ExecuteReaderAsync();
                        ReadEmployess(reader);
                        break;
                    case ConsoleKey.U:
                        UpdateSalary(connection);
                        break;
                    case ConsoleKey.D:
                        RemoveEmployee(connection);
                        break;
                    case ConsoleKey.I:
                        InsertEmployee(connection);
                        break;
                }
                Console.WriteLine("Press any key");
                Console.ReadKey();
            }
            while (key.Key != ConsoleKey.Q);

                        // если подключение открыто
            if (connection.State == ConnectionState.Open)
            {
                // закрываем подключение
                await connection.CloseAsync();
            }
        }
        static SqlCommand GetGETCommand(SqlConnection connection) => new SqlCommand("select * from Employess", connection);
        static async void ReadEmployess(SqlDataReader reader) 
        {
            if (reader != null)
            {
                // выводим названия столбцов
                Console.WriteLine("{0,3}{1,20}{2,20}{3,20}{4,20}", reader.GetName(0), reader.GetName(1), reader.GetName(2), reader.GetName(3), reader.GetName(4));

                while (await reader.ReadAsync()) // построчно считываем данные
                {
                    object id = reader.GetValue(0);
                    object firstName = reader.GetValue(1);
                    object lastName = reader.GetValue(2);
                    object Position = reader.GetValue(3);
                    object Salary = reader.GetValue(4);

                    Console.WriteLine("{0,3}{1,20}{2,20}{3,20}{4,20}", id, firstName, lastName, Position, Salary);
                }
                reader.CloseAsync();
            }

        }
        static async void UpdateSalary(SqlConnection connection) 
        {
            int employeeID = GetEmployeeID();
            if (employeeID < 0) { IncorectInput(); return; }
            double salary = GetSalary();
            if (salary < 0) { IncorectInput(); return; }
            SqlCommand command = new SqlCommand($"UPDATE Employess SET Salary = {salary.ToString().Replace(',', '.')} WHERE EmployeeID = {employeeID}", connection);
            command.ExecuteNonQuery();
        }
        static int GetEmployeeID() 
        {
            Console.WriteLine("Enter the EmployeeID");
            string employeeIDInput = Console.ReadLine();
            int employeeID = 0;
            if (!int.TryParse(employeeIDInput, out employeeID)) { IncorectInput(); return -1; }
            employeeID = int.Parse(employeeIDInput);
            return employeeID;
        }
        static void IncorectInput() { Console.WriteLine("Incorrect input"); }
        static double GetSalary() 
        {
            Console.WriteLine("Enter salary");
            string salaryInput = Console.ReadLine();
            double salary = 0;
            if (!double.TryParse(salaryInput, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out salary)) { IncorectInput(); return -1; }
            salary = double.Parse(salaryInput);
            return salary;
        }
        static async void RemoveEmployee(SqlConnection connection) 
        {
            int employeeID = GetEmployeeID();
            if (employeeID < 0) { IncorectInput(); return; }
            SqlCommand command = new SqlCommand($"DELETE FROM Employess WHERE EmployeeID = {employeeID}", connection);
            command.ExecuteNonQuery();
        }
        static async void InsertEmployee(SqlConnection connection) 
        {
            Console.WriteLine("Enter the firstname");
            string FirstName = Console.ReadLine();
            Console.WriteLine("Enter the lastname");
            string lastName = Console.ReadLine();
            Console.WriteLine("Enter the position");
            string post = Console.ReadLine();
            double salary = GetSalary();
            if (salary < 0) { IncorectInput(); return; }
            SqlCommand command = new SqlCommand($"INSERT INTO Employess (FirstName, LastName, Position, Salary) VALUES('{FirstName}','{lastName}','{post}', {salary})", connection);
            command.ExecuteNonQuery();
        }
    }
}