using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;
using System.Data;
using System.Xml.Linq;

namespace TestEmployeeSalary
{
    internal class EmployeeDAL
    {

        private readonly string CS;

        internal EmployeeDAL(string CS)
        {
            this.CS=CS;
        }

        internal int UpdateEmployeeSalary(int empId, int newSalary)
        {
            using MySqlConnection conn = new MySqlConnection(CS);
            using MySqlCommand cmd = new MySqlCommand("UpdateEmployeeSalary", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("?empID", empId);
            cmd.Parameters["?empID"].Direction = ParameterDirection.Input;
            cmd.Parameters.AddWithValue("?newSalary", newSalary);
            cmd.Parameters["?newSalary"].Direction = ParameterDirection.Input;

            cmd.Parameters.Add("?statusCode", MySqlDbType.Int32);
            cmd.Parameters["?statusCode"].Direction = ParameterDirection.Output;
            cmd.Parameters.Add("?oldSalary", MySqlDbType.Int32);
            cmd.Parameters["?oldSalary"].Direction = ParameterDirection.Output;

            conn.Open();
            cmd.ExecuteNonQuery();
            int statusCode = (int)cmd.Parameters["?statusCode"].Value;
            if (statusCode==((int)EployeeErrorCodes.EMPLOYEENOTFOUND))
            {
                throw new InvalidDataException($"Employee with id {empId} does not exist");
            }
            return (int)cmd.Parameters["?oldSalary"].Value;
        }


        internal int CreateEmployee(string name, int salary)
        {
            string sql = @"
               INSERT INTO employees (name,salary)
               VALUES(?name,?salary)";

            using var conn = new MySqlConnection(CS);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("?name", name);
            cmd.Parameters.AddWithValue("?salary", salary);
            conn.Open();
            cmd.ExecuteNonQuery();
            return (int)cmd.LastInsertedId;
        }


        internal int GetEmployeeSalary(int empId)
        {
            using var conn = new MySqlConnection(CS);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT salary FROM employees WHERE id=?id";
            cmd.Parameters.AddWithValue("?id", empId);

            conn.Open();
            var reader = cmd.ExecuteReader();
            reader.Read();
            return reader.GetInt32("salary");
        }
    }
}
