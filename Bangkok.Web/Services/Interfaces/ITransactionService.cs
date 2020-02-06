using System.Collections.Generic;
using System.Threading.Tasks;
using Bangkok.Models;
using Bangkok.Web.Models;
using Microsoft.AspNetCore.Http;

namespace Bangkok.Web.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<Response<IEnumerable<TransactionData>>> GetAllTransactions(RequestOptions options);
        Task<Response> SaveTransactionData(IFormFile file);
    }
}
