using CorporateTrainingSystem.Application.Features.Courses.CreateCourse;
using CorporateTrainingSystem.Application.Features.Courses.ListCourses;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CorporateTrainingSystem.Web.Features.Courses
{
    [Authorize]
    public class CoursesController : Controller
    {
        private readonly CreateCourseHandler _createHandler;
        private readonly ListCoursesHandler _listHandler;
        private readonly IValidator<CreateCourseCommand> _createValidator;

        public CoursesController(
            CreateCourseHandler createHandler,
            ListCoursesHandler listHandler,
            IValidator<CreateCourseCommand> createValidator)
        {
            _createHandler = createHandler;
            _listHandler = listHandler;
            _createValidator = createValidator;
        }

        public async Task<IActionResult> Index()
        {
            var courses = await _listHandler.HandleAsync();
            return View(courses);
        }

        [Authorize(Roles = "Administrator,TrainingManager")]
        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateCourseCommand());
        }

        [Authorize(Roles = "Administrator,TrainingManager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCourseCommand command)
        {
            var validation = await _createValidator.ValidateAsync(command);
            if (!validation.IsValid)
            {
                foreach (var error in validation.Errors)
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                return View(command);
            }

            await _createHandler.HandleAsync(command);
            TempData["Success"] = $"Course '{command.Title}' created.";
            return RedirectToAction(nameof(Index));
        }
    }
}