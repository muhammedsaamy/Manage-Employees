using Employees_CRUD.Models;
using Microsoft.AspNetCore.Mvc;

namespace Employees_CRUD.Interfaces
{
    public interface IUpdateEmployeeRepository
    {
        public Task<IActionResult> UpdateEmployee(int id, Employee employee);

    }
}
