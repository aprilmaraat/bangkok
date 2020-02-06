using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bangkok.EF;
using Bangkok.Models;
using Microsoft.EntityFrameworkCore;
using Bangkok.Web.Models;
using Bangkok.Web.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.Extensions.Logging;
using Bangkok.Web.Utilities;
using Microsoft.Net.Http.Headers;

namespace Bangkok.Web.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly BangkokContext _bangkokContext;
        private readonly ILogger<TransactionService> _logger;

        public TransactionService(BangkokContext bangkokContext)
        {
            _bangkokContext = bangkokContext;
        }

        public async Task<Response<IEnumerable<TransactionData>>> GetAllTransactions(RequestOptions options)
        {
            try
            {
                var data = new List<TransactionData>();
                switch (options.OptionType)
                {
                    case OptionType.ByCurrency:
                        data = await _bangkokContext.TransactionData
                            .Where(t => t.CurrencyCode == options.CurrencyCode)
                            .ToListAsync();
                        break;
                    case OptionType.ByDateRange:
                        data = await _bangkokContext.TransactionData
                            .Where(t => t.TransactionDT >= options.FromDT && t.TransactionDT <= options.ToDT)
                            .ToListAsync();
                        break;
                    case OptionType.ByStatus:
                        data = await _bangkokContext.TransactionData
                            .Where(t => t.Status == options.Status)
                            .ToListAsync();
                        break;
                    default:
                        data = await _bangkokContext.TransactionData
                            .ToListAsync();
                        break;
                }
                return Response<IEnumerable<TransactionData>>.Success(data);
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<TransactionData>>.Error(ex);
            }
        }

        public async Task<Response> SaveTransactionData(IFormFile file) 
        {
            ContentDispositionHeaderValue.TryParse(
                        file.ContentDisposition, out var contentDisposition);

            var fileStream = FileHelper.ProcessStreamedFile(contentDisposition);

            if (fileStream.State == ResponseState.Success)
            {

                return new Response
                {
                    State = ResponseState.Success,
                    Message = ResponseMessage.Success,
                    ErrorText = null,
                    Exception = null
                };
            }

            return new Response
            {
                State = fileStream.State,
                Message = fileStream.Message,
                ErrorText = fileStream.ErrorText,
                Exception = fileStream.Exception
            };
        }
    }
}
