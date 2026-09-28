-- ===============================================================================
-- CBSE SCHOOL MANAGEMENT SYSTEM - STORED PROCEDURES (SP) SCRIPT
-- Server: (localdb)\MSSQLLocalDB
-- Database: CBSESchoolDb
-- ===============================================================================

USE [CBSESchoolDb];
GO

-- 1. SP: Get All Students with Class Name
CREATE OR ALTER PROCEDURE dbo.sp_GetAllStudents
    @ClassId INT = NULL,
    @SearchString NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        s.Id,
        s.RollNumber,
        s.FullName,
        s.DateOfBirth,
        s.Gender,
        s.ClassId,
        c.ClassName + ' (' + c.Section + ')' AS ClassName,
        s.ParentName,
        s.ParentContact,
        s.ParentEmail,
        s.Address,
        s.EnrollmentDate,
        s.UserId
    FROM dbo.Students s
    LEFT JOIN dbo.Classes c ON s.ClassId = c.Id
    WHERE (@ClassId IS NULL OR s.ClassId = @ClassId)
      AND (@SearchString IS NULL OR s.FullName LIKE '%' + @SearchString + '%' OR s.RollNumber LIKE '%' + @SearchString + '%')
    ORDER BY s.Id DESC;
END;
GO

-- 2. SP: Add New Student
CREATE OR ALTER PROCEDURE dbo.sp_AddStudent
    @FullName NVARCHAR(100),
    @DateOfBirth DATETIME2,
    @Gender NVARCHAR(10),
    @ClassId INT,
    @ParentName NVARCHAR(100),
    @ParentContact NVARCHAR(20),
    @ParentEmail NVARCHAR(100),
    @Address NVARCHAR(250),
    @RollNumber NVARCHAR(30) = NULL,
    @NewStudentId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    -- Auto-generate Roll Number if null or empty
    IF @RollNumber IS NULL OR LTRIM(RTRIM(@RollNumber)) = ''
    BEGIN
        DECLARE @ClassName NVARCHAR(20);
        SELECT @ClassName = REPLACE(REPLACE(ClassName, 'Class ', ''), ' ', '') FROM dbo.Classes WHERE Id = @ClassId;
        IF @ClassName IS NULL SET @ClassName = 'GEN';

        DECLARE @Count INT;
        SELECT @Count = COUNT(1) + 1 FROM dbo.Students WHERE ClassId = @ClassId;
        SET @RollNumber = 'CBSE' + CAST(YEAR(GETUTCDATE()) AS NVARCHAR(4)) + '-' + @ClassName + '-' + RIGHT('00' + CAST(@Count AS NVARCHAR(10)), 2);
    END

    INSERT INTO dbo.Students (RollNumber, FullName, DateOfBirth, Gender, ClassId, ParentName, ParentContact, ParentEmail, Address, EnrollmentDate)
    VALUES (@RollNumber, @FullName, @DateOfBirth, @Gender, @ClassId, @ParentName, @ParentContact, @ParentEmail, @Address, GETUTCDATE());

    SET @NewStudentId = SCOPE_IDENTITY();
END;
GO

-- 3. SP: Save / Update Mark & Auto Calculate CBSE Grade
CREATE OR ALTER PROCEDURE dbo.sp_SaveStudentMark
    @ExamId INT,
    @StudentId INT,
    @SubjectId INT,
    @MarksObtained FLOAT,
    @MaxMarks FLOAT = 100
AS
BEGIN
    SET NOCOUNT ON;

    -- Calculate Percentage & Grade
    DECLARE @Percentage FLOAT = (@MarksObtained / @MaxMarks) * 100.0;
    DECLARE @Grade NVARCHAR(5);
    DECLARE @Remarks NVARCHAR(100);

    IF @Percentage >= 91 SET @Grade = 'A1';
    ELSE IF @Percentage >= 81 SET @Grade = 'A2';
    ELSE IF @Percentage >= 71 SET @Grade = 'B1';
    ELSE IF @Percentage >= 61 SET @Grade = 'B2';
    ELSE IF @Percentage >= 51 SET @Grade = 'C1';
    ELSE IF @Percentage >= 41 SET @Grade = 'C2';
    ELSE IF @Percentage >= 33 SET @Grade = 'D';
    ELSE SET @Grade = 'E';

    SELECT @Remarks = CASE @Grade
        WHEN 'A1' THEN 'Outstanding Performance'
        WHEN 'A2' THEN 'Excellent Performance'
        WHEN 'B1' THEN 'Very Good Progress'
        WHEN 'B2' THEN 'Good Progress'
        WHEN 'C1' THEN 'Above Average Effort'
        WHEN 'C2' THEN 'Average Scope for Improvement'
        WHEN 'D' THEN 'Marginal Pass - Needs Attention'
        ELSE 'Essential Repeat - Needs Coaching'
    END;

    IF EXISTS (SELECT 1 FROM dbo.Marks WHERE ExamId = @ExamId AND StudentId = @StudentId AND SubjectId = @SubjectId)
    BEGIN
        UPDATE dbo.Marks
        SET MarksObtained = @MarksObtained,
            MaxMarks = @MaxMarks,
            Grade = @Grade,
            Remarks = @Remarks
        WHERE ExamId = @ExamId AND StudentId = @StudentId AND SubjectId = @SubjectId;
    END
    ELSE
    BEGIN
        INSERT INTO dbo.Marks (ExamId, StudentId, SubjectId, MarksObtained, MaxMarks, Grade, Remarks)
        VALUES (@ExamId, @StudentId, @SubjectId, @MarksObtained, @MaxMarks, @Grade, @Remarks);
    END
END;
GO

-- 4. SP: Get Complete Student Report Card Data
CREATE OR ALTER PROCEDURE dbo.sp_GetStudentReportCard
    @StudentId INT,
    @ExamId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        sub.SubjectCode,
        sub.SubjectName,
        ISNULL(m.MarksObtained, 0) AS MarksObtained,
        ISNULL(m.MaxMarks, sub.MaxMarks) AS MaxMarks,
        ISNULL(m.Grade, 'E') AS Grade,
        ISNULL(m.Remarks, 'N/A') AS Remarks,
        CASE WHEN ISNULL(m.MarksObtained, 0) >= sub.PassMarks THEN 1 ELSE 0 END AS IsPassing
    FROM dbo.Subjects sub
    INNER JOIN dbo.Students st ON st.ClassId = sub.ClassId
    LEFT JOIN dbo.Marks m ON m.SubjectId = sub.Id AND m.StudentId = st.Id AND m.ExamId = @ExamId
    WHERE st.Id = @StudentId;
END;
GO

-- 5. SP: Get Dashboard Counts
CREATE OR ALTER PROCEDURE dbo.sp_GetDashboardStats
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        (SELECT COUNT(1) FROM dbo.Students) AS TotalStudents,
        (SELECT COUNT(1) FROM dbo.Teachers) AS TotalTeachers,
        (SELECT COUNT(1) FROM dbo.Classes) AS TotalClasses,
        (SELECT COUNT(1) FROM dbo.Notices) AS ActiveNoticesCount,
        (SELECT ISNULL(SUM(AmountPaid), 0) FROM dbo.StudentFees WHERE Status = 'Paid') AS TotalFeeCollection;
END;
GO
