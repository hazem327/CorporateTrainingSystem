using CorporateTrainingSystem.Domain.Interfaces;
using CorporateTrainingSystem.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CorporateTrainingSystem.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var courses = await _unitOfWork.Repository<Course>().GetAllAsync();
            var sessions = await _unitOfWork.Repository<TrainingSession>().GetAllAsync();
            var employees = await _unitOfWork.Repository<Employee>().GetAllAsync();
            var enrollments = await _unitOfWork.Repository<Enrollment>().GetAllAsync();
            var certifications = await _unitOfWork.Repository<Certification>().GetAllAsync();
            var departments = await _unitOfWork.Repository<Department>().GetAllAsync();

            ViewBag.CourseCount = courses.Count();
            ViewBag.ActiveCourseCount = courses.Count(c => c.IsActive);
            ViewBag.SessionCount = sessions.Count();
            ViewBag.ScheduledSessionCount = sessions.Count(s => s.Status == SessionStatus.Scheduled);
            ViewBag.EmployeeCount = employees.Count();
            ViewBag.ActiveEmployeeCount = employees.Count(e => e.IsActive);
            ViewBag.EnrollmentCount = enrollments.Count();
            ViewBag.CertificationCount = certifications.Count();
            ViewBag.DepartmentCount = departments.Count();

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}