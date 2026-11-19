using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SmallBackendApi.Models;
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
        // DAY 1 (Refactored for DAY 5): Async Test Connection & Create Table
        // --------------------------------------------------------
        [HttpGet("test-connection")]
        public async Task<IActionResult> TestConnectionAsync()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    // Converted to OpenAsync
                    await connection.OpenAsync();

                    string createTableQuery = @"
                        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Employees' AND xtype='U')
                        BEGIN
                            CREATE TABLE Employees (
                                Id INT PRIMARY KEY IDENTITY(1,1),
                                Name NVARCHAR(100) NOT NULL,
                                Salary DECIMAL(18,2) NOT NULL DEFAULT 0,
                                DepartmentId INT NULL FOREIGN KEY REFERENCES Departments(Id)
                            )
                        END";

                    using (var command = new SqlCommand(createTableQuery, connection))
                    {
                        // Converted to ExecuteNonQueryAsync
                        await command.ExecuteNonQueryAsync();
                    }

                    return Ok("Connection opened successfully! 'Employees' table is ready.");
                }
            }
            catch (SqlException ex)
            {
                // Explicit error handling
                return StatusCode(500, $"Oops! Could not connect to the database. Technical Details: {ex.Message}");
            }
        }

        // --------------------------------------------------------
        // DAY 2: Reading Data & Object Mapping
        // --------------------------------------------------------

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

        // --------------------------------------------------------
        // DAY 3: CRUD, ExecuteNonQuery & Safe Parameters
        // --------------------------------------------------------

        [HttpPost("employees")]
        public async Task<IActionResult> AddEmployeeAsync([FromBody] Employee employee)
        {
            if (string.IsNullOrWhiteSpace(employee.Name))
            {
                return BadRequest("Employee name is required.");
            }

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string query = "INSERT INTO Employees (Name, Salary) VALUES (@Name, @Salary)";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Name", employee.Name);
                        command.Parameters.AddWithValue("@Salary", employee.Salary);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        return Ok(new { Message = "Employee added successfully.", RowsAffected = rowsAffected });
                    }
                }
            }
            catch (SqlException ex)
            {
                return StatusCode(500, $"Database error: {ex.Message}");
            }
        }

        [HttpPut("employees/{id}")]
        public async Task<IActionResult> UpdateEmployeeAsync(int id, [FromBody] Employee employee)
        {
            if (id != employee.Id)
            {
                return BadRequest("Employee ID in the URL does not match the ID in the body.");
            }

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string query = "UPDATE Employees SET Name = @Name, Salary = @Salary WHERE Id = @Id";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        command.Parameters.AddWithValue("@Name", employee.Name);
                        command.Parameters.AddWithValue("@Salary", employee.Salary);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected == 0)
                        {
                            return NotFound($"No employee found with ID: {id}");
                        }

                        return Ok(new { Message = "Employee updated successfully.", RowsAffected = rowsAffected });
                    }
                }
            }
            catch (SqlException ex)
            {
                return StatusCode(500, $"Database error: {ex.Message}");
            }
        }

        [HttpDelete("employees/{id}")]
        public async Task<IActionResult> DeleteEmployeeAsync(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string query = "DELETE FROM Employees WHERE Id = @Id";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected == 0)
                        {
                            return NotFound($"No employee found with ID: {id}");
                        }

                        return Ok(new { Message = "Employee deleted successfully.", RowsAffected = rowsAffected });
                    }
                }
            }
            catch (SqlException ex)
            {
                return StatusCode(500, $"Database error: {ex.Message}");
            }
        }

        // --------------------------------------------------------
        // DAY 4: Relationships, Foreign Keys & JOIN Queries
        // --------------------------------------------------------

        [HttpGet("employees/{id}/department")]
        public async Task<IActionResult> GetEmployeeWithDepartmentAsync(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string query = @"
                        SELECT e.Id, e.Name, e.Salary, d.Name AS DepartmentName 
                        FROM Employees e 
                        INNER JOIN Departments d ON e.DepartmentId = d.Id 
                        WHERE e.Id = @Id";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var result = new
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    Salary = reader.GetDecimal(reader.GetOrdinal("Salary")),
                                    DepartmentName = reader.GetString(reader.GetOrdinal("DepartmentName"))
                                };

                                return Ok(result);
                            }
                        }
                    }
                }

                return NotFound($"No employee found or employee does not belong to a department. ID: {id}");
            }
            catch (SqlException ex)
            {
                return StatusCode(500, $"Database error: {ex.Message}");
            }
        }

        [HttpGet("departments/{departmentId}/employees")]
        public async Task<IActionResult> GetEmployeesByDepartmentAsync(int departmentId)
        {
            var employees = new List<object>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string query = @"
                        SELECT Id, Name, Salary 
                        FROM Employees 
                        WHERE DepartmentId = @DepartmentId 
                        ORDER BY Salary DESC";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@DepartmentId", departmentId);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                employees.Add(new
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

        [HttpGet("employees/{employeeId}/tasks")]
        public async Task<IActionResult> GetEmployeeTasksAsync(int employeeId)
        {
            var tasks = new List<object>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string query = @"
                        SELECT Id, Title 
                        FROM EmployeeTasks 
                        WHERE EmployeeId = @EmployeeId";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@EmployeeId", employeeId);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                tasks.Add(new
                                {
                                    TaskId = reader.GetInt32(reader.GetOrdinal("Id")),
                                    TaskTitle = reader.GetString(reader.GetOrdinal("Title"))
                                });
                            }
                        }
                    }
                }

                return Ok(tasks);
            }
            catch (SqlException ex)
            {
                return StatusCode(500, $"Database error: {ex.Message}");
            }
        }

        // --------------------------------------------------------
        // DAY 6: Transactions & Organizing the Data Access Layer
        // --------------------------------------------------------

        [HttpPost("employees/with-initial-task")]
        public async Task<IActionResult> CreateEmployeeWithTaskAsync([FromBody] Employee employee, [FromQuery] string taskTitle)
        {
            if (string.IsNullOrWhiteSpace(taskTitle))
            {
                return BadRequest("Task title is required in the query parameters.");
            }

            // Create an instance of the new DataAccess class
            var dataAccess = new SmallBackendApi.DataAccess.EmployeeDataAccess();

            bool isSuccess = await dataAccess.CreateEmployeeWithInitialTaskAsync(employee, taskTitle);

            if (isSuccess)
            {
                return Ok("Employee and initial task created successfully as one unit of work.");
            }
            else
            {
                return StatusCode(500, "Transaction failed. Employee insert was rolled back.");
            }
        }
    }
}