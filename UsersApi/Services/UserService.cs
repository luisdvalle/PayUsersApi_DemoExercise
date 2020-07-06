using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using UsersApi.Models;
using UsersApi.Storage;

namespace UsersApi.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository _repository;

        public UserService(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<User> GetUser(string email)
        {
            var usersTableEntity = await _repository.GetEntityAsync<UsersTableEntity>(StorageTablesNames.Users, email,
                UsersApiConstants.UsersTableEntityPartitionKey);

            if (usersTableEntity == null)
            {
                return null;
            }

            return new User
            {
                Email = usersTableEntity.Email,
                Name = usersTableEntity.Name,
                MonthlySalary = usersTableEntity.MonthlySalary,
                MonthlyExpenses = usersTableEntity.MonthlyExpenses
            };
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            var tableEntities = await _repository.GetAllEntitiesAsync<UsersTableEntity>(StorageTablesNames.Users);

            return tableEntities.Select(entity => new User
            {
                Email = entity.Email, Name = entity.Name, MonthlySalary = entity.MonthlySalary,
                MonthlyExpenses = entity.MonthlyExpenses
            });
        }

        public async Task<Result> CreateUser(User user)
        {
            if (!ValidateUser(user))
            {
                return new Result
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Unable to process User, one or more properties did not meet the requirements"
                };
            }

            var usersTableEntity = await _repository.GetEntityAsync<UsersTableEntity>(StorageTablesNames.Users,
                user.Email, UsersApiConstants.UsersTableEntityPartitionKey);

            if (usersTableEntity != null)
            {
                return new Result
                {
                    StatusCode = HttpStatusCode.Conflict,
                    Message = "This email account is already being used by another user"
                };
            }
            
            usersTableEntity = new UsersTableEntity(user.Email)
            {
                Name = user.Name, MonthlySalary = user.MonthlySalary,
                MonthlyExpenses = user.MonthlyExpenses
            };
            var result = await _repository.InsertOrReplaceEntityAsync(StorageTablesNames.Users, usersTableEntity);

            return new Result
            {
                StatusCode = (HttpStatusCode) result
            };
        }

        private bool ValidateUser(User user)
        {
            if (string.IsNullOrWhiteSpace(user.Name) || string.IsNullOrWhiteSpace(user.Email) ||
                user.MonthlySalary <= 0 || user.MonthlyExpenses <= 0)
            {
                return false;
            }

            return true;
        }
    }
}
