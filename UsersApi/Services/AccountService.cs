using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using UsersApi.Models;
using UsersApi.Storage;

namespace UsersApi.Services
{
    public class AccountService : IAccountService
    {
        private readonly IRepository _repository;

        public AccountService(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Account>> GetAllAccountsAsync()
        {
            var tableEntities = await _repository.GetAllEntitiesAsync<AccountsTableEntity>(StorageTablesNames.Accounts);

            return tableEntities.Select(entity => new Account
            {
                Email = entity.Email,
                Status = entity.Status
            });
        }

        public async Task<Result> CreateAccountAsync(User user)
        {
            var accountsTableEntity = await _repository.GetEntityAsync<AccountsTableEntity>(StorageTablesNames.Accounts,
                user.Email, UsersApiConstants.AccountsTableEntityPartitionKey);

            if (accountsTableEntity != null)
            {
                return new Result
                {
                    StatusCode = HttpStatusCode.Conflict,
                    Message = "An Account already exists with this email address."
                };
            }

            if (!ValidateUser(user))
            {
                return new Result
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Unable to create Account. User does not meet Salary/Expenses requirements."
                };
            }

            accountsTableEntity = new AccountsTableEntity(user.Email);
            var result = await _repository.InsertOrReplaceEntityAsync(StorageTablesNames.Accounts, accountsTableEntity);

            return new Result
            {
                StatusCode = (HttpStatusCode) result
            };
        }

        private bool ValidateUser(User user)
        {
            if (user.MonthlySalary < 1000 || user.MonthlyExpenses > 1000)
            {
                return false;
            }

            return true;
        }
    }
}
