using CorporateTrainingSystem.Application.Features.Enrollments.CancelEnrollment;
using CorporateTrainingSystem.Application.Features.Enrollments.EnrollEmployee;
using CorporateTrainingSystem.Domain.Entities;
using CorporateTrainingSystem.Domain.Interfaces;
using Moq;
using Xunit;

namespace CorporateTrainingSystem.Tests
{
    public class EnrollmentBusinessRuleTests
    {
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<IRepository<TrainingSession>> _mockSessionRepo;
        private readonly Mock<IRepository<Employee>> _mockEmployeeRepo;
        private readonly Mock<IRepository<Enrollment>> _mockEnrollmentRepo;

        public EnrollmentBusinessRuleTests()
        {
            _mockUow = new Mock<IUnitOfWork>();
            _mockSessionRepo = new Mock<IRepository<TrainingSession>>();
            _mockEmployeeRepo = new Mock<IRepository<Employee>>();
            _mockEnrollmentRepo = new Mock<IRepository<Enrollment>>();

            _mockUow.Setup(u => u.Repository<TrainingSession>()).Returns(_mockSessionRepo.Object);
            _mockUow.Setup(u => u.Repository<Employee>()).Returns(_mockEmployeeRepo.Object);
            _mockUow.Setup(u => u.Repository<Enrollment>()).Returns(_mockEnrollmentRepo.Object);
        }

        [Fact]
        public async Task EnrollEmployee_WhenDuplicateActiveEnrollmentExists_RejectsEnrollment_BR01()
        {
            // Arrange
            int sessionId = 1;
            int employeeId = 10;

            var session = new TrainingSession
            {
                Id = sessionId,
                Capacity = 10,
                Status = SessionStatus.Scheduled,
                StartDate = DateTime.Today.AddDays(1),
                EndDate = DateTime.Today.AddDays(5)
            };

            var employee = new Employee
            {
                Id = employeeId,
                FullName = "Alice Smith",
                IsActive = true
            };

            var existingEnrollments = new List<Enrollment>
            {
                new Enrollment
                {
                    Id = 100,
                    EmployeeId = employeeId,
                    TrainingSessionId = sessionId,
                    Status = EnrollmentStatus.Active
                }
            }.AsQueryable();

            _mockSessionRepo.Setup(r => r.GetByIdAsync(sessionId)).ReturnsAsync(session);
            _mockEmployeeRepo.Setup(r => r.GetByIdAsync(employeeId)).ReturnsAsync(employee);
            _mockEnrollmentRepo.Setup(r => r.Query()).Returns(existingEnrollments);

            var handler = new EnrollEmployeeHandler(_mockUow.Object);
            var command = new EnrollEmployeeCommand { EmployeeId = employeeId, TrainingSessionId = sessionId };

            // Act
            var result = await handler.HandleAsync(command);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("BR-01", result.ErrorMessage);
            _mockEnrollmentRepo.Verify(r => r.AddAsync(It.IsAny<Enrollment>()), Times.Never);
        }

        [Fact]
        public async Task EnrollEmployee_WhenCapacityReached_RejectsEnrollment_BR02()
        {
            // Arrange
            int sessionId = 1;
            int employeeId = 10;
            int capacity = 2;

            var session = new TrainingSession
            {
                Id = sessionId,
                Capacity = capacity,
                Status = SessionStatus.Scheduled,
                StartDate = DateTime.Today.AddDays(1),
                EndDate = DateTime.Today.AddDays(5)
            };

            var employee = new Employee
            {
                Id = employeeId,
                FullName = "Bob Jones",
                IsActive = true
            };

            // Already 2 active enrollments for other employees
            var existingEnrollments = new List<Enrollment>
            {
                new Enrollment { Id = 1, EmployeeId = 101, TrainingSessionId = sessionId, Status = EnrollmentStatus.Active },
                new Enrollment { Id = 2, EmployeeId = 102, TrainingSessionId = sessionId, Status = EnrollmentStatus.Active }
            }.AsQueryable();

            _mockSessionRepo.Setup(r => r.GetByIdAsync(sessionId)).ReturnsAsync(session);
            _mockEmployeeRepo.Setup(r => r.GetByIdAsync(employeeId)).ReturnsAsync(employee);
            _mockEnrollmentRepo.Setup(r => r.Query()).Returns(existingEnrollments);

            var handler = new EnrollEmployeeHandler(_mockUow.Object);
            var command = new EnrollEmployeeCommand { EmployeeId = employeeId, TrainingSessionId = sessionId };

            // Act
            var result = await handler.HandleAsync(command);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("BR-02", result.ErrorMessage);
            _mockEnrollmentRepo.Verify(r => r.AddAsync(It.IsAny<Enrollment>()), Times.Never);
        }

