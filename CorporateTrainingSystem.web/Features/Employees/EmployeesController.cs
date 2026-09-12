using CorporateTrainingSystem.Application.Features.Employees.CreateEmployee;
using CorporateTrainingSystem.Application.Features.Employees.ListEmployees;
using CorporateTrainingSystem.Domain.Entities;
using CorporateTrainingSystem.Domain.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using CorporateTrainingSystem.Application.Features.Employees.UpdateEmployee;
using CorporateTrainingSystem.Application.Features.Employees.DeactivateEmployee;

namespace CorporateTrainingSystem.Web.Features.Employees
{
    [Authorize]
    public class EmployeesController : Controller
    {
        private readonly CreateEmployeeHandler _createHandler;
        private readonly ListEmployeesHandler _listHandler;
        private readonly IValidator<CreateEmployeeCommand> _createValidator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UpdateEmployeeHandler _updateHandler;
        private readonly DeactivateEmployeeHandler _deactivateHandler;
        private readonly IValidator<UpdateEmployeeCommand> _updateValidator;

        public EmployeesController(
            CreateEmployeeHandler createHandler,
            ListEmployeesHandler listHandler,
            IValidator<CreateEmployeeCommand> createValidator,
            IUnitOfWork unitOfWork,
            UpdateEmployeeHandler updateHandler,
            DeactivateEmployeeHandler deactivateHandler,
            IValidator<UpdateEmployeeCommand> updateValidator)
        {
            _createHandler = createHandler;
            _listHandler = listHandler;
            _createValidator = createValidator;
            _unitOfWork = unitOfWork;
            _updateHandler = updateHandler;
            _deactivateHandler = deactivateHandler;
            _updateValidator = updateValidator;
        }

        public async Task<IActionResult> Index(EmployeeListQuery query)
        {
            var result = await _listHandler.HandleAsync(query);
            await LoadDepartments();
            ViewBag.Query = query;
            return View(result);
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
                [Authorize(Roles = "Administrator,TrainingManager")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var employee = await _unitOfWork.Repository<Employee>().GetByIdAsync(id);
            if (employee == null) return NotFound();

            await LoadDepartments();

            return View(new UpdateEmployeeCommand
            {
                Id = employee.Id,
                EmployeeNumber = employee.EmployeeNumber,
                FullName = employee.FullName,
                Email = employee.Email,
                JobTitle = employee.JobTitle,
                HireDate = employee.HireDate,
                DepartmentId = employee.DepartmentId
            });
        }

        [Authorize(Roles = "Administrator,TrainingManager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateEmployeeCommand command)
        {
            var validation = await _updateValidator.ValidateAsync(command);
            if (!validation.IsValid)
            {
                foreach (var error in validation.Errors)
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                await LoadDepartments();
                return View(command);
            }

            var success = await _updateHandler.HandleAsync(command);
            TempData[success ? "Success" : "Error"] = success ? "Employee updated." : "Employee not found.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Administrator,TrainingManager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(int id)
        {
            var success = await _deactivateHandler.HandleAsync(id);
            TempData[success ? "Success" : "Error"] = success ? "Employee deactivated." : "Employee not found.";
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