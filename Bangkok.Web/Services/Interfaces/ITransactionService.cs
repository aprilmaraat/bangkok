using System.Collections.Generic;
using System.Threading.Tasks;
using Bangkok.Models;
using Bangkok.Web.Models;

namespace Bangkok.Web.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<Response<IEnumerable<TransactionData>>> GetAllTransactions(RequestOptions options);
    }
}
