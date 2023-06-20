using Employees_CRUD.Models;
using Microsoft.AspNetCore.Mvc;

namespace Employees_CRUD.Interfaces
{
    public interface IGetEmployeesRepository
    {
        public  Task<ActionResult<IEnumerable<Employee>>> GetEmployees(int page = 1, int pageSize = 10);

    }
}
