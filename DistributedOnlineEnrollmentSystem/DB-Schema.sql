-- Create and use the database
CREATE DATABASE IF NOT EXISTS OnlineEnrollmentSystem;
USE OnlineEnrollmentSystem;

-- Drop tables in reverse dependency order
DROP TABLE IF EXISTS Enrollments;
DROP TABLE IF EXISTS Courses;
DROP TABLE IF EXISTS Users;

-- Create Users table
CREATE TABLE Users (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    Username VARCHAR(100) NOT NULL,
    Password VARCHAR(255) NOT NULL,
    Role BOOLEAN NOT NULL  -- TRUE = Instructor, FALSE = Student
);

-- Create Courses table based on CourseModel
CREATE TABLE Courses (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    InstructorId INT NOT NULL,
    CourseCode VARCHAR(50) NOT NULL UNIQUE,
    Units INT NOT NULL,
    Capacity INT NOT NULL,
    FOREIGN KEY (InstructorId) REFERENCES Users(Id) ON DELETE CASCADE
);

-- Create Enrollments table based on EnrollmentModel
CREATE TABLE Enrollments (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    StudentId INT NOT NULL,
    CourseId INT NOT NULL,
    CourseCode VARCHAR(50) NOT NULL,
    Grade VARCHAR(10) DEFAULT 'NGA',
    FOREIGN KEY (StudentId) REFERENCES Users(Id) ON DELETE CASCADE,
    FOREIGN KEY (CourseId) REFERENCES Courses(Id) ON DELETE CASCADE,
    UNIQUE (StudentId, CourseId)
);


INSERT INTO Users (Username, Password, Role) VALUES
('christianmir', 'cmc', 0),
('dane-chan', 'dane', 0),
('broos', 'bruce123', 0),
('jonats', 'frostfenix', 1),
('rluy', 'roger', 1);

INSERT INTO Courses (InstructorId, CourseCode, Units, Capacity) VALUES
(4, 'ST-MATH', 3, 40),
(4, 'ST-INTSY', 3, 40),
(4, 'CS-OPESY', 3, 40),
(5, 'CS-ARCH1', 3, 25),
(4, 'ST-DISCM', 3, 45),
(5, 'THS-ST2', 3, 30);

INSERT INTO Enrollments (StudentId, CourseId, CourseCode, Grade) VALUES
(1, 5, 'ST-DISCM', '1.0'),
(1, 6, 'THS-ST2', '4.0'),
(2, 5, 'ST-DISCM', '4.0'),
(2, 1, 'ST-MATH', '3.0'),
(3, 2, 'ST-INTSY', '3.0'),
(3, 3, 'CS-OPESY', '4.0');
