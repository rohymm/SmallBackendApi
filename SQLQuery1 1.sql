DELETE FROM EmployeeTasks;
DELETE FROM Employees;
DELETE FROM Departments;

INSERT INTO Departments (Name) VALUES ('IT'), ('HR');

INSERT INTO Employees (Name, Salary, DepartmentId, HireDate) VALUES
('Ahmed', 9000, (SELECT Id FROM Departments WHERE Name = 'IT'), GETDATE()),
('Sara',  12000, (SELECT Id FROM Departments WHERE Name = 'IT'), GETDATE()),
('Omar',  7000, (SELECT Id FROM Departments WHERE Name = 'HR'), GETDATE());

INSERT INTO EmployeeTasks (Title, IsCompleted, EmployeeId) VALUES
('Build API', 0, (SELECT Id FROM Employees WHERE Name = 'Ahmed')),
('Fix bug', 1, (SELECT Id FROM Employees WHERE Name = 'Sara')),
('Write report', 0, (SELECT Id FROM Employees WHERE Name = 'Omar'));