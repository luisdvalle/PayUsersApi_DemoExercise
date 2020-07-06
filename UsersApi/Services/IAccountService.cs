using System.Collections.Generic;
using System.Threading.Tasks;
using UsersApi.Models;

namespace UsersApi.Services
{
    /// <summary>
    /// Define functionality to interact with Account data.
    /// </summary>
    public interface IAccountService
    {
        /// <summary>
        /// Gets all Accounts.
        /// </summary>
        /// <returns>An Enumerable of Account objects.</returns>
        Task<IEnumerable<Account>> GetAllAccounts();

        /// <summary>
        /// Creates an Account for a specified User.
        /// </summary>
        /// <param name="user">The User.</param>
        /// <returns>An AccountResult object with the result of this operation.</returns>
        Task<Result> CreateAccount(User user);
    }
}
