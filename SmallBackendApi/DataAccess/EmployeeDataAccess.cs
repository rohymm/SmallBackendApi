using Microsoft.Data.SqlClient;
using SmallBackendApi.Models;
using System;
using System.Threading.Tasks;

namespace SmallBackendApi.DataAccess
{
    public class EmployeeDataAccess
    {
        // Connection string defined directly in the class
        private string _connectionString = @"Server=(localdb)\mssqllocaldb;Database=EmployeeManagement;Trusted_Connection=True;TrustServerCertificate=True;";

        public async Task<bool> CreateEmployeeWithInitialTaskAsync(Employee employee, string taskTitle)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Begin the local transaction
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 1. Insert Employee and return the generated ID
                        string empQuery = @"INSERT INTO Employees (Name, Salary, DepartmentId) 
                                            OUTPUT INSERTED.Id 
                                            VALUES (@Name, @Salary, @DepartmentId)";

                        int newEmployeeId = 0;

                        using (var command = new SqlCommand(empQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@Name", employee.Name);
                            command.Parameters.AddWithValue("@Salary", employee.Salary);

                            if (employee.DepartmentId > 0)
                            {
                                command.Parameters.AddWithValue("@DepartmentId", employee.DepartmentId);
                            }
                            else
                            {
                                command.Parameters.AddWithValue("@DepartmentId", DBNull.Value);
                            }

                            // ExecuteScalar returns the first column of the first row (the new ID)
                            newEmployeeId = (int)await command.ExecuteScalarAsync();
                        }

                        // 2. Insert the initial EmployeeTask using the new Employee ID
                        string taskQuery = "INSERT INTO EmployeeTasks (Title, EmployeeId) VALUES (@Title, @EmployeeId)";

                        using (var taskCommand = new SqlCommand(taskQuery, connection, transaction))
                        {
                            taskCommand.Parameters.AddWithValue("@Title", taskTitle);
                            taskCommand.Parameters.AddWithValue("@EmployeeId", newEmployeeId);

                            await taskCommand.ExecuteNonQueryAsync();
                        }

                        // Commit transaction only when every required operation succeeds
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception)
                    {
                        // Roll back if any step fails
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }
    }
}