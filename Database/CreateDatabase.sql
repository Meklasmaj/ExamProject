-- Creates GymnasieProjektDB with tables and fake test data.
-- All names and absence are made up (no real personal data, GDPR).
-- Can be run again at any time: all tables are dropped and recreated.

IF DB_ID('GymnasieProjektDB') IS NULL
    CREATE DATABASE GymnasieProjektDB;
GO

USE GymnasieProjektDB;
GO

-- Drop in reverse order of the foreign keys
DROP TABLE IF EXISTS dbo.Attendance;
DROP TABLE IF EXISTS dbo.AbsenceType;
DROP TABLE IF EXISTS dbo.Module;
DROP TABLE IF EXISTS dbo.StudentTeam;
DROP TABLE IF EXISTS dbo.Team;
DROP TABLE IF EXISTS dbo.Student;
DROP TABLE IF EXISTS dbo.SchoolClass;
GO

---------------------------------------------------------------------------
-- Tables
---------------------------------------------------------------------------

CREATE TABLE dbo.SchoolClass (               -- Klasse
    SchoolClassId INT IDENTITY(1,1) PRIMARY KEY,
    Name          NVARCHAR(10) NOT NULL,     -- klassenavn
    Year          INT NOT NULL               -- årgang
);

CREATE TABLE dbo.Student (                   -- Elev
    StudentId     INT IDENTITY(1,1) PRIMARY KEY,
    FirstName     NVARCHAR(50) NOT NULL,
    LastName      NVARCHAR(50) NOT NULL,
    SchoolClassId INT NOT NULL REFERENCES dbo.SchoolClass(SchoolClassId)
);

CREATE TABLE dbo.Team (                      -- Hold
    TeamId        INT IDENTITY(1,1) PRIMARY KEY,
    Name          NVARCHAR(50) NOT NULL,     -- holdnavn
    Year          INT NOT NULL               -- årstal
);

CREATE TABLE dbo.StudentTeam (               -- Elev "deltager i" Hold (many-to-many)
    StudentId     INT NOT NULL REFERENCES dbo.Student(StudentId),
    TeamId        INT NOT NULL REFERENCES dbo.Team(TeamId),
    PRIMARY KEY (StudentId, TeamId)
);

CREATE TABLE dbo.Module (                    -- Modul
    ModuleId      INT IDENTITY(1,1) PRIMARY KEY,
    TeamId        INT NOT NULL REFERENCES dbo.Team(TeamId),
    Date          DATE NOT NULL,
    StartTime     TIME(0) NOT NULL,
    EndTime       TIME(0) NOT NULL,
    ModuleNumber  INT NOT NULL
);

CREATE TABLE dbo.AbsenceType (               -- Fraværstype
    AbsenceTypeId        INT IDENTITY(1,1) PRIMARY KEY,
    Name                 NVARCHAR(50) NOT NULL,
    CountsTowardsAbsence BIT NOT NULL        -- medregnes [ja/nej]
);

CREATE TABLE dbo.Attendance (                -- Tilstedeværelse
    AttendanceId   INT IDENTITY(1,1) PRIMARY KEY,
    StudentId      INT NOT NULL REFERENCES dbo.Student(StudentId),
    ModuleId       INT NOT NULL REFERENCES dbo.Module(ModuleId),
    Status         NVARCHAR(20) NOT NULL CHECK (Status IN ('Present', 'Absent')),
    AbsencePercent INT NULL CHECK (AbsencePercent BETWEEN 1 AND 100),   -- fraværsandel
    AbsenceTypeId  INT NULL REFERENCES dbo.AbsenceType(AbsenceTypeId),
    UNIQUE (StudentId, ModuleId)
);
GO

---------------------------------------------------------------------------
-- Fixed test data
---------------------------------------------------------------------------

INSERT INTO dbo.SchoolClass (Name, Year) VALUES
(N'1.a', 2026), (N'2.b', 2025), (N'3.c', 2024);

INSERT INTO dbo.Student (FirstName, LastName, SchoolClassId) VALUES
(N'Anna',    N'Jensen',      1),
(N'Mads',    N'Hansen',      1),
(N'Sofie',   N'Nielsen',     1),
(N'Oliver',  N'Pedersen',    1),
(N'Emma',    N'Andersen',    2),
(N'Lucas',   N'Christensen', 2),
(N'Freja',   N'Larsen',      2),
(N'Noah',    N'Sørensen',    2),
(N'Ida',     N'Rasmussen',   3),
(N'William', N'Jørgensen',   3),
(N'Clara',   N'Petersen',    3),
(N'Victor',  N'Madsen',      3);

INSERT INTO dbo.AbsenceType (Name, CountsTowardsAbsence) VALUES
(N'Sygdom',                   1),
(N'Private forhold',          1),
(N'Skolerelateret aktivitet', 0);

-- One team per class per subject, e.g. "1.a Dansk"
INSERT INTO dbo.Team (Name, Year)
SELECT c.Name + N' ' + s.Subject, 2026
FROM dbo.SchoolClass c
CROSS JOIN (VALUES (1, N'Dansk'), (2, N'Matematik'), (3, N'Engelsk'), (4, N'Historie')) AS s(SubjectNo, Subject)
ORDER BY c.SchoolClassId, s.SubjectNo;

