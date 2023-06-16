using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TestEmployeeSalary
{
    internal class DbSetup
    {
        private string CS;
        internal DbSetup(string CS) {
            this.CS = CS;
        }

        internal void Initialize(string dbName)
        {
            DropDatabase(dbName);
            CreateDatabase(dbName);
            CreateTable();
            CreateProcedure();
        }

        internal void DropDatabase(string name)
        {
            using var conn = new MySqlConnection(CS);
            using var cmd = conn.CreateCommand();
            conn.Open();
            cmd.CommandText = $"DROP DATABASE IF EXISTS `{name}`;";
            cmd.ExecuteNonQuery();
        }

        private void CreateDatabase(string name)
        {
            using var conn = new MySqlConnection(CS);
            using var cmd = conn.CreateCommand();
            conn.Open();
            cmd.CommandText = $"CREATE DATABASE IF NOT EXISTS `{name}`;";
            cmd.ExecuteNonQuery();
            CS += $"database={name};";
        }

        private void CreateTable()
        {
            string sql = @"
                CREATE TABLE employees (
                    id     INT PRIMARY KEY AUTO_INCREMENT,
                    name   VARCHAR(50) NOT NULL,
                    salary INT         NOT NULL
                )";

            using var conn = new MySqlConnection(CS);
            using var cmd = conn.CreateCommand();
            conn.Open();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }


        private void CreateProcedure()
        {
            string sql = @"
                CREATE PROCEDURE UpdateEmployeeSalary(
                    IN empID INT,
                    IN newSalary INT,
                    OUT oldSalary INT,
                    OUT statusCode INT
                )
                   main: BEGIN

                        DECLARE storedId INT;

                        SELECT
                            id,
                            salary
                        INTO storedId, oldSalary
                        FROM employees
                        WHERE id = empID FOR UPDATE;

                        IF storedId IS NULL
                        THEN
                            /* Employee not found */
                            SET statusCode = 1;
                            LEAVE main;
                        END IF;

                        UPDATE employees
                        SET salary = newSalary
                        WHERE id = empID;
                        COMMIT;
                        SET statusCode = 0;
                    END ";

            using var conn = new MySqlConnection(CS);
            using var cmd = conn.CreateCommand();
            conn.Open();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }


    }
}
