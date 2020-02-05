using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Bangkok.Web.Models;
using Bangkok.Web.Services;

namespace Bangkok.Web.Controllers
{
    [ApiController]
    [Route("bangkok/transaction")]
    public class TransactionController : Controller
    {
        private readonly TransactionService _transactionService;

        public TransactionController(TransactionService transactionService) 
        {
            _transactionService = transactionService;
        }

        [HttpPost]
        public async Task<IActionResult> GetAll([FromBody] RequestOptions options)
        {
            var response = await _transactionService.GetAllTransactions(options);

            switch (response.State)
            {
                case ResponseState.Exception:
                    return StatusCode(500, response.Exception);
                case ResponseState.Error:
                    return BadRequest(response.MessageText);
                default:
                    return Ok(response.ResponseObject);
            }
        }
    }
}
