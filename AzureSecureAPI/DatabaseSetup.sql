-- Create Employee table
CREATE TABLE Employee (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Department NVARCHAR(50) NOT NULL,
    Salary DECIMAL(10,2) NOT NULL
);

-- Insert sample data
INSERT INTO Employee (Name, Email, Department, Salary) VALUES
('John Doe', 'john.doe@example.com', 'IT', 75000),
('Jane Smith', 'jane.smith@example.com', 'HR', 65000),
('Bob Johnson', 'bob.johnson@example.com', 'Sales', 70000),
('Alice Williams', 'alice.williams@example.com', 'Marketing', 68000),
('Charlie Brown', 'charlie.brown@example.com', 'IT', 80000),
('Diana Davis', 'diana.davis@example.com', 'Finance', 72000),
('Edward Wilson', 'edward.wilson@example.com', 'Sales', 65000),
('Fiona Garcia', 'fiona.garcia@example.com', 'HR', 63000),
('George Martinez', 'george.martinez@example.com', 'IT', 77000),
('Helen Anderson', 'helen.anderson@example.com', 'Marketing', 70000);

-- Create index on Email for better performance
CREATE INDEX IX_Employee_Email ON Employee(Email);

-- Create index on Department for better performance
CREATE INDEX IX_Employee_Department ON Employee(Department);