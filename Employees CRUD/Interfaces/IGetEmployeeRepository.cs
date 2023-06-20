using Employees_CRUD.Models;
using Microsoft.AspNetCore.Mvc;

namespace Employees_CRUD.Interfaces
{
    public interface IGetEmployeeRepository
    {
        public Task<ActionResult<Employee>> GetEmployee(int id);

    }
}
