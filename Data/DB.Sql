-- ===============================================================================
-- CBSE SCHOOL MANAGEMENT SYSTEM - COMPLETE DATABASE SCHEMA & SEED DATA SCRIPT
-- Server: (localdb)\MSSQLLocalDB
-- Database: CBSESchoolDb
-- ===============================================================================

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = N'CBSESchoolDb')
BEGIN
    CREATE DATABASE [CBSESchoolDb];
END
GO

USE [CBSESchoolDb];
GO

-- 1. ASP.NET CORE IDENTITY TABLES
IF OBJECT_ID(N'dbo.AspNetRoles', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[AspNetRoles] (
        [Id] NVARCHAR(450) NOT NULL,
        [Name] NVARCHAR(256) NULL,
        [NormalizedName] NVARCHAR(256) NULL,
        [ConcurrencyStamp] NVARCHAR(MAX) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO

IF OBJECT_ID(N'dbo.AspNetUsers', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[AspNetUsers] (
        [Id] NVARCHAR(450) NOT NULL,
        [FullName] NVARCHAR(100) NOT NULL DEFAULT N'',
        [RoleName] NVARCHAR(50) NULL DEFAULT N'Student',
        [CreatedAt] DATETIME2 (7) NOT NULL DEFAULT (GETUTCDATE()),
        [IsActive] BIT NOT NULL DEFAULT 1,
        [UserName] NVARCHAR(256) NULL,
        [NormalizedUserName] NVARCHAR(256) NULL,
        [Email] NVARCHAR(256) NULL,
        [NormalizedEmail] NVARCHAR(256) NULL,
        [EmailConfirmed] BIT NOT NULL DEFAULT 0,
        [PasswordHash] NVARCHAR(MAX) NULL,
        [SecurityStamp] NVARCHAR(MAX) NULL,
        [ConcurrencyStamp] NVARCHAR(MAX) NULL,
        [PhoneNumber] NVARCHAR(MAX) NULL,
        [PhoneNumberConfirmed] BIT NOT NULL DEFAULT 0,
        [TwoFactorEnabled] BIT NOT NULL DEFAULT 0,
        [LockoutEnd] DATETIMEOFFSET (7) NULL,
        [LockoutEnabled] BIT NOT NULL DEFAULT 1,
        [AccessFailedCount] INT NOT NULL DEFAULT 0,
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO

IF OBJECT_ID(N'dbo.AspNetUserRoles', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[AspNetUserRoles] (
        [UserId] NVARCHAR(450) NOT NULL,
        [RoleId] NVARCHAR(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY CLUSTERED ([UserId] ASC, [RoleId] ASC),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END
GO

-- 2. SCHOOL DOMAIN TABLES

-- Classes Table
IF OBJECT_ID(N'dbo.Classes', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Classes] (
        [Id] INT IDENTITY (1, 1) NOT NULL,
        [ClassName] NVARCHAR(50) NOT NULL,
        [Section] NVARCHAR(10) NOT NULL DEFAULT N'A',
        [Stream] NVARCHAR(50) NULL DEFAULT N'General',
        [Capacity] INT NOT NULL DEFAULT 40,
        CONSTRAINT [PK_Classes] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO

-- Subjects Table
IF OBJECT_ID(N'dbo.Subjects', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Subjects] (
        [Id] INT IDENTITY (1, 1) NOT NULL,
        [SubjectCode] NVARCHAR(20) NOT NULL,
        [SubjectName] NVARCHAR(100) NOT NULL,
        [ClassId] INT NOT NULL,
        [PassMarks] INT NOT NULL DEFAULT 33,
        [MaxMarks] INT NOT NULL DEFAULT 100,
        CONSTRAINT [PK_Subjects] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Subjects_Classes_ClassId] FOREIGN KEY ([ClassId]) REFERENCES [dbo].[Classes] ([Id]) ON DELETE CASCADE
    );
END
GO

-- Students Table
IF OBJECT_ID(N'dbo.Students', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Students] (
        [Id] INT IDENTITY (1, 1) NOT NULL,
        [RollNumber] NVARCHAR(30) NOT NULL,
        [FullName] NVARCHAR(100) NOT NULL,
        [DateOfBirth] DATETIME2 (7) NOT NULL,
        [Gender] NVARCHAR(10) NULL DEFAULT N'Male',
        [ClassId] INT NOT NULL,
        [ParentName] NVARCHAR(100) NULL,
        [ParentContact] NVARCHAR(20) NULL,
        [ParentEmail] NVARCHAR(100) NULL,
        [Address] NVARCHAR(250) NULL,
        [EnrollmentDate] DATETIME2 (7) NOT NULL DEFAULT (GETUTCDATE()),
        [UserId] NVARCHAR(450) NULL,
        CONSTRAINT [PK_Students] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Students_Classes_ClassId] FOREIGN KEY ([ClassId]) REFERENCES [dbo].[Classes] ([Id]) ON DELETE NO ACTION
    );
END
GO

-- Teachers Table
IF OBJECT_ID(N'dbo.Teachers', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Teachers] (
        [Id] INT IDENTITY (1, 1) NOT NULL,
        [EmployeeCode] NVARCHAR(30) NOT NULL,
        [FullName] NVARCHAR(100) NOT NULL,
        [Qualification] NVARCHAR(100) NULL,
        [SpecializationSubject] NVARCHAR(100) NULL,
        [Email] NVARCHAR(256) NOT NULL,
        [Phone] NVARCHAR(20) NULL,
        [JoiningDate] DATETIME2 (7) NOT NULL DEFAULT (GETUTCDATE()),
        [UserId] NVARCHAR(450) NULL,
        CONSTRAINT [PK_Teachers] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO

-- Exams Table
IF OBJECT_ID(N'dbo.Exams', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Exams] (
        [Id] INT IDENTITY (1, 1) NOT NULL,
        [ExamName] NVARCHAR(100) NOT NULL,
        [AcademicYear] NVARCHAR(20) NOT NULL DEFAULT N'2025-2026',
        [ClassId] INT NOT NULL,
        [ExamDate] DATETIME2 (7) NOT NULL DEFAULT (GETUTCDATE()),
        CONSTRAINT [PK_Exams] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Exams_Classes_ClassId] FOREIGN KEY ([ClassId]) REFERENCES [dbo].[Classes] ([Id]) ON DELETE CASCADE
    );
END
GO

-- Marks Table (With NO ACTION on delete to resolve cascade path conflicts)
IF OBJECT_ID(N'dbo.Marks', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Marks] (
        [Id] INT IDENTITY (1, 1) NOT NULL,
        [ExamId] INT NOT NULL,
        [StudentId] INT NOT NULL,
        [SubjectId] INT NOT NULL,
        [MarksObtained] FLOAT (53) NOT NULL,
        [MaxMarks] FLOAT (53) NOT NULL DEFAULT 100,
        [Grade] NVARCHAR(5) NULL DEFAULT N'A1',
        [Remarks] NVARCHAR(100) NULL DEFAULT N'Outstanding',
        CONSTRAINT [PK_Marks] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Marks_Exams_ExamId] FOREIGN KEY ([ExamId]) REFERENCES [dbo].[Exams] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Marks_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [dbo].[Students] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Marks_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [dbo].[Subjects] ([Id]) ON DELETE NO ACTION
    );
END
GO

-- Notices Table
IF OBJECT_ID(N'dbo.Notices', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Notices] (
        [Id] INT IDENTITY (1, 1) NOT NULL,
        [Title] NVARCHAR(150) NOT NULL,
        [Content] NVARCHAR(MAX) NOT NULL,
        [TargetRole] NVARCHAR(50) NULL DEFAULT N'All',
        [PostDate] DATETIME2 (7) NOT NULL DEFAULT (GETUTCDATE()),
        [ExpiryDate] DATETIME2 (7) NULL,
        [IsImportant] BIT NOT NULL DEFAULT 0,
        [PostedBy] NVARCHAR(100) NULL DEFAULT N'Principal Office',
        CONSTRAINT [PK_Notices] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO

-- FeeStructures Table
IF OBJECT_ID(N'dbo.FeeStructures', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[FeeStructures] (
        [Id] INT IDENTITY (1, 1) NOT NULL,
        [ClassId] INT NOT NULL,
        [FeeType] NVARCHAR(50) NOT NULL,
        [Amount] DECIMAL (18, 2) NOT NULL,
        [AcademicYear] NVARCHAR(20) NULL DEFAULT N'2025-2026',
        CONSTRAINT [PK_FeeStructures] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_FeeStructures_Classes_ClassId] FOREIGN KEY ([ClassId]) REFERENCES [dbo].[Classes] ([Id]) ON DELETE CASCADE
    );
END
GO

-- StudentFees Table
IF OBJECT_ID(N'dbo.StudentFees', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[StudentFees] (
        [Id] INT IDENTITY (1, 1) NOT NULL,
        [StudentId] INT NOT NULL,
        [FeeStructureId] INT NOT NULL,
        [AmountPaid] DECIMAL (18, 2) NOT NULL,
        [PaymentDate] DATETIME2 (7) NOT NULL DEFAULT (GETUTCDATE()),
        [PaymentMode] NVARCHAR(50) NULL DEFAULT N'Online UPI',
        [TransactionId] NVARCHAR(50) NULL,
        [Status] NVARCHAR(20) NULL DEFAULT N'Paid',
        CONSTRAINT [PK_StudentFees] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_StudentFees_FeeStructures_FeeStructureId] FOREIGN KEY ([FeeStructureId]) REFERENCES [dbo].[FeeStructures] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StudentFees_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [dbo].[Students] ([Id]) ON DELETE NO ACTION
    );
END
GO

-- ===============================================================================
-- 3. SEED INITIAL DATA (ROLES, ADMIN USER, CLASSES, SUBJECTS, EXAMS, MARKS, FEES)
-- ===============================================================================

-- Seed Roles
IF NOT EXISTS (SELECT 1 FROM dbo.AspNetRoles WHERE Name = 'Admin')
    INSERT INTO dbo.AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp) VALUES ('1', 'Admin', 'ADMIN', NEWID());

IF NOT EXISTS (SELECT 1 FROM dbo.AspNetRoles WHERE Name = 'Teacher')
    INSERT INTO dbo.AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp) VALUES ('2', 'Teacher', 'TEACHER', NEWID());

IF NOT EXISTS (SELECT 1 FROM dbo.AspNetRoles WHERE Name = 'Student')
    INSERT INTO dbo.AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp) VALUES ('3', 'Student', 'STUDENT', NEWID());

IF NOT EXISTS (SELECT 1 FROM dbo.AspNetRoles WHERE Name = 'Parent')
    INSERT INTO dbo.AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp) VALUES ('4', 'Parent', 'PARENT', NEWID());

-- Seed Classes
IF NOT EXISTS (SELECT 1 FROM dbo.Classes)
BEGIN
    INSERT INTO dbo.Classes (ClassName, Section, Stream, Capacity) VALUES 
    (N'Class IX', N'A', N'General', 40),
    (N'Class X', N'A', N'General', 40),
    (N'Class XI', N'A', N'Science', 35),
    (N'Class XII', N'B', N'Commerce', 35);
END

-- Seed Subjects (Class X & XI)
IF NOT EXISTS (SELECT 1 FROM dbo.Subjects)
BEGIN
    DECLARE @Class10Id INT = (SELECT TOP 1 Id FROM dbo.Classes WHERE ClassName = N'Class X');
    DECLARE @Class11Id INT = (SELECT TOP 1 Id FROM dbo.Classes WHERE ClassName = N'Class XI');

    INSERT INTO dbo.Subjects (SubjectCode, SubjectName, ClassId, PassMarks, MaxMarks) VALUES
    (N'041', N'Mathematics Standard', @Class10Id, 33, 100),
    (N'086', N'Science', @Class10Id, 33, 100),
    (N'184', N'English Language & Literature', @Class10Id, 33, 100),
    (N'087', N'Social Science', @Class10Id, 33, 100),
    (N'042', N'Physics', @Class11Id, 33, 100),
    (N'043', N'Chemistry', @Class11Id, 33, 100);
END

-- Seed Teachers
IF NOT EXISTS (SELECT 1 FROM dbo.Teachers)
BEGIN
    INSERT INTO dbo.Teachers (EmployeeCode, FullName, Qualification, SpecializationSubject, Email, Phone, JoiningDate) VALUES
    (N'EMP-T-101', N'Dr. Rajesh Sharma', N'M.Sc, Ph.D, B.Ed', N'Mathematics', N'rajesh.sharma@cbse.school.edu', N'+91 9876543210', GETUTCDATE()),
    (N'EMP-T-102', N'Mrs. Sunita Verma', N'M.Sc Physics, B.Ed', N'Science / Physics', N'sunita.verma@cbse.school.edu', N'+91 9876543211', GETUTCDATE());
END

-- Seed Students
IF NOT EXISTS (SELECT 1 FROM dbo.Students)
BEGIN
    DECLARE @Class10IdForStudent INT = (SELECT TOP 1 Id FROM dbo.Classes WHERE ClassName = N'Class X');
    INSERT INTO dbo.Students (RollNumber, FullName, DateOfBirth, Gender, ClassId, ParentName, ParentContact, ParentEmail, Address, EnrollmentDate) VALUES
    (N'CBSE2026-X-01', N'Aarav Gupta', '2010-05-14', N'Male', @Class10IdForStudent, N'Ramesh Gupta', N'+91 9123456789', N'ramesh.gupta@example.com', N'Flat 402, Green Valley, Hyderabad', GETUTCDATE()),
    (N'CBSE2026-X-02', N'Ananya Reddy', '2010-08-22', N'Female', @Class10IdForStudent, N'Srinivas Reddy', N'+91 9123456790', N'srinivas.reddy@example.com', N'Plot 88, Jubilee Hills, Hyderabad', GETUTCDATE());
END

-- Seed Exams & Marks
IF NOT EXISTS (SELECT 1 FROM dbo.Exams)
BEGIN
    DECLARE @Class10IdForExam INT = (SELECT TOP 1 Id FROM dbo.Classes WHERE ClassName = N'Class X');
    INSERT INTO dbo.Exams (ExamName, AcademicYear, ClassId, ExamDate) VALUES
    (N'Mid-Term Examination 2025', N'2025-2026', @Class10IdForExam, GETUTCDATE());

    DECLARE @ExamId INT = SCOPE_IDENTITY();
    DECLARE @StudentId INT = (SELECT TOP 1 Id FROM dbo.Students WHERE RollNumber = N'CBSE2026-X-01');
    DECLARE @SubMathId INT = (SELECT TOP 1 Id FROM dbo.Subjects WHERE SubjectCode = N'041');
    DECLARE @SubSciId INT = (SELECT TOP 1 Id FROM dbo.Subjects WHERE SubjectCode = N'086');

    INSERT INTO dbo.Marks (ExamId, StudentId, SubjectId, MarksObtained, MaxMarks, Grade, Remarks) VALUES
    (@ExamId, @StudentId, @SubMathId, 92.5, 100, N'A1', N'Outstanding Performance'),
    (@ExamId, @StudentId, @SubSciId, 88.0, 100, N'A2', N'Excellent Performance');
END

-- Seed Notices
IF NOT EXISTS (SELECT 1 FROM dbo.Notices)
BEGIN
    INSERT INTO dbo.Notices (Title, Content, TargetRole, PostDate, IsImportant, PostedBy) VALUES
    (N'CBSE Class X & XII Board Examination Date Sheet Released', N'The Central Board of Secondary Education has officially released the exam schedule. All students are advised to check the date sheet.', N'All', GETUTCDATE(), 1, N'Principal Office'),
    (N'Annual Science & Innovation Fair 2026', N'Participating students must submit their project abstracts to their respective science teachers.', N'Students', GETUTCDATE(), 0, N'Science Department');
END

-- Seed Fee Structures & Payments
IF NOT EXISTS (SELECT 1 FROM dbo.FeeStructures)
BEGIN
    DECLARE @Class10IdForFee INT = (SELECT TOP 1 Id FROM dbo.Classes WHERE ClassName = N'Class X');
    INSERT INTO dbo.FeeStructures (ClassId, FeeType, Amount, AcademicYear) VALUES
    (@Class10IdForFee, N'Tuition Fee (Quarter 1)', 15000.00, N'2025-2026'),
    (@Class10IdForFee, N'Science & Computer Lab Fee', 3500.00, N'2025-2026');

    DECLARE @FeeStructId INT = SCOPE_IDENTITY();
    DECLARE @FeeStudentId INT = (SELECT TOP 1 Id FROM dbo.Students WHERE RollNumber = N'CBSE2026-X-01');
    INSERT INTO dbo.StudentFees (StudentId, FeeStructureId, AmountPaid, PaymentDate, PaymentMode, TransactionId, Status) VALUES
    (@FeeStudentId, @FeeStructId, 15000.00, GETUTCDATE(), N'Online UPI', N'TXN-CBSE-998811', N'Paid');
END
GO