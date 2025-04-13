DROP TABLE IF EXISTS Users, Courses, Enrollments;

CREATE TABLE Users (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    Username VARCHAR(100) NOT NULL,
    Password VARCHAR(255) NOT NULL,  -- Assuming the password might be hashed
    Role BOOLEAN NOT NULL  -- Assuming boolean values, use TINYINT if necessary
);

CREATE TABLE Courses (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    InstructorId INT NOT NULL,
    CourseCode VARCHAR(50) NOT NULL,
    Units INT NOT NULL,
    Capacity INT NOT NULL
);

CREATE TABLE Enrollments (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    StudentId INT NOT NULL,
    CourseId INT NOT NULL,
    CourseCode VARCHAR(100) NOT NULL,
    Grade VARCHAR(10) NOT NULL,
    FOREIGN KEY (CourseId) REFERENCES Courses(Id) ON DELETE CASCADE
);

INSERT INTO Users (Username, Password, Role) VALUES
('student', 'password', 0),
('instructor', 'password', 1),
('instructor2', 'password', 1),
('instructor3', 'password', 1),
('instructor4', 'password', 1),
('student2', 'password', 0);

INSERT INTO Courses (InstructorId, CourseCode, Units, Capacity) VALUES
(2, 'ST-MATH', 3, 40),
(2, 'ST-INTSY', 3, 40),
(3, 'CS-OPESY', 3, 40),
(4, 'CS-ARCH1', 3, 25);

INSERT INTO Enrollments (StudentId, CourseId, CourseCode, Grade) VALUES
(1, 1, 'ST-MATH', '3.0'),
(1, 2, 'ST-INTSY', '2.5'),
(6, 1, 'ST-MATH', '0.0'),
(6, 4, 'CS-ARCH1', 'lol0');

