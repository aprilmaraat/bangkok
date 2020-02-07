using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Bangkok.Web.Models;
using Bangkok.Web.Services.Interfaces;
using Microsoft.Extensions.Logging;

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
        /// <summary>
        /// Generic endpoint to handle what OptionType the client requests
        /// Can handle getting transactions with conditions by currency, by date range, by status, or all transactions
        /// </summary>
        /// <param name="options">RequestOptions object. Contains option type and value based on option type</param>
        /// <returns></returns>
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
        /// <summary>
        /// This endpoint handles the uploading of files and only accepts .csv and .xml files.
        /// </summary>
        /// <returns></returns>
        [Route("upload")]
        [HttpPost, DisableRequestSizeLimit]
        public async Task<IActionResult> Upload()
        {
            var response = await _transactionService.SaveTransactionDataFromFile(Request.Form.Files[0]);

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
