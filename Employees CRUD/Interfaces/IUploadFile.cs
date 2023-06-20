using Microsoft.AspNetCore.Mvc;

namespace Employees_CRUD.Interfaces
{
    public interface IUploadFile
    {
        public Task<IActionResult> UploadFile();
    };
}
