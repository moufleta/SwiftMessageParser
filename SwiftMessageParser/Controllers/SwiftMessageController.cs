using Microsoft.AspNetCore.Mvc;
using SwiftMessageParser.Business.Contracts;
using SwiftMessageParser.Business.Exceptions;

namespace SwiftMessageParser.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SwiftMessageController : ControllerBase
    {
        private readonly IParser parser;

        public SwiftMessageController(IParser parser)
        {
            this.parser = parser;
        }

        [HttpPost]
        public IActionResult UploadSwiftMessage(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("Invalid file");
                }

                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    var fileContent = reader.ReadToEnd();

                    parser.ParseSwiftMessageFile(fileContent);
                    return Ok("File uploaded and processed successfully");
                }
            }
            catch (ArgumentException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
            catch (SyntaxException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}