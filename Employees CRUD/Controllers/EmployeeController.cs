using Employees_CRUD.Data;
using Employees_CRUD.DTOs;
using Employees_CRUD.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Employees_CRUD.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly EmployeeDbContext _dbContext;

        public EmployeeController(EmployeeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees(int page = 1, int pageSize = 50)
        {
            var employees = await _dbContext.Employees
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(employees);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var employee = await _dbContext.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            return employee;
        }


        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployeesByEmailOrMobile(string? email, string? mobile)
        {
            var employees = await _dbContext.Employees
                .Where(e => e.Email == email || e.Mobile == mobile)
                .ToListAsync();

            return Ok(employees);
        }




        [HttpPost]
        public async Task<ActionResult<Employee>> CreateEmployee([FromForm]EmployeDto employeDto)
        {

            //var formFile = Request.Form.Files.FirstOrDefault();
            var formFile = employeDto.file;

            if (formFile == null || formFile.Length == 0)
                return BadRequest("No file was provided for upload.");

            var fileName = Guid.NewGuid() + formFile.FileName;

            // Specify the FTP server details
            var ftpServer = "ftp://localhost";
            var ftpUsername = "USERNAME";
            var ftpPassword = "PASSWORD";
            //var guid = Guid.NewGuid().ToString();

            // Create a request using the FTP URL
            var request = (FtpWebRequest)WebRequest.Create($"{ftpServer}/{fileName}");
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(ftpUsername, ftpPassword);

            // Upload the file to the FTP server
            using (var stream = formFile.OpenReadStream())
            {
                using (var ftpStream = request.GetRequestStream())
                {
                    stream.CopyTo(ftpStream);
                }
            }

            var employee = new Employee
            {
                Email = employeDto.Email,
                Address = employeDto.Address,
                Mobile = employeDto.Mobile,
                Name = employeDto.Name,
                File = fileName
            };     
            _dbContext.Employees.Add(employee);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromForm]EmployeDto employeDto)
        {
            //var formFile = Request.Form.Files.FirstOrDefault();
            var formFile = employeDto.file;

            if (formFile == null || formFile.Length == 0)
                return BadRequest("No file was provided for upload.");


            var employeeData = await _dbContext.Employees.FindAsync(id);

            if(employeeData.File != employeDto.file.FileName)
            {

                //_dbContext.Entry(employeeData).State = EntityState.Detached;


                var fileName = Guid.NewGuid() + formFile.FileName;


                // Specify the FTP server details
                var ftpServer = "ftp://localhost";
                var ftpUsername = "USERNAME";
                var ftpPassword = "PASSWORD";
                //var guid = Guid.NewGuid().ToString();

                // Check if employee already has a file
                if (!string.IsNullOrEmpty(employeeData.File))
                {
                    await DeleteFileFromFtp(employeeData.File);
                }

                // Create a request using the FTP URL
                var request = (FtpWebRequest)WebRequest.Create($"{ftpServer}/{fileName}");
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(ftpUsername, ftpPassword);

                // Upload the file to the FTP server
                using (var stream = formFile.OpenReadStream())
                {
                    using (var ftpStream = request.GetRequestStream())
                    {
                        stream.CopyTo(ftpStream);
                    }
                }
                employeeData.File = fileName;

            }

            employeeData.Id = id;
            employeeData.Email = employeDto.Email;
            employeeData.Address = employeDto.Address;
            employeeData.Mobile = employeDto.Mobile;
            employeeData.Name = employeDto.Name;

            //_dbContext.Entry(employeeData).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteEmployee(int id)
        //{
        //    var employee = await _dbContext.Employees.FindAsync(id);

        //    if (employee == null)
        //    {
        //        return NotFound();
        //    }

        //    _dbContext.Employees.Remove(employee);
        //    await _dbContext.SaveChangesAsync();

        //    return NoContent();
        //}

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _dbContext.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            // Delete file from FTP server
            var ftpServer = "ftp://localhost";
            var ftpUsername = "USERNAMED";
            var ftpPassword = "PASSWORD";
            var fileName = employee.File;

            try
            {
                // Create a request using the FTP URL
                var request = (FtpWebRequest)WebRequest.Create($"{ftpServer}/{fileName}");
                request.Method = WebRequestMethods.Ftp.DeleteFile;
                request.Credentials = new NetworkCredential(ftpUsername, ftpPassword);

                // Send the request to the FTP server
                using (var response = (FtpWebResponse)request.GetResponse())
                {
                    // The file has been deleted successfully from the FTP server
                }
            }
            catch (Exception ex)
            {
                // Handle any errors that occurred during the deletion process
                return StatusCode(500, ex.Message);
            }

            // Remove the employee from the database
            _dbContext.Employees.Remove(employee);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }



        //[HttpDelete("deleteSelected")]
        //public async Task<IActionResult> DeleteSelectedEmployees([FromBody]int[] ids)
        //{
        //    var employees = await _dbContext.Employees
        //        .Where(e => ids.Contains(e.Id))
        //        .ToListAsync();

        //    if (employees.Count == 0)
        //    {
        //        return NotFound();
        //    }

        //    _dbContext.Employees.RemoveRange(employees);
        //    await _dbContext.SaveChangesAsync();

        //    return NoContent();
        //}



        [HttpDelete("deleteSelected")]
        public async Task<IActionResult> DeleteSelectedEmployees([FromBody] int[] ids)
        {
            var employees = await _dbContext.Employees
                .Where(e => ids.Contains(e.Id))
                .ToListAsync();

            if (employees.Count == 0)
            {
                return NotFound();
            }

            // Delete files from FTP server
            var ftpServer = "ftp://localhost";
            var ftpUsername = "USERNAME";
            var ftpPassword = "PASSWORD";

            try
            {
                foreach (var employee in employees)
                {
                    var fileName = employee.File;

                    // Create a request using the FTP URL
                    var request = (FtpWebRequest)WebRequest.Create($"{ftpServer}/{fileName}");
                    request.Method = WebRequestMethods.Ftp.DeleteFile;
                    request.Credentials = new NetworkCredential(ftpUsername, ftpPassword);

                    // Send the request to the FTP server
                    using (var response = (FtpWebResponse)request.GetResponse())
                    {
                        // The file has been deleted successfully from the FTP server
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any errors that occurred during the deletion process
                return StatusCode(500, ex.Message);
            }

            // Remove the employees from the database
            _dbContext.Employees.RemoveRange(employees);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }





        //[HttpGet("download/{fileName}")]
        //public IActionResult DownloadFile(string fileName)
        //{
        //    try
        //    {
        //        // Specify the FTP server details
        //        var ftpServer = "ftp://localhost";
        //        var ftpUsername = "USERNAME";
        //        var ftpPassword = "PASSWORD";

        //        // Create a request using the FTP URL
        //        var request = (FtpWebRequest)WebRequest.Create($"{ftpServer}/{fileName}");
        //        request.Method = WebRequestMethods.Ftp.DownloadFile;
        //        request.Credentials = new NetworkCredential(ftpUsername, ftpPassword);

        //        // Get the response from the FTP server
        //        using (var response = (FtpWebResponse)request.GetResponse())
        //        {
        //            using (var ftpStream = response.GetResponseStream())
        //            {
        //                // Read the file content
        //                using (var memoryStream = new MemoryStream())
        //                {
        //                    ftpStream.CopyTo(memoryStream);
        //                    var fileContent = memoryStream.ToArray();

        //                    Response.Headers.Add("Content-Disposition", "inline; filename=" + fileName);

        //                    // Return the file content as a response
        //                    return File(fileContent, "application/octet-stream", fileName);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message);
        //    }
        //}

        [HttpGet("download/{fileName}")]
        public IActionResult DownloadFile(string fileName)
        {
            try
            {
                // Specify the FTP server details
                var ftpServer = "ftp://localhost";
                var ftpUsername = "USERNAME";
                var ftpPassword = "PASSWORD";

                // Create a request using the FTP URL
                var request = (FtpWebRequest)WebRequest.Create($"{ftpServer}/{fileName}");
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential(ftpUsername, ftpPassword);

                // Get the response from the FTP server
                using (var response = (FtpWebResponse)request.GetResponse())
                {
                    using (var ftpStream = response.GetResponseStream())
                    {
                        // Read the file content
                        using (var memoryStream = new MemoryStream())
                        {
                            ftpStream.CopyTo(memoryStream);
                            var fileContent = memoryStream.ToArray();

                            // Set the content type header based on the file extension
                            var contentType = GetContentType(fileName);
                            Response.Headers["Content-Type"] = contentType;

                            // Return the file content as a response
                            return File(fileContent, contentType);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        private string GetContentType(string fileName)
        {
            var fileExtension = Path.GetExtension(fileName).ToLowerInvariant();
            switch (fileExtension)
            {
                case ".pdf":
                    return "application/pdf";
                case ".txt":
                    return "application/txt";
                case ".doc":
                case ".docx":
                    return "application/msword";
                case ".xls":
                case ".xlsx":
                    return "application/vnd.ms-excel";
                // Add more file extensions and corresponding content types as needed
                default:
                    return "application/octet-stream";
            }
        }


        private async Task DeleteFileFromFtp(string fileName)
        {
            // Get FTP settings
            // Specify the FTP server details
            var ftpServer = "ftp://localhost/";
            var ftpUsername = "USERNAME";
            var ftpPassword = "PASSWORD";

            var filePath = ftpServer + fileName;

            // Create an FtpWebRequest
            var request = (FtpWebRequest)WebRequest.Create(filePath);
            // Set the method to DeleteFile
            request.Method = WebRequestMethods.Ftp.DeleteFile;
            // Set the NetworkCredentials
            request.Credentials = new NetworkCredential(ftpUsername, ftpPassword);

            // Send the request to delete the file
            using (var response = (FtpWebResponse)await request.GetResponseAsync())
            {
                // Check if the deletion was successful
                if (response.StatusCode != FtpStatusCode.FileActionOK)
                {
                    throw new Exception("Failed to delete the file from the FTP server.");
                }
            }
        }





        private bool EmployeeExists(int id)
        {
            return _dbContext.Employees.Any(e => e.Id == id);
        }
    }
}
