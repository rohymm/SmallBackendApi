using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient; 
using System;

namespace SmallBackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdoNetController : ControllerBase
    {
        private readonly string _connectionString = @"Server=(localdb)\mssqllocaldb;Database=EmployeeManagement;Trusted_Connection=True;TrustServerCertificate=True;";

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
                                Name NVARCHAR(100) NOT NULL
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
    }
}