using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Employees_CRUD.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileController : ControllerBase
    {
        [HttpPost("upload")]
        public IActionResult UploadFile(IFormFile file)
        {
            if (!Request.HasFormContentType)
                return BadRequest("Unsupported media type.");

            try
            {
                //var formFile = Request.Form.Files.FirstOrDefault();
                var formFile = file;

                if (formFile == null || formFile.Length == 0)
                    return BadRequest("No file was provided for upload.");

                var fileName = formFile.FileName;

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

                return Ok("File uploaded successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

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

                            // Return the file content as a response
                            return File(fileContent, "application/octet-stream", fileName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
