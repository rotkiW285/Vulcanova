using System.Collections.Generic;
using System.Threading.Tasks;
using Vulcanova.Features.Auth.Accounts;

namespace Vulcanova.Features.Auth
{
    public interface IAccountRepository
    {
        Task AddAccountsAsync(IEnumerable<Account> accounts);
        Task<Account> GetActiveAccountAsync();
        Task<IReadOnlyCollection<Account>> GetAccountsAsync();
        Task<Account> GetByIdAsync(int id);
        Task UpdateAccountAsync(Account account);
        Task UpdateAccountsAsync(IEnumerable<Account> accounts);
        Task DeleteByIdAsync(int id);
    }
}