using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using UsersApi.Models;
using UsersApi.Services;

namespace UsersApi.Functions
{
    public class UserFunctions
    {
        private readonly IUserService _userService;

        public UserFunctions(IUserService userService)
        {
            _userService = userService;
        }

        [FunctionName("CreateUser")]
        public async Task<IActionResult> CreateUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "users")] HttpRequest req,
            ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var user = JsonConvert.DeserializeObject<User>(requestBody);

            var result = await _userService.CreateUser(user);

            switch (result.StatusCode)
            {
                case HttpStatusCode.NoContent:
                    return new OkResult();
                case HttpStatusCode.BadRequest:
                    return new BadRequestObjectResult(result.Message);
                case HttpStatusCode.Conflict:
                    return new ConflictObjectResult(result.Message);
                default:
                    return new InternalServerErrorResult();
            }
        }

        [FunctionName("GetUser")]
        public async Task<IActionResult> GetUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "users/{email}")] HttpRequest req,
            [FromRoute] string email,
            ILogger log)
        {
            var user = await _userService.GetUser(email);

            if (user == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(user);
        }

        [FunctionName("ListUsers")]
        public async Task<IActionResult> ListUsers(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "users")]
            HttpRequest req,
            ILogger log)
        {
            var userProfiles = await _userService.GetAllUsers();

            return new OkObjectResult(userProfiles);
        }
    }
}
