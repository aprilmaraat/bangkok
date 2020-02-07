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
using System.Net;
using System.Data;
using CsvHelper;
using System.Globalization;
using TinyCsvParser;
using System.Text;

namespace Bangkok.Web.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly BangkokContext _bangkokContext;
        private readonly ILogger<TransactionService> _logger;
        private static readonly string[] _permittedExtensions = { ".xml", ".csv" };

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

        public async Task<Response> SaveTransactionDataFromFile(IFormFile formFile)
        {
            var streamedFileContent = await ProcessFormFile(formFile);

            if (streamedFileContent.State == ResponseState.Success)
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
                State = streamedFileContent.State,
                Message = streamedFileContent.Message,
                ErrorText = streamedFileContent.ErrorText,
                Exception = streamedFileContent.Exception
            };
        }

        private static async Task<Response<byte[]>> ProcessFormFile(IFormFile formFile)
        {
            // Don't trust the file name sent by the client. To display
            // the file name, HTML-encode the value.
            var trustedFileNameForDisplay = WebUtility.HtmlEncode(
                formFile.FileName);

            // Check the file length. This check doesn't catch files that only have 
            // a BOM as their content.
            if (formFile.Length == 0)
            {
                var error = $"Unknown format: {formFile.Name} ({trustedFileNameForDisplay}) is empty.";

                return new Response<byte[]>
                {
                    State = ResponseState.Error,
                    Message = ResponseMessage.MiscError,
                    ErrorText = error,
                    Exception = null
                };
            }

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await formFile.CopyToAsync(memoryStream);

                    // Check the content length in case the file's only
                    // content was a BOM and the content is actually
                    // empty after removing the BOM.
                    if (memoryStream.Length == 0)
                    {
                        return new Response<byte[]>
                        {
                            State = ResponseState.Error,
                            Message = ResponseMessage.MiscError,
                            ErrorText = $"Unknown format: {formFile.Name} ({trustedFileNameForDisplay}) is empty.",
                            Exception = null
                        };
                    }

                    var isValid = IsValidFileExtension(
                        formFile.FileName, memoryStream);

                    if (isValid.State == ResponseState.Success) 
                    {
                        DataTable csvData = new DataTable();

                        CsvParserOptions csvParserOptions = new CsvParserOptions(true, ',');
                        var csvParser = new CsvParser<TransactionData>(csvParserOptions, new CsvMapper());
                        var records = csvParser.ReadFromFile(formFile.FileName, Encoding.UTF8);
                        var x = records.Select(x => x.Result).ToList();


                        //using (CsvReader csv = new CsvReader(new StreamReader(memoryStream), CultureInfo.InvariantCulture))
                        //{
                        //    var records = csv.GetRecords<TransactionData>();
                        //    var x = records.ToList();
                        //}
                        //return new Response<byte[]>
                        //{
                        //    State = ResponseState.Success,
                        //    Message = ResponseMessage.Success,
                        //    ErrorText = null,
                        //    Exception = null,
                        //    ResponseObject = memoryStream.ToArray()
                        //};
                    }

                    return new Response<byte[]>
                    {
                        State = isValid.State,
                        Message = isValid.Message,
                        ErrorText = $@"Unknown format: 
                            {formFile.Name} ({trustedFileNameForDisplay}) file type isn't permitted.",
                        Exception = null
                    };
                }
            }
            catch (Exception ex)
            {
                return new Response<byte[]>
                {
                    State = ResponseState.Exception,
                    Message = ResponseMessage.Exception,
                    ErrorText = null,
                    Exception = ex
                };
            }
        }

        private static Response IsValidFileExtension(string fileName, Stream data)
        {
            if (string.IsNullOrEmpty(fileName) || data == null || data.Length == 0)
            {
                return new Response
                {
                    State = ResponseState.Error,
                    Message = ResponseMessage.MiscError,
                    ErrorText = "Unknown format",
                    Exception = null
                };
            }

            var ext = Path.GetExtension(fileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(ext) || !_permittedExtensions.Contains(ext))
            {
                return new Response
                {
                    State = ResponseState.Error,
                    Message = ResponseMessage.MiscError,
                    ErrorText = "Unknown format",
                    Exception = null
                };
            }

            return new Response
            {
                State = ResponseState.Success,
                Message = ResponseMessage.Success,
                ErrorText = null,
                Exception = null
            };
        }
    }
}
