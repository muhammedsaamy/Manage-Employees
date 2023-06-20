using System.ComponentModel.DataAnnotations;

namespace Employees_CRUD.DTOs
{
    public class EmployeDto
    {

        public string Name { get; set; }

        public string Email { get; set; }

        public string Mobile { get; set; }

        public string Address { get; set; }
        public string FileName { get; set; } = "angular";

        public IFormFile file { get; set; }
    }
}
