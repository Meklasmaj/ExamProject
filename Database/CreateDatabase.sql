-- Creates GymnasieProjektDB with the Student table and some fake test data.
-- All names are made up (no real personal data, GDPR).
-- Run in SSMS against server DESKTOP-APV603A.

IF DB_ID('GymnasieProjektDB') IS NULL
    CREATE DATABASE GymnasieProjektDB;
GO

USE GymnasieProjektDB;
GO

IF OBJECT_ID('dbo.Student', 'U') IS NOT NULL
    DROP TABLE dbo.Student;
GO

CREATE TABLE dbo.Student (
    StudentId   INT IDENTITY(1,1) PRIMARY KEY,
    FirstName   NVARCHAR(50) NOT NULL,
    LastName    NVARCHAR(50) NOT NULL,
    ClassName   NVARCHAR(10) NOT NULL
);
GO

INSERT INTO dbo.Student (FirstName, LastName, ClassName) VALUES
(N'Anna',    N'Jensen',      N'1.a'),
(N'Mads',    N'Hansen',      N'1.a'),
(N'Sofie',   N'Nielsen',     N'1.a'),
(N'Oliver',  N'Pedersen',    N'1.a'),
(N'Emma',    N'Andersen',    N'2.b'),
(N'Lucas',   N'Christensen', N'2.b'),
(N'Freja',   N'Larsen',      N'2.b'),
(N'Noah',    N'Sørensen',    N'2.b'),
(N'Ida',     N'Rasmussen',   N'3.c'),
(N'William', N'Jørgensen',   N'3.c'),
(N'Clara',   N'Petersen',    N'3.c'),
(N'Victor',  N'Madsen',      N'3.c');
GO
