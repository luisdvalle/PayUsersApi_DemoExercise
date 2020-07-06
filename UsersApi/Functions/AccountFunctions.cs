using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using UsersApi.Models;
using UsersApi.Services;

namespace UsersApi.Functions
{
    public class AccountFunctions
    {
        private readonly IAccountService _accountService;

        public AccountFunctions(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [FunctionName("CreateAccount")]
        public async Task<IActionResult> CreateAccount(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "accounts")] HttpRequest req,
            ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var user = JsonConvert.DeserializeObject<User>(requestBody);

            var result = await _accountService.CreateAccountAsync(user);

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

        [FunctionName("ListAccounts")]
        public async Task<IActionResult> ListAccounts(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "accounts")]
            HttpRequest req,
            ILogger log)
        {
            var accounts = await _accountService.GetAllAccountsAsync();

            return new OkObjectResult(accounts);
        }
    }
}
