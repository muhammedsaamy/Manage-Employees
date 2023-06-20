using Microsoft.AspNetCore.Mvc;

namespace Employees_CRUD.Interfaces
{
    public interface IDeleteEmployeeRepository
    {
        public Task<IActionResult> DeleteEmployee(int id);

    }
}
