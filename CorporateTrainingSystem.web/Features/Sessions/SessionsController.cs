using CorporateTrainingSystem.Application.Features.Sessions.CreateSession;
using CorporateTrainingSystem.Application.Features.Sessions.ListSessions;
using CorporateTrainingSystem.Domain.Entities;
using CorporateTrainingSystem.Domain.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CorporateTrainingSystem.Web.Features.Sessions
{
    [Authorize]
    public class SessionsController : Controller
    {
        private readonly CreateSessionHandler _createHandler;
        private readonly ListSessionsHandler _listHandler;
        private readonly IValidator<CreateSessionCommand> _createValidator;
        private readonly IUnitOfWork _unitOfWork;

        public SessionsController(
            CreateSessionHandler createHandler,
            ListSessionsHandler listHandler,
            IValidator<CreateSessionCommand> createValidator,
            IUnitOfWork unitOfWork)
        {
            _createHandler = createHandler;
            _listHandler = listHandler;
            _createValidator = createValidator;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var sessions = await _listHandler.HandleAsync();
            return View(sessions);
        }

        [Authorize(Roles = "Administrator,TrainingManager")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadDropdowns();
            return View(new CreateSessionCommand());
        }

        [Authorize(Roles = "Administrator,TrainingManager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateSessionCommand command)
        {
            var validation = await _createValidator.ValidateAsync(command);
            if (!validation.IsValid)
            {
                foreach (var error in validation.Errors)
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                await LoadDropdowns();
                return View(command);
            }

            await _createHandler.HandleAsync(command);
            TempData["Success"] = "Training session created.";
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadDropdowns()
        {
            var courses = (await _unitOfWork.Repository<Course>().GetAllAsync())
                .Where(c => c.IsActive)
                .Select(c => new { c.Id, c.Title })
                .ToList();
            ViewBag.Courses = new SelectList(courses, "Id", "Title");

            var employees = (await _unitOfWork.Repository<Employee>().GetAllAsync())
                .Where(e => e.IsActive)
                .Select(e => new { e.Id, e.FullName })
                .ToList();
            ViewBag.Instructors = new SelectList(employees, "Id", "FullName");
        }
    }
}