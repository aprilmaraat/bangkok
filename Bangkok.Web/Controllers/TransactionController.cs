using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Bangkok.Web.Models;
using Bangkok.Web.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;

namespace Bangkok.Web.Controllers
{
    [ApiController]
    [Route("api/transaction")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly ILogger<TransactionController> _logger;

        public TransactionController(
            ITransactionService transactionService)
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
        [Route("upload")]
        [HttpPost, DisableRequestSizeLimit]
        public async Task<IActionResult> Upload()
        {
            var response = await _transactionService.SaveTransactionData(Request.Form.Files[0]);

            switch (response.State)
            {
                case ResponseState.Exception:
                    return StatusCode(500, response.Exception);
                case ResponseState.Error:
                    return BadRequest(response.MessageText);
                default:
                    return Ok();
            }
        }
    }
}
