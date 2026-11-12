-- 1. Create Departments Table 
CREATE TABLE Departments (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL
);
GO

-- 2. Add DepartmentId to Employees as a Foreign Key 
ALTER TABLE Employees ADD DepartmentId INT NULL FOREIGN KEY REFERENCES Departments(Id);
GO

-- 3. Create EmployeeTasks Table with a Foreign Key to Employees
CREATE TABLE EmployeeTasks (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(200) NOT NULL,
    EmployeeId INT NOT NULL FOREIGN KEY REFERENCES Employees(Id)
);
GO

-- 4. Insert Dummy Data to test queries
INSERT INTO Departments (Name) VALUES ('IT'), ('HR'), ('Marketing');
GO

-- Update the employee you added earlier (Ahmed Ali with ID 1) to be in the IT department
UPDATE Employees SET DepartmentId = 1 WHERE Id = 1;
GO

-- Add some tasks for this employee
INSERT INTO EmployeeTasks (Title, EmployeeId) VALUES ('Setup Cloud Server', 1), ('Fix API Bug', 1);
GO

-- ============================================================
-- DAY 4 MANUAL TASK: Run at least one INNER JOIN and one LEFT JOIN
-- ============================================================

-- 5. INNER JOIN: Get employees who belong to a department
SELECT e.Name AS EmployeeName, e.Salary, d.Name AS DepartmentName
FROM Employees e
INNER JOIN Departments d ON e.DepartmentId = d.Id;

-- 6. LEFT JOIN: Get all departments, even if they have no employees yet
SELECT d.Name AS DepartmentName, e.Name AS EmployeeName
FROM Departments d
LEFT JOIN Employees e ON d.Id = e.DepartmentId;