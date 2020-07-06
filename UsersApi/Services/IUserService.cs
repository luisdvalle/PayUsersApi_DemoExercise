using System.Collections.Generic;
using System.Threading.Tasks;
using UsersApi.Models;

namespace UsersApi.Services
{
    /// <summary>
    /// Define functionality to interact with User data.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Gets a User based on email address, which is a unique field.
        /// </summary>
        /// <param name="email">The Email Address</param>
        /// <returns>Returns the User if email address is found, otherwise returns null.</returns>
        Task<User> GetUserAsync(string email);

        /// <summary>
        /// Gets all Users.
        /// </summary>
        /// <returns>An Enumerable of Users objects.</returns>
        Task<IEnumerable<User>> GetAllUsersAsync();

        /// <summary>
        /// Creates an User.
        /// </summary>
        /// <param name="user">The User to be created.</param>
        /// <returns>An UserResult object with the result of this operation.</returns>
        Task<Result> CreateUserAsync(User user);
    }
}
