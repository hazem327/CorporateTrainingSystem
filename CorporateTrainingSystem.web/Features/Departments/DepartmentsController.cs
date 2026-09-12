using CorporateTrainingSystem.Application.Features.Departments.CreateDepartment;
using CorporateTrainingSystem.Application.Features.Departments.ListDepartments;
using CorporateTrainingSystem.Application.Features.Departments.UpdateDepartment;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CorporateTrainingSystem.Web.Features.Departments
{
    [Authorize(Roles = "Administrator,TrainingManager")]
    public class DepartmentsController : Controller
    {
        private readonly CreateDepartmentHandler _createHandler;
        private readonly ListDepartmentsHandler _listHandler;
        private readonly UpdateDepartmentHandler _updateHandler;
        private readonly IValidator<CreateDepartmentCommand> _createValidator;
        private readonly IValidator<UpdateDepartmentCommand> _updateValidator;

        public DepartmentsController(
            CreateDepartmentHandler createHandler,
            ListDepartmentsHandler listHandler,
            UpdateDepartmentHandler updateHandler,
            IValidator<CreateDepartmentCommand> createValidator,
            IValidator<UpdateDepartmentCommand> updateValidator)
        {
            _createHandler = createHandler;
            _listHandler = listHandler;
            _updateHandler = updateHandler;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<IActionResult> Index()
        {
            var departments = await _listHandler.HandleAsync();
            return View(departments);
        }

        [HttpGet]
        public IActionResult Create() => View(new CreateDepartmentCommand());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateDepartmentCommand command)
        {
            var validation = await _createValidator.ValidateAsync(command);
            if (!validation.IsValid)
            {
                foreach (var error in validation.Errors)
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                return View(command);
            }

            await _createHandler.HandleAsync(command);
            TempData["Success"] = "Department created.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var departments = await _listHandler.HandleAsync();
            var dept = departments.FirstOrDefault(d => d.Id == id);
            if (dept == null) return NotFound();

            return View(new UpdateDepartmentCommand { Id = dept.Id, Name = dept.Name, Description = dept.Description });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateDepartmentCommand command)
        {
            var validation = await _updateValidator.ValidateAsync(command);
            if (!validation.IsValid)
            {
                foreach (var error in validation.Errors)
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                return View(command);
            }

            var success = await _updateHandler.HandleAsync(command);
            TempData[success ? "Success" : "Error"] = success ? "Department updated." : "Department not found.";
            return RedirectToAction(nameof(Index));
        }
    }
}