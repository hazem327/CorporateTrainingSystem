using CorporateTrainingSystem.Application.Features.Employees.CreateEmployee;
using CorporateTrainingSystem.Application.Features.Employees.ListEmployees;
using CorporateTrainingSystem.Domain.Entities;
using CorporateTrainingSystem.Domain.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CorporateTrainingSystem.Web.Features.Employees
{
    [Authorize]
    public class EmployeesController : Controller
    {
        private readonly CreateEmployeeHandler _createHandler;
        private readonly ListEmployeesHandler _listHandler;
        private readonly IValidator<CreateEmployeeCommand> _createValidator;
        private readonly IUnitOfWork _unitOfWork;

        public EmployeesController(
            CreateEmployeeHandler createHandler,
            ListEmployeesHandler listHandler,
            IValidator<CreateEmployeeCommand> createValidator,
            IUnitOfWork unitOfWork)
        {
            _createHandler = createHandler;
            _listHandler = listHandler;
            _createValidator = createValidator;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var employees = await _listHandler.HandleAsync();
            return View(employees);
        }

        [Authorize(Roles = "Administrator,TrainingManager")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadDepartments();
            return View(new CreateEmployeeCommand());
        }

        [Authorize(Roles = "Administrator,TrainingManager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEmployeeCommand command)
        {
            var validation = await _createValidator.ValidateAsync(command);
            if (!validation.IsValid)
            {
                foreach (var error in validation.Errors)
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                await LoadDepartments();
                return View(command);
            }

            await _createHandler.HandleAsync(command);
            TempData["Success"] = $"Employee '{command.FullName}' created.";
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadDepartments()
        {
            var departments = (await _unitOfWork.Repository<Department>().GetAllAsync())
                .Select(d => new { d.Id, d.Name })
                .ToList();
            ViewBag.Departments = new SelectList(departments, "Id", "Name");
        }
    }
}