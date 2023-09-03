using Microsoft.AspNetCore.Mvc;
using NLog;
using SwiftMessageParser.Business;
using SwiftMessageParser.Business.Contracts;
using SwiftMessageParser.Business.Exceptions;

namespace SwiftMessageParser.Controllers
{
    [ApiController]
    [Route("api/message")]
    public class SwiftMessageController : ControllerBase
    {
        private readonly IParser parser;

        public SwiftMessageController(IParser parser)
        {
            this.parser = parser;
        }

        [HttpPost("insert")]
        public IActionResult InsertSwiftMessage(IFormFile file)
        {
            try
            {
                MyLogger.GetInstance().Info("Entering SwiftMessageController. InsertSwiftMessage method.");

                if (file == null || file.Length == 0)
                {
                    MyLogger.GetInstance().Error("Invalid file.");
                    return BadRequest("Invalid file.");
                }

                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    var fileContent = reader.ReadToEnd();

                    parser.ParseSwiftMessageFile(fileContent);

                    MyLogger.GetInstance().Info("File uploaded and processed successfully.");
                    return Ok("File uploaded and processed successfully.");
                }
            }
            catch (ArgumentException ex)
            {
                MyLogger.GetInstance().Error("ArgumentException occurred: " + ex.Message);
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
            catch (SyntaxException ex)
            {
                MyLogger.GetInstance().Error("SyntaxException occurred: " + ex.Message);
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Unhandled exception occurred: " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}