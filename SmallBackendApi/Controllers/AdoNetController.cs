using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SmallBackendApi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmallBackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdoNetController : ControllerBase
    {
        // Connection string pointing to the EmployeeManagement database
        private readonly string _connectionString = @"Server=(localdb)\mssqllocaldb;Database=EmployeeManagement;Trusted_Connection=True;TrustServerCertificate=True;";

        // --------------------------------------------------------
        // DAY 1: Test Connection & Create Table
        // --------------------------------------------------------
        [HttpGet("test-connection")]
        public IActionResult TestConnection()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string createTableQuery = @"
                        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Employees' AND xtype='U')
                        BEGIN
                            CREATE TABLE Employees (
                                Id INT PRIMARY KEY IDENTITY(1,1),
                                Name NVARCHAR(100) NOT NULL,
                                Salary DECIMAL(18,2) NOT NULL DEFAULT 0
                            )
                        END";

                    using (SqlCommand command = new SqlCommand(createTableQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    return Ok("Connection opened successfully! 'Employees' table is ready.");
                }
            }
            catch (SqlException ex)
            {
                return StatusCode(500, $"Oops! Could not connect to the database. Friendly Error: {ex.Message}");
            }
        }

        // --------------------------------------------------------
        // DAY 2: Reading Data & Object Mapping
        // --------------------------------------------------------

        // 1. Retrieve all employees and map to List<Employee>
        [HttpGet("employees")]
        public async Task<IActionResult> GetAllEmployeesAsync()
        {
            var employees = new List<Employee>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string query = "SELECT Id, Name, Salary FROM Employees";

                    using (var command = new SqlCommand(query, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            
                            while (await reader.ReadAsync())
                            {
                                var employee = new Employee
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    Salary = reader.GetDecimal(reader.GetOrdinal("Salary"))
                                };

                                employees.Add(employee);
                            }
                        }
                    }
                }

                return Ok(employees);
            }
            catch (SqlException ex)
            {
                return StatusCode(500, $"Database error: {ex.Message}");
            }
        }

        // 2. Retrieve a single employee by ID using a parameterized query
        [HttpGet("employees/{id}")]
        public async Task<IActionResult> GetEmployeeByIdAsync(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string query = "SELECT Id, Name, Salary FROM Employees WHERE Id = @Id";

                    using (var command = new SqlCommand(query, connection))
                    {
                        
                        command.Parameters.AddWithValue("@Id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var employee = new Employee
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    Salary = reader.GetDecimal(reader.GetOrdinal("Salary"))
                                };

                                return Ok(employee);
                            }
                        }
                    }
                }

                return NotFound($"No employee found with ID: {id}");
            }
            catch (SqlException ex)
            {
                return StatusCode(500, $"Database error: {ex.Message}");
            }
        }

        // 3. Retrieve employees with a salary greater than a specific value
        [HttpGet("employees/salary-greater-than/{salary}")]
        public async Task<IActionResult> GetEmployeesWithSalaryGreaterThanAsync(decimal salary)
        {
            var employees = new List<Employee>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string query = "SELECT Id, Name, Salary FROM Employees WHERE Salary > @Salary";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Salary", salary);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                employees.Add(new Employee
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    Salary = reader.GetDecimal(reader.GetOrdinal("Salary"))
                                });
                            }
                        }
                    }
                }

                return Ok(employees);
            }
            catch (SqlException ex)
            {
                return StatusCode(500, $"Database error: {ex.Message}");
            }
        }
    }
}