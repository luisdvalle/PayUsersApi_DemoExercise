using System.Net;
using System.Threading.Tasks;
using Moq;
using UsersApi.Models;
using UsersApi.Services;
using UsersApi.Storage;
using Xunit;

namespace UsersApi.Test.Services
{
    public class UserServiceTests
    {
        private readonly IUserService _fakeUserService;

        public UserServiceTests()
        {
            var fakeRepository = new Mock<IRepository>();
                fakeRepository.Setup(m => m.GetEntityAsync<UsersTableEntity>(StorageTablesNames.Users,
                        It.Is<string>(s => s == "john@testemail.com"),
                    UsersApiConstants.UsersTableEntityPartitionKey))
                .ReturnsAsync(new UsersTableEntity("john@testemail.com"));

                fakeRepository.Setup(m => m.GetEntityAsync<UsersTableEntity>(StorageTablesNames.Users,
                        It.Is<string>(s => s == "luis@testemail.com"),
                        UsersApiConstants.UsersTableEntityPartitionKey))
                    .ReturnsAsync(() => null);

                fakeRepository.Setup(m => m.InsertOrReplaceEntityAsync(StorageTablesNames.Users,
                    It.Is<UsersTableEntity>(e => e.Email == "luis@testemail.com")))
                    .ReturnsAsync(204);

            _fakeUserService = new UserService(fakeRepository.Object);
        }

        [Theory]
        [InlineData("", "luis@testemail.com", 2000, 100)]
        [InlineData("Luis", "", 2000, 100)]
        [InlineData("Luis", "luis@testemail.com", -2000, 100)]
        [InlineData("Luis", "luis@testemail.com", 2000, -100)]
        public async Task CreateUserAsync_UserIsNotValid_ReturnsBadRequestResponse(string name, string email,
            double monthlySalary, double monthlyExpenses)
        {
            var fakeUser = new User
                { Email = email, Name = name, MonthlyExpenses = monthlyExpenses, MonthlySalary = monthlySalary };

            var mockResult = await _fakeUserService.CreateUserAsync(fakeUser);

            Assert.Equal(HttpStatusCode.BadRequest, mockResult.StatusCode);
        }

        [Fact]
        public async Task CreateUserAsync_UserAlreadyExist_ReturnsConflictResponse()
        {
            var fakeUser = new User
                { Email = "john@testemail.com", Name = "John", MonthlyExpenses = 500, MonthlySalary = 2000 };

            var mockResult = await _fakeUserService.CreateUserAsync(fakeUser);

            Assert.Equal(HttpStatusCode.Conflict, mockResult.StatusCode);
        }

        [Fact]
        public async Task CreateUserAsync_UserIsValid_Returns()
        {
            var fakeUser = new User
                { Email = "luis@testemail.com", Name = "luis", MonthlyExpenses = 500, MonthlySalary = 2000 };

            var mockResult = await _fakeUserService.CreateUserAsync(fakeUser);

            Assert.Equal(HttpStatusCode.NoContent, mockResult.StatusCode);
        }
    }
}
