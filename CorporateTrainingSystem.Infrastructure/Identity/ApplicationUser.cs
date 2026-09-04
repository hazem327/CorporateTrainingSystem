using CorporateTrainingSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace CorporateTrainingSystem.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public int? EmployeeId { get; set; }
        public Employee? Employee { get; set; }
    }
}