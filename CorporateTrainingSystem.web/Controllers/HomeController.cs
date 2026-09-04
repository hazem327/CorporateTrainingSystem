using CorporateTrainingSystem.Domain.Interfaces;
using CorporateTrainingSystem.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public HomeController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var departmentCount = (await _unitOfWork.Repository<Department>().GetAllAsync()).Count();
        ViewBag.DepartmentCount = departmentCount;
        return View();
    }
}