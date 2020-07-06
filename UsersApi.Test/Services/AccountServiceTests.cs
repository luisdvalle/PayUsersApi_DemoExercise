using System.Net;
using System.Threading.Tasks;
using Moq;
using UsersApi.Models;
using UsersApi.Services;
using UsersApi.Storage;
using Xunit;

namespace UsersApi.Test.Services
{
    public class AccountServiceTests
    {
        private readonly IAccountService _fakeAccountService;

        public AccountServiceTests()
        {
            var fakeRepository = new Mock<IRepository>();

            fakeRepository.Setup(m => m.GetEntityAsync<AccountsTableEntity>(StorageTablesNames.Accounts,
                    It.Is<string>(s => s == "john@testemail.com"),
                    UsersApiConstants.AccountsTableEntityPartitionKey))
                .ReturnsAsync(new AccountsTableEntity("john@testemail.com"));

            fakeRepository.Setup(m => m.InsertOrReplaceEntityAsync(StorageTablesNames.Accounts,
                It.Is<AccountsTableEntity>(e => e.Email == "luis@testemail.com")))
                .ReturnsAsync(204);

            _fakeAccountService = new AccountService(fakeRepository.Object);
        }

        [Fact]
        public async Task CreateAccountAsync_AccountAlreadyExist_ReturnsConflictResponse()
        {
            var fakeUser = new User
                { Email = "john@testemail.com", Name = "John", MonthlyExpenses = 500, MonthlySalary = 2000 };

            var mockResult = await _fakeAccountService.CreateAccountAsync(fakeUser);

            Assert.Equal(HttpStatusCode.Conflict, mockResult.StatusCode);
        }

        [Theory]
        [InlineData("Luis", "luis@testemail.com", 500, 100)]
        [InlineData("Luis", "luis@testemail.com", 2000, 1500)]
        public async Task CreateAccountAsync_UserIsNotValid_ReturnsBadRequestResponse(string name, string email,
            double monthlySalary, double monthlyExpenses)
        {
            var fakeUser = new User
            { Email = email, Name = name, MonthlyExpenses = monthlyExpenses, MonthlySalary = monthlySalary };

            var mockResult = await _fakeAccountService.CreateAccountAsync(fakeUser);

            Assert.Equal(HttpStatusCode.BadRequest, mockResult.StatusCode);
        }

        [Fact]
        public async Task CreateAccountAsync_UserIsValid_Returns()
        {
            var fakeUser = new User
            { Email = "luis@testemail.com", Name = "luis", MonthlyExpenses = 500, MonthlySalary = 2000 };

            var mockResult = await _fakeAccountService.CreateAccountAsync(fakeUser);

            Assert.Equal(HttpStatusCode.NoContent, mockResult.StatusCode);
        }
    }
}