-- Every student is on all four teams of their class
INSERT INTO dbo.StudentTeam (StudentId, TeamId)
SELECT st.StudentId, t.TeamId
FROM dbo.Student st
JOIN dbo.SchoolClass c ON c.SchoolClassId = st.SchoolClassId
JOIN dbo.Team t ON t.Name LIKE c.Name + N' %';
GO

---------------------------------------------------------------------------
-- Generated modules: 4 modules every weekday from 17 Aug to 20 Nov 2026
---------------------------------------------------------------------------

WITH Days AS (
    SELECT TOP (96) DATEADD(DAY, ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) - 1, CAST('2026-08-17' AS DATE)) AS Date
    FROM sys.all_objects
),
SchoolDays AS (
    -- 1900-01-01 was a Monday, so 0-4 = Monday-Friday (independent of server language settings)
    SELECT Date, DATEDIFF(DAY, '19000101', Date) % 7 AS Weekday
    FROM Days
    WHERE DATEDIFF(DAY, '19000101', Date) % 7 < 5
),
Slots AS (
    SELECT * FROM (VALUES
        (1, CAST('08:15' AS TIME(0)), CAST('09:45' AS TIME(0))),
        (2, CAST('10:00' AS TIME(0)), CAST('11:30' AS TIME(0))),
        (3, CAST('12:00' AS TIME(0)), CAST('13:30' AS TIME(0))),
        (4, CAST('13:45' AS TIME(0)), CAST('15:15' AS TIME(0)))
    ) AS s(ModuleNumber, StartTime, EndTime)
),
ClassTeams AS (
    -- Number each class's four teams 0-3, so the timetable can rotate through them
    SELECT c.SchoolClassId, t.TeamId,
           ROW_NUMBER() OVER (PARTITION BY c.SchoolClassId ORDER BY t.TeamId) - 1 AS TeamNo
    FROM dbo.SchoolClass c
    JOIN dbo.Team t ON t.Name LIKE c.Name + N' %'
)
INSERT INTO dbo.Module (TeamId, Date, StartTime, EndTime, ModuleNumber)
SELECT ct.TeamId, d.Date, s.StartTime, s.EndTime, s.ModuleNumber
FROM SchoolDays d
CROSS JOIN Slots s
JOIN ClassTeams ct ON ct.TeamNo = (d.Weekday + s.ModuleNumber) % 4;
GO

---------------------------------------------------------------------------
-- Generated attendance: one row per student per module on their teams
---------------------------------------------------------------------------

-- How often each student is absent (per mille), so the statistics have something to show
DECLARE @Rate TABLE (StudentId INT PRIMARY KEY, AbsencePerMille INT);
INSERT INTO @Rate VALUES
(1, 30), (2, 120), (3, 10), (4, 250),
(5, 50), (6, 80),  (7, 20), (8, 180),
(9, 40), (10, 60), (11, 5), (12, 100);

WITH Registrations AS (
    SELECT st.StudentId, m.ModuleId, m.Date, m.ModuleNumber, r.AbsencePerMille,
           -- Two repeatable "random" numbers per student and module
           ABS(CHECKSUM(HASHBYTES('MD5', CONCAT(st.StudentId, '-', m.ModuleId)))) % 1000 AS Roll,
           ABS(CHECKSUM(HASHBYTES('MD5', CONCAT(m.ModuleId, '/', st.StudentId)))) % 100 AS Kind
    FROM dbo.StudentTeam st
    JOIN dbo.Module m ON m.TeamId = st.TeamId
    JOIN @Rate r ON r.StudentId = st.StudentId
),
Marked AS (
    SELECT *,
           CASE
               WHEN Roll < AbsencePerMille THEN 1
               -- Extra absence in the first module on Mondays
               WHEN ModuleNumber = 1 AND DATEDIFF(DAY, '19000101', Date) % 7 = 0
                    AND Roll < AbsencePerMille + 80 THEN 1
               ELSE 0
           END AS IsAbsent
    FROM Registrations
)
INSERT INTO dbo.Attendance (StudentId, ModuleId, Status, AbsencePercent, AbsenceTypeId)
SELECT StudentId, ModuleId,
       CASE WHEN IsAbsent = 1 THEN 'Absent' ELSE 'Present' END,
       CASE WHEN IsAbsent = 0 THEN NULL
            WHEN Kind % 4 = 0 THEN 50          -- a quarter of absences are half a module
            ELSE 100 END,
       CASE WHEN IsAbsent = 0 THEN NULL
            WHEN Kind < 60 THEN 1              -- Sygdom
            WHEN Kind < 80 THEN 2              -- Private forhold
            WHEN Kind < 90 THEN 3              -- Skolerelateret aktivitet (does not count)
            ELSE NULL END                      -- no type registered
FROM Marked;
GO

-- Quick check: absence per student
SELECT s.FirstName + N' ' + s.LastName AS Student,
       COUNT(*) AS Modules,
       SUM(CASE WHEN a.Status = 'Absent' AND ISNULL(t.CountsTowardsAbsence, 1) = 1
                THEN ISNULL(a.AbsencePercent, 100) ELSE 0 END) / 100.0 AS AbsentModules
FROM dbo.Attendance a
JOIN dbo.Student s ON s.StudentId = a.StudentId
LEFT JOIN dbo.AbsenceType t ON t.AbsenceTypeId = a.AbsenceTypeId
GROUP BY s.StudentId, s.FirstName, s.LastName
ORDER BY s.StudentId;
GO
