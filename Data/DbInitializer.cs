using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CBSESchool.Models;
using CBSESchool.Services;

namespace CBSESchool.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var gradeCalculator = scope.ServiceProvider.GetRequiredService<IGradeCalculatorService>();

            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // Create / Update Stored Procedures
            await context.Database.ExecuteSqlRawAsync(@"
                CREATE OR ALTER PROCEDURE dbo.sp_GetAllStudents
                    @ClassId INT = NULL,
                    @SearchString NVARCHAR(100) = NULL
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SELECT s.Id, s.RollNumber, s.FullName, s.DateOfBirth, s.Gender, s.ClassId,
                           s.ParentName, s.ParentContact, s.ParentEmail, s.Address, s.EnrollmentDate, s.UserId
                    FROM dbo.Students s
                    WHERE (@ClassId IS NULL OR s.ClassId = @ClassId)
                      AND (@SearchString IS NULL OR s.FullName LIKE '%' + @SearchString + '%' OR s.RollNumber LIKE '%' + @SearchString + '%')
                    ORDER BY s.Id DESC;
                END;
            ");

            await context.Database.ExecuteSqlRawAsync(@"
                CREATE OR ALTER PROCEDURE dbo.sp_SaveStudentMark
                    @ExamId INT,
                    @StudentId INT,
                    @SubjectId INT,
                    @MarksObtained FLOAT,
                    @MaxMarks FLOAT = 100
                AS
                BEGIN
                    SET NOCOUNT ON;
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
                        SET MarksObtained = @MarksObtained, MaxMarks = @MaxMarks, Grade = @Grade, Remarks = @Remarks
                        WHERE ExamId = @ExamId AND StudentId = @StudentId AND SubjectId = @SubjectId;
                    END
                    ELSE
                    BEGIN
                        INSERT INTO dbo.Marks (ExamId, StudentId, SubjectId, MarksObtained, MaxMarks, Grade, Remarks)
                        VALUES (@ExamId, @StudentId, @SubjectId, @MarksObtained, @MaxMarks, @Grade, @Remarks);
                    END
                END;
            ");

            // 1. Seed Roles
            string[] roles = new string[] { "Admin", "Teacher", "Student", "Parent" };
            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // 2. Seed Admin User
            var adminEmail = "admin@cbse.school.edu";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FullName = "CBSE Principal / Admin",
                    RoleName = "Admin"
                };
                var result = await userManager.CreateAsync(adminUser, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // 3. Seed Classes & Sections if empty
            if (!context.Classes.Any())
            {
                var class9 = new SchoolClass { ClassName = "Class IX", Section = "A", Stream = "General", Capacity = 40 };
                var class10 = new SchoolClass { ClassName = "Class X", Section = "A", Stream = "General", Capacity = 40 };
                var class11Sci = new SchoolClass { ClassName = "Class XI", Section = "A", Stream = "Science", Capacity = 35 };
                var class12Com = new SchoolClass { ClassName = "Class XII", Section = "B", Stream = "Commerce", Capacity = 35 };

                context.Classes.AddRange(class9, class10, class11Sci, class12Com);
                await context.SaveChangesAsync();

                // 4. Seed CBSE Subjects
                var subMath10 = new Subject { SubjectCode = "041", SubjectName = "Mathematics Standard", ClassId = class10.Id, PassMarks = 33, MaxMarks = 100 };
                var subSci10 = new Subject { SubjectCode = "086", SubjectName = "Science", ClassId = class10.Id, PassMarks = 33, MaxMarks = 100 };
                var subEng10 = new Subject { SubjectCode = "184", SubjectName = "English Language & Literature", ClassId = class10.Id, PassMarks = 33, MaxMarks = 100 };
                var subSst10 = new Subject { SubjectCode = "087", SubjectName = "Social Science", ClassId = class10.Id, PassMarks = 33, MaxMarks = 100 };
                var subPhy11 = new Subject { SubjectCode = "042", SubjectName = "Physics", ClassId = class11Sci.Id, PassMarks = 33, MaxMarks = 100 };
                var subChem11 = new Subject { SubjectCode = "043", SubjectName = "Chemistry", ClassId = class11Sci.Id, PassMarks = 33, MaxMarks = 100 };

                context.Subjects.AddRange(subMath10, subSci10, subEng10, subSst10, subPhy11, subChem11);
                await context.SaveChangesAsync();

                // 5. Seed Teachers
                var teacher1 = new Teacher
                {
                    EmployeeCode = "EMP-T-101",
                    FullName = "Dr. Rajesh Sharma",
                    Qualification = "M.Sc, Ph.D, B.Ed",
                    SpecializationSubject = "Mathematics",
                    Email = "rajesh.sharma@cbse.school.edu",
                    Phone = "+91 9876543210",
                    JoiningDate = DateTime.UtcNow.AddYears(-5)
                };
                var teacher2 = new Teacher
                {
                    EmployeeCode = "EMP-T-102",
                    FullName = "Mrs. Sunita Verma",
                    Qualification = "M.Sc Physics, B.Ed",
                    SpecializationSubject = "Science / Physics",
                    Email = "sunita.verma@cbse.school.edu",
                    Phone = "+91 9876543211",
                    JoiningDate = DateTime.UtcNow.AddYears(-3)
                };

                context.Teachers.AddRange(teacher1, teacher2);
                await context.SaveChangesAsync();

                // Create Identity accounts for teachers
                var teacherUser1 = new ApplicationUser { UserName = teacher1.Email, Email = teacher1.Email, EmailConfirmed = true, FullName = teacher1.FullName, RoleName = "Teacher" };
                if ((await userManager.CreateAsync(teacherUser1, "Teacher@123")).Succeeded)
                {
                    await userManager.AddToRoleAsync(teacherUser1, "Teacher");
                    teacher1.UserId = teacherUser1.Id;
                }

                // 6. Seed Students
                var s1 = new Student
                {
                    RollNumber = "CBSE2026-X-01",
                    FullName = "Aarav Gupta",
                    DateOfBirth = new DateTime(2010, 5, 14),
                    Gender = "Male",
                    ClassId = class10.Id,
                    ParentName = "Ramesh Gupta",
                    ParentContact = "+91 9123456789",
                    ParentEmail = "ramesh.gupta@example.com",
                    Address = "Flat 402, Green Valley Apartments, Hyderabad",
                    EnrollmentDate = DateTime.UtcNow.AddYears(-2)
                };
                var s2 = new Student
                {
                    RollNumber = "CBSE2026-X-02",
                    FullName = "Ananya Reddy",
                    DateOfBirth = new DateTime(2010, 8, 22),
                    Gender = "Female",
                    ClassId = class10.Id,
                    ParentName = "Srinivas Reddy",
                    ParentContact = "+91 9123456790",
                    ParentEmail = "srinivas.reddy@example.com",
                    Address = "Plot 88, Jubilee Hills, Hyderabad",
                    EnrollmentDate = DateTime.UtcNow.AddYears(-2)
                };

                context.Students.AddRange(s1, s2);
                await context.SaveChangesAsync();

                // Create Student user account
                var studentUser1 = new ApplicationUser { UserName = "aarav.gupta@cbse.school.edu", Email = "aarav.gupta@cbse.school.edu", EmailConfirmed = true, FullName = s1.FullName, RoleName = "Student" };
                if ((await userManager.CreateAsync(studentUser1, "Student@123")).Succeeded)
                {
                    await userManager.AddToRoleAsync(studentUser1, "Student");
                    s1.UserId = studentUser1.Id;
                }

                // 7. Seed Exam & Marks
                var examMid = new Exam
                {
                    ExamName = "Mid-Term Examination 2025",
                    AcademicYear = "2025-2026",
                    ClassId = class10.Id,
                    ExamDate = DateTime.UtcNow.AddDays(-15)
                };
                context.Exams.Add(examMid);
                await context.SaveChangesAsync();

                var mark1 = new Mark
                {
                    ExamId = examMid.Id,
                    StudentId = s1.Id,
                    SubjectId = subMath10.Id,
                    MarksObtained = 92.5,
                    MaxMarks = 100,
                    Grade = gradeCalculator.CalculateGrade(92.5, 100),
                    Remarks = gradeCalculator.GetRemarks("A1")
                };
                var mark2 = new Mark
                {
                    ExamId = examMid.Id,
                    StudentId = s1.Id,
                    SubjectId = subSci10.Id,
                    MarksObtained = 88.0,
                    MaxMarks = 100,
                    Grade = gradeCalculator.CalculateGrade(88.0, 100),
                    Remarks = gradeCalculator.GetRemarks("A2")
                };

                context.Marks.AddRange(mark1, mark2);

                // 8. Seed Notices
                var notice1 = new Notice
                {
                    Title = "CBSE Class X & XII Board Examination Date Sheet Released",
                    Content = "The Central Board of Secondary Education has officially released the exam schedule. All students are advised to download the date sheet from the student portal.",
                    TargetRole = "All",
                    PostDate = DateTime.UtcNow.AddDays(-2),
                    IsImportant = true,
                    PostedBy = "Principal Office"
                };
                var notice2 = new Notice
                {
                    Title = "Annual Science & Innovation Fair 2026",
                    Content = "Participating students must submit their project abstracts to their respective science teachers by next Friday.",
                    TargetRole = "Students",
                    PostDate = DateTime.UtcNow.AddDays(-5),
                    IsImportant = false,
                    PostedBy = "Science Department"
                };

                context.Notices.AddRange(notice1, notice2);

                // 9. Seed Fee Structure & Student Payment
                var fee1 = new FeeStructure { ClassId = class10.Id, FeeType = "Tuition Fee (Quarter 1)", Amount = 15000.00m, AcademicYear = "2025-2026" };
                var fee2 = new FeeStructure { ClassId = class10.Id, FeeType = "Science & Computer Lab Fee", Amount = 3500.00m, AcademicYear = "2025-2026" };
                context.FeeStructures.AddRange(fee1, fee2);
                await context.SaveChangesAsync();

                var studentFee1 = new StudentFee
                {
                    StudentId = s1.Id,
                    FeeStructureId = fee1.Id,
                    AmountPaid = 15000.00m,
                    PaymentDate = DateTime.UtcNow.AddDays(-10),
                    PaymentMode = "Online UPI",
                    TransactionId = "TXN-CBSE-998811",
                    Status = "Paid"
                };
                context.StudentFees.Add(studentFee1);

                await context.SaveChangesAsync();
            }
        }
    }
}