        [Fact]
        public async Task EnrollEmployee_WhenSessionIsCancelled_RejectsEnrollment_BR05()
        {
            // Arrange
            int sessionId = 1;
            int employeeId = 10;

            var session = new TrainingSession
            {
                Id = sessionId,
                Capacity = 10,
                Status = SessionStatus.Cancelled,
                StartDate = DateTime.Today.AddDays(1),
                EndDate = DateTime.Today.AddDays(5)
            };

            _mockSessionRepo.Setup(r => r.GetByIdAsync(sessionId)).ReturnsAsync(session);

            var handler = new EnrollEmployeeHandler(_mockUow.Object);
            var command = new EnrollEmployeeCommand { EmployeeId = employeeId, TrainingSessionId = sessionId };

            // Act
            var result = await handler.HandleAsync(command);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("BR-05", result.ErrorMessage);
            _mockEnrollmentRepo.Verify(r => r.AddAsync(It.IsAny<Enrollment>()), Times.Never);
        }

        [Fact]
        public async Task EnrollEmployee_WhenValidAndWithinCapacity_Succeeds()
        {
            // Arrange
            int sessionId = 1;
            int employeeId = 10;

            var session = new TrainingSession
            {
                Id = sessionId,
                Capacity = 15,
                Status = SessionStatus.Scheduled,
                StartDate = DateTime.Today.AddDays(1),
                EndDate = DateTime.Today.AddDays(5)
            };

            var employee = new Employee
            {
                Id = employeeId,
                FullName = "Charlie Brown",
                IsActive = true
            };

            var existingEnrollments = new List<Enrollment>().AsQueryable();

            _mockSessionRepo.Setup(r => r.GetByIdAsync(sessionId)).ReturnsAsync(session);
            _mockEmployeeRepo.Setup(r => r.GetByIdAsync(employeeId)).ReturnsAsync(employee);
            _mockEnrollmentRepo.Setup(r => r.Query()).Returns(existingEnrollments);

            Enrollment? addedEnrollment = null;
            _mockEnrollmentRepo.Setup(r => r.AddAsync(It.IsAny<Enrollment>()))
                .Callback<Enrollment>(e => addedEnrollment = e)
                .Returns(Task.CompletedTask);

            var handler = new EnrollEmployeeHandler(_mockUow.Object);
            var command = new EnrollEmployeeCommand { EmployeeId = employeeId, TrainingSessionId = sessionId };

            // Act
            var result = await handler.HandleAsync(command);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(addedEnrollment);
            Assert.Equal(employeeId, addedEnrollment.EmployeeId);
            Assert.Equal(sessionId, addedEnrollment.TrainingSessionId);
            Assert.Equal(EnrollmentStatus.Active, addedEnrollment.Status);
            _mockUow.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CancelEnrollment_SetsStatusToCancelled_NeverHardDeletes_BR07()
        {
            // Arrange
            int enrollmentId = 55;
            var enrollment = new Enrollment
            {
                Id = enrollmentId,
                EmployeeId = 1,
                TrainingSessionId = 2,
                Status = EnrollmentStatus.Active
            };

            _mockEnrollmentRepo.Setup(r => r.GetByIdAsync(enrollmentId)).ReturnsAsync(enrollment);

            var handler = new CancelEnrollmentHandler(_mockUow.Object);
            var command = new CancelEnrollmentCommand { EnrollmentId = enrollmentId };

            // Act
            var result = await handler.HandleAsync(command);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(EnrollmentStatus.Cancelled, enrollment.Status);
            _mockEnrollmentRepo.Verify(r => r.Delete(It.IsAny<Enrollment>()), Times.Never);
            _mockEnrollmentRepo.Verify(r => r.Update(enrollment), Times.Once);
            _mockUow.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CancelEnrollment_WhenAlreadyCancelled_ReturnsError()
        {
            // Arrange
            int enrollmentId = 55;
            var enrollment = new Enrollment
            {
                Id = enrollmentId,
                EmployeeId = 1,
                TrainingSessionId = 2,
                Status = EnrollmentStatus.Cancelled
            };

            _mockEnrollmentRepo.Setup(r => r.GetByIdAsync(enrollmentId)).ReturnsAsync(enrollment);

            var handler = new CancelEnrollmentHandler(_mockUow.Object);
            var command = new CancelEnrollmentCommand { EnrollmentId = enrollmentId };

            // Act
            var result = await handler.HandleAsync(command);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Enrollment is already cancelled.", result.ErrorMessage);
            _mockUow.Verify(u => u.SaveChangesAsync(), Times.Never);
        }
        [Fact]
public async Task EnrollEmployee_WhenSessionIsCompleted_RejectsEnrollment()
{
    // Arrange
    int sessionId = 1;
    int employeeId = 10;

    var session = new TrainingSession
    {
        Id = sessionId,
        Capacity = 10,
        Status = SessionStatus.Completed,
        StartDate = DateTime.Today.AddDays(-10),
        EndDate = DateTime.Today.AddDays(-5)
    };

    _mockSessionRepo.Setup(r => r.GetByIdAsync(sessionId)).ReturnsAsync(session);

    var handler = new EnrollEmployeeHandler(_mockUow.Object);
    var command = new EnrollEmployeeCommand { EmployeeId = employeeId, TrainingSessionId = sessionId };

    // Act
    var result = await handler.HandleAsync(command);

    // Assert
    Assert.False(result.Success);
    Assert.Contains("completed", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    _mockEnrollmentRepo.Verify(r => r.AddAsync(It.IsAny<Enrollment>()), Times.Never);
}

[Fact]
public async Task EnrollEmployee_WhenEmployeeIsInactive_RejectsEnrollment()
{
    // Arrange
    int sessionId = 1;
    int employeeId = 10;

    var session = new TrainingSession
    {
        Id = sessionId,
        Capacity = 10,
        Status = SessionStatus.Scheduled,
        StartDate = DateTime.Today.AddDays(1),
        EndDate = DateTime.Today.AddDays(5)
    };

    var inactiveEmployee = new Employee
    {
        Id = employeeId,
        FullName = "Dana White",
        IsActive = false
    };

    _mockSessionRepo.Setup(r => r.GetByIdAsync(sessionId)).ReturnsAsync(session);
    _mockEmployeeRepo.Setup(r => r.GetByIdAsync(employeeId)).ReturnsAsync(inactiveEmployee);

    var handler = new EnrollEmployeeHandler(_mockUow.Object);
    var command = new EnrollEmployeeCommand { EmployeeId = employeeId, TrainingSessionId = sessionId };

    // Act
    var result = await handler.HandleAsync(command);

    // Assert
    Assert.False(result.Success);
    Assert.Contains("inactive", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    _mockEnrollmentRepo.Verify(r => r.AddAsync(It.IsAny<Enrollment>()), Times.Never);
}

[Fact]
public async Task EnrollEmployee_WhenSessionNotFound_ReturnsError()
{
    // Arrange
    int sessionId = 999;
    int employeeId = 10;

    _mockSessionRepo.Setup(r => r.GetByIdAsync(sessionId)).ReturnsAsync((TrainingSession?)null);

    var handler = new EnrollEmployeeHandler(_mockUow.Object);
    var command = new EnrollEmployeeCommand { EmployeeId = employeeId, TrainingSessionId = sessionId };

    // Act
    var result = await handler.HandleAsync(command);

    // Assert
    Assert.False(result.Success);
    Assert.Contains("not found", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    _mockEnrollmentRepo.Verify(r => r.AddAsync(It.IsAny<Enrollment>()), Times.Never);
}

[Fact]
public async Task EnrollEmployee_WhenEmployeeNotFound_ReturnsError()
{
    // Arrange
    int sessionId = 1;
    int employeeId = 999;

    var session = new TrainingSession
    {
        Id = sessionId,
        Capacity = 10,
        Status = SessionStatus.Scheduled,
        StartDate = DateTime.Today.AddDays(1),
        EndDate = DateTime.Today.AddDays(5)
    };

    _mockSessionRepo.Setup(r => r.GetByIdAsync(sessionId)).ReturnsAsync(session);
    _mockEmployeeRepo.Setup(r => r.GetByIdAsync(employeeId)).ReturnsAsync((Employee?)null);

    var handler = new EnrollEmployeeHandler(_mockUow.Object);
    var command = new EnrollEmployeeCommand { EmployeeId = employeeId, TrainingSessionId = sessionId };

    // Act
    var result = await handler.HandleAsync(command);

    // Assert
    Assert.False(result.Success);
    Assert.Contains("not found", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    _mockEnrollmentRepo.Verify(r => r.AddAsync(It.IsAny<Enrollment>()), Times.Never);
}
    }
}
