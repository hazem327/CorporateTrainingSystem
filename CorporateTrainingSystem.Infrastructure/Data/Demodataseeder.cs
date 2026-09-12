using CorporateTrainingSystem.Domain.Entities;
using CorporateTrainingSystem.Infrastructure.Data;
using CorporateTrainingSystem.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CorporateTrainingSystem.Infrastructure.Data
{
    public static class DemoDataSeeder
    {
        public static async Task SeedDemoDataAsync(IServiceProvider services)
        {
            var context = services.GetRequiredService<AppDbContext>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            // Only seed once - if departments already exist, skip everything
            if (await context.Departments.AnyAsync())
            {
                return;
            }

            // ---------- Departments ----------
            var engineering = new Department { Name = "Engineering", Description = "Software engineering team" };
            var hr = new Department { Name = "Human Resources", Description = "HR and people operations" };
            var sales = new Department { Name = "Sales", Description = "Sales and business development" };

            context.Departments.AddRange(engineering, hr, sales);
            await context.SaveChangesAsync();

            // ---------- Employees ----------
            var employees = new List<Employee>
            {
                new() { EmployeeNumber = "EMP-1001", FullName = "Sarah Ahmed", Email = "sarah.ahmed@corp.local", JobTitle = "Senior Developer", HireDate = DateTime.Today.AddYears(-3), DepartmentId = engineering.Id, IsActive = true },
                new() { EmployeeNumber = "EMP-1002", FullName = "Omar Khalid", Email = "omar.khalid@corp.local", JobTitle = "DevOps Engineer", HireDate = DateTime.Today.AddYears(-2), DepartmentId = engineering.Id, IsActive = true },
                new() { EmployeeNumber = "EMP-1003", FullName = "Lina Farouk", Email = "lina.farouk@corp.local", JobTitle = "HR Specialist", HireDate = DateTime.Today.AddYears(-4), DepartmentId = hr.Id, IsActive = true },
                new() { EmployeeNumber = "EMP-1004", FullName = "Youssef Mansour", Email = "youssef.mansour@corp.local", JobTitle = "Sales Executive", HireDate = DateTime.Today.AddYears(-1), DepartmentId = sales.Id, IsActive = true },
                new() { EmployeeNumber = "EMP-1005", FullName = "Nour Hassan", Email = "nour.hassan@corp.local", JobTitle = "Training Coordinator", HireDate = DateTime.Today.AddYears(-2), DepartmentId = hr.Id, IsActive = true },
                new() { EmployeeNumber = "EMP-1006", FullName = "Karim Adel", Email = "karim.adel@corp.local", JobTitle = "Lead Instructor", HireDate = DateTime.Today.AddYears(-5), DepartmentId = engineering.Id, IsActive = true },
                new() { EmployeeNumber = "EMP-1007", FullName = "Mona Fathy", Email = "mona.fathy@corp.local", JobTitle = "QA Engineer", HireDate = DateTime.Today.AddYears(-2), DepartmentId = engineering.Id, IsActive = true },
                new() { EmployeeNumber = "EMP-1008", FullName = "Tarek Samir", Email = "tarek.samir@corp.local", JobTitle = "Backend Developer", HireDate = DateTime.Today.AddMonths(-8), DepartmentId = engineering.Id, IsActive = true },
                new() { EmployeeNumber = "EMP-1009", FullName = "Hana Ibrahim", Email = "hana.ibrahim@corp.local", JobTitle = "Recruiter", HireDate = DateTime.Today.AddYears(-1), DepartmentId = hr.Id, IsActive = true },
                new() { EmployeeNumber = "EMP-1010", FullName = "Ziad Nabil", Email = "ziad.nabil@corp.local", JobTitle = "Sales Manager", HireDate = DateTime.Today.AddYears(-3), DepartmentId = sales.Id, IsActive = true },
                new() { EmployeeNumber = "EMP-1011", FullName = "Dina Saeed", Email = "dina.saeed@corp.local", JobTitle = "Account Executive", HireDate = DateTime.Today.AddMonths(-5), DepartmentId = sales.Id, IsActive = true },
                new() { EmployeeNumber = "EMP-1012", FullName = "Amr Ashraf", Email = "amr.ashraf@corp.local", JobTitle = "Frontend Developer", HireDate = DateTime.Today.AddYears(-1), DepartmentId = engineering.Id, IsActive = true },
                new() { EmployeeNumber = "EMP-1013", FullName = "Rana Gamal", Email = "rana.gamal@corp.local", JobTitle = "HR Business Partner", HireDate = DateTime.Today.AddYears(-4), DepartmentId = hr.Id, IsActive = true },
                new() { EmployeeNumber = "EMP-1014", FullName = "Khaled Fouad", Email = "khaled.fouad@corp.local", JobTitle = "Sales Development Rep", HireDate = DateTime.Today.AddMonths(-3), DepartmentId = sales.Id, IsActive = true },
                new() { EmployeeNumber = "EMP-1015", FullName = "Salma Tarek", Email = "salma.tarek@corp.local", JobTitle = "Software Engineer", HireDate = DateTime.Today.AddYears(-2), DepartmentId = engineering.Id, IsActive = true },
                new() { EmployeeNumber = "EMP-1016", FullName = "Mostafa Reda", Email = "mostafa.reda@corp.local", JobTitle = "Payroll Specialist", HireDate = DateTime.Today.AddYears(-5), DepartmentId = hr.Id, IsActive = true },
                new() { EmployeeNumber = "EMP-1017", FullName = "Yasmin Adel", Email = "yasmin.adel@corp.local", JobTitle = "Account Manager", HireDate = DateTime.Today.AddMonths(-10), DepartmentId = sales.Id, IsActive = true },
                new() { EmployeeNumber = "EMP-1018", FullName = "Hossam Wael", Email = "hossam.wael@corp.local", JobTitle = "DevOps Engineer", HireDate = DateTime.Today.AddYears(-1), DepartmentId = engineering.Id, IsActive = false },
                new() { EmployeeNumber = "EMP-1019", FullName = "Nada Hisham", Email = "nada.hisham@corp.local", JobTitle = "Talent Acquisition", HireDate = DateTime.Today.AddYears(-3), DepartmentId = hr.Id, IsActive = false },
                new() { EmployeeNumber = "EMP-1020", FullName = "Ahmed Sabry", Email = "ahmed.sabry@corp.local", JobTitle = "Sales Associate", HireDate = DateTime.Today.AddMonths(-6), DepartmentId = sales.Id, IsActive = true },
            };

            context.Employees.AddRange(employees);
            await context.SaveChangesAsync();

            var instructor = employees.First(e => e.EmployeeNumber == "EMP-1006");

            // ---------- Courses ----------
            var courses = new List<Course>
            {
                new() { Title = "Workplace Safety Fundamentals", Description = "Core safety practices for all staff.", Category = "Safety", DurationHours = 4, PassingScore = 70, CertificateValidityMonths = 12, IsActive = true },
                new() { Title = "Data Privacy & GDPR Basics", Description = "Handling personal data responsibly.", Category = "Compliance", DurationHours = 6, PassingScore = 75, CertificateValidityMonths = 24, IsActive = true },
                new() { Title = "Leadership Essentials", Description = "Foundational leadership and management skills.", Category = "Management", DurationHours = 8, PassingScore = 65, CertificateValidityMonths = 36, IsActive = true },
                new() { Title = "First Aid & CPR", Description = "Emergency response and basic first aid.", Category = "Safety", DurationHours = 5, PassingScore = 80, CertificateValidityMonths = 12, IsActive = true },
            };

            context.Courses.AddRange(courses);
            await context.SaveChangesAsync();

            // ---------- Training Sessions ----------
            var safetyCourse = courses.First(c => c.Title == "Workplace Safety Fundamentals");
            var privacyCourse = courses.First(c => c.Title == "Data Privacy & GDPR Basics");
            var leadershipCourse = courses.First(c => c.Title == "Leadership Essentials");
            var firstAidCourse = courses.First(c => c.Title == "First Aid & CPR");

            var sessions = new List<TrainingSession>
            {
                new() { CourseId = safetyCourse.Id, InstructorId = instructor.Id, StartDate = DateTime.Today.AddDays(-30), EndDate = DateTime.Today.AddDays(-29), Capacity = 15, Location = "Room A", Status = SessionStatus.Completed },
                new() { CourseId = privacyCourse.Id, InstructorId = instructor.Id, StartDate = DateTime.Today.AddDays(-15), EndDate = DateTime.Today.AddDays(-14), Capacity = 10, Location = "Room B", Status = SessionStatus.Completed },
                new() { CourseId = leadershipCourse.Id, InstructorId = instructor.Id, StartDate = DateTime.Today.AddDays(7), EndDate = DateTime.Today.AddDays(9), Capacity = 12, Location = "Room C", Status = SessionStatus.Scheduled },
                new() { CourseId = firstAidCourse.Id, InstructorId = instructor.Id, StartDate = DateTime.Today.AddDays(14), EndDate = DateTime.Today.AddDays(14), Capacity = 8, Location = "Room A", Status = SessionStatus.Scheduled },
                new() { CourseId = safetyCourse.Id, InstructorId = instructor.Id, StartDate = DateTime.Today.AddDays(-60), EndDate = DateTime.Today.AddDays(-59), Capacity = 2, Location = "Room A", Status = SessionStatus.Cancelled },
            };

            context.TrainingSessions.AddRange(sessions);
            await context.SaveChangesAsync();

            // ---------- Enrollments (for the two completed sessions) ----------
            var completedSafetySession = sessions[0];
            var completedPrivacySession = sessions[1];

            var enrollees = employees.Where(e => e.EmployeeNumber != "EMP-1006").ToList(); // everyone except the instructor

            var enrollments = new List<Enrollment>();
            foreach (var emp in enrollees)
            {
                enrollments.Add(new Enrollment
                {
                    EmployeeId = emp.Id,
                    TrainingSessionId = completedSafetySession.Id,
                    EnrolledAt = DateTime.UtcNow.AddDays(-35),
                    Status = EnrollmentStatus.Completed
                });
            }

            // Only first 3 employees also took the privacy course
            foreach (var emp in enrollees.Take(3))
            {
                enrollments.Add(new Enrollment
                {
                    EmployeeId = emp.Id,
                    TrainingSessionId = completedPrivacySession.Id,
                    EnrolledAt = DateTime.UtcNow.AddDays(-20),
                    Status = EnrollmentStatus.Completed
                });
            }

            // One active enrollment in the upcoming leadership session
            enrollments.Add(new Enrollment
            {
                EmployeeId = enrollees[0].Id,
                TrainingSessionId = sessions[2].Id,
                EnrolledAt = DateTime.UtcNow.AddDays(-2),
                Status = EnrollmentStatus.Active
            });

            context.Enrollments.AddRange(enrollments);
            await context.SaveChangesAsync();

            // ---------- Assessment Results (for completed enrollments) ----------
            var random = new Random(42); // fixed seed for reproducible demo data
            var completedEnrollments = enrollments.Where(e => e.Status == EnrollmentStatus.Completed).ToList();

            var assessmentResults = new List<AssessmentResult>();
            foreach (var enr in completedEnrollments)
            {
                var session = sessions.First(s => s.Id == enr.TrainingSessionId);
                var course = courses.First(c => c.Id == session.CourseId);

                // Mostly passing scores, a couple of intentional fails for realistic data
                int score = random.Next(55, 100);
                bool passed = score >= course.PassingScore;

                assessmentResults.Add(new AssessmentResult
                {
                    EnrollmentId = enr.Id,
                    Score = score,
                    Passed = passed,
                    AssessedAt = enr.EnrolledAt.AddDays(1)
                });
            }

            context.AssessmentResults.AddRange(assessmentResults);
            await context.SaveChangesAsync();

            // ---------- Attendance (matches assessment results 1:1) ----------
            var attendanceRecords = completedEnrollments.Select(enr => new Attendance
            {
                EnrollmentId = enr.Id,
                IsPresent = true
            }).ToList();

            context.Attendances.AddRange(attendanceRecords);
            await context.SaveChangesAsync();

            // ---------- Certifications (only for passed assessments) ----------
            var certifications = new List<Certification>();
            foreach (var result in assessmentResults.Where(r => r.Passed))
            {
                var enrollment = completedEnrollments.First(e => e.Id == result.EnrollmentId);
                var session = sessions.First(s => s.Id == enrollment.TrainingSessionId);
                var course = courses.First(c => c.Id == session.CourseId);

                var issueDate = result.AssessedAt;
                var expiryDate = issueDate.AddMonths(course.CertificateValidityMonths);

                certifications.Add(new Certification
                {
                    EmployeeId = enrollment.EmployeeId,
                    CourseId = course.Id,
                    EnrollmentId = enrollment.Id,
                    CertificateNumber = $"CERT-{issueDate:yyyyMMdd}-{enrollment.Id:D5}",
                    IssueDate = issueDate,
                    ExpiryDate = expiryDate,
                    Status = CertificationStatus.Valid
                });
            }

            // Force one certification to be "expiring soon" for demo purposes
            if (certifications.Count > 0)
            {
                certifications[0].ExpiryDate = DateTime.UtcNow.AddDays(10);
            }
            // Force one to already be expired, if we have enough
            if (certifications.Count > 1)
            {
                certifications[1].ExpiryDate = DateTime.UtcNow.AddDays(-5);
            }

            context.Certifications.AddRange(certifications);
            await context.SaveChangesAsync();

            // ---------- Demo login accounts for each role ----------
            await CreateDemoUserAsync(userManager, "manager@corporatetraining.local", "Manager@12345", "TrainingManager", employees.First(e => e.EmployeeNumber == "EMP-1005").Id);
            await CreateDemoUserAsync(userManager, "instructor@corporatetraining.local", "Instructor@12345", "Instructor", instructor.Id);
            await CreateDemoUserAsync(userManager, "employee@corporatetraining.local", "Employee@12345", "Employee", enrollees[0].Id);
        }

        private static async Task CreateDemoUserAsync(UserManager<ApplicationUser> userManager, string email, string password, string role, int employeeId)
        {
            if (await userManager.FindByEmailAsync(email) is null)
            {
                var user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    EmployeeId = employeeId
                };

                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }
    }
}