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
using System.Net.Http.Headers;
using TinyCsvParser.TypeConverter;

namespace Bangkok.Web.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly BangkokContext _bangkokContext;
        private static readonly string[] _permittedExtensions = { ".xml", ".csv" };

        public TransactionService(BangkokContext bangkokContext)
        {
            _bangkokContext = bangkokContext;
        }

        /// <summary>
        /// The generic service in getting all transactions based on condition
        /// </summary>
        /// <param name="options">RequestOptions object. Contains option type and value based on option type</param>
        /// <returns></returns>
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
        /// <summary>
        /// Service method landing point for upload file requests
        /// </summary>
        /// <param name="formFile">File uploaded by client</param>
        /// <returns></returns>
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
        /// <summary>
        /// Save data from CSV file
        /// </summary>
        /// <param name="filePath">Path to CSV file</param>
        /// <returns></returns>
        private async Task<Response> SaveCsvData(string filePath)
        {
            List<TransactionData> transactionList = new List<TransactionData>();
            try
            {
                bool fileDataIsValid = true;
                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var records = csv.GetRecords<TransactionCsv>();
                    int index = 0;
                    foreach (var record in records)
                    {
                        bool recordHasNull = false;
                        if (NullOrWhiteSpace(record.ID) 
                            || NullOrWhiteSpace(record.Amount) 
                            || NullOrWhiteSpace(record.CurrencyCode) 
                            || NullOrWhiteSpace(record.TransactionDT)
                            || NullOrWhiteSpace(record.Status))
                        {
                            var description = $@"Data in index [{index}] with {{ID: {record.ID}, Amount: {record.Amount}
, CurrencyCode: {record.CurrencyCode}, TransactionDT: {record.TransactionDT}
, Status: {record.Status}}} has invalid data(s).";
                            _bangkokContext.LogData.Add(new LogData { ErrorType = "Bad Data", Description = description});
                            recordHasNull = true;
                        }

                        if (!recordHasNull)
                        {
                            var transaction = new TransactionData
                            {
                                ID = record.ID,
                                Amount = Convert.ToDecimal(record.Amount.Replace(",", "").Trim()),
                                CurrencyCode = record.CurrencyCode,
                                TransactionDT = DateTime.ParseExact(record.TransactionDT, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                                Status = ConvertStatusValue(record.Status),
                            };

                            transactionList.Add(transaction);
                        }
                            
                        index++;
                    }
                }
                File.Delete(filePath);
                if (fileDataIsValid)
                {
                    await _bangkokContext.AddRangeAsync(transactionList);
                    await _bangkokContext.SaveChangesAsync();

                    return new Response
                    {
                        State = ResponseState.Success,
                        Message = ResponseMessage.Success,
                        ErrorText = null,
                        Exception = null
                    };
                }
                else
                    return new Response
                    {
                        State = ResponseState.Error,
                        Message = ResponseMessage.MiscError,
                        ErrorText = "File has invalid data in it.",
                        Exception = null
                    };
            }
            catch (Exception ex) 
            {
                File.Delete(filePath);

                return new Response
                {
                    State = ResponseState.Exception,
                    Message = ResponseMessage.Exception,
                    ErrorText = null,
                    Exception = ex
                };
            }
        }
        /// <summary>
        /// Save data from XML file
        /// </summary>
        /// <param name="filePath">Path to XML file</param>
        /// <returns></returns>
        private async Task<Response> SaveXmlData(string filePath)
        {
            return new Response
            {
                State = ResponseState.Error,
                Message = ResponseMessage.MiscError,
                ErrorText = "",
                Exception = null
            };
        }
        /// <summary>
        /// Helper method to check if value is null, empty, or only contains white spaces
        /// </summary>
        /// <param name="property">Value to be checked</param>
        /// <returns></returns>
        private bool NullOrWhiteSpace(string property) 
        {
            if (string.IsNullOrEmpty(property) || string.IsNullOrWhiteSpace(property))
                return true;
            return false;
        }
        private Status ConvertStatusValue(string status) 
        {
            var lowerStatus = status.ToLower();
            if (lowerStatus == "approved")
                return Status.A;
            else if (lowerStatus == "finished" || lowerStatus == "done")
                return Status.D;
            else
                return Status.R;
        }
        /// <summary>
        /// File landing point to process and check file validity
        /// </summary>
        /// <param name="formFile">File sent by client</param>
        /// <returns></returns>
        private async Task<Response> ProcessFormFile(IFormFile formFile)
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

                return new Response
                {
                    State = ResponseState.Error,
                    Message = ResponseMessage.MiscError,
                    ErrorText = error,
                    Exception = null
                };
            }

            try
            {
                var folderName = "Temp";
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                var fileName = ContentDispositionHeaderValue.Parse(formFile.ContentDisposition).FileName.Trim('"');
                var fullPath = Path.Combine(pathToSave, fileName);
                var dbPath = Path.Combine(folderName, fileName);
                string fileExtension = string.Empty;

                using (var memoryStream = new FileStream(fullPath, FileMode.Create))
                {
                    await formFile.CopyToAsync(memoryStream);

                    // Check the content length in case the file's only
                    // content was a BOM and the content is actually
                    // empty after removing the BOM.
                    if (memoryStream.Length == 0)
                    {
                        return new Response
                        {
                            State = ResponseState.Error,
                            Message = ResponseMessage.MiscError,
                            ErrorText = $"Unknown format: {formFile.Name} ({trustedFileNameForDisplay}) is empty.",
                            Exception = null
                        };
                    }

                    var isValid = IsValidFileExtension(
                        formFile.FileName, memoryStream);

                    if (!(isValid.State == ResponseState.Success))
                    {
                        return new Response
                        {
                            State = isValid.State,
                            Message = isValid.Message,
                            ErrorText = $@"Unknown format: 
                                {formFile.Name} ({trustedFileNameForDisplay}) file type isn't permitted.",
                            Exception = null
                        };
                    }

                    fileExtension = isValid.ResponseObject;
                }

                switch (fileExtension)
                {
                    case ".csv":
                        return await SaveCsvData(fullPath);
                    case ".xml":
                        return await SaveXmlData(fullPath);
                    default:
                        return new Response
                        {
                            State = ResponseState.Error,
                            Message = ResponseMessage.MiscError,
                            ErrorText = $@"Unknown format: {formFile.Name} ({trustedFileNameForDisplay}) file type isn't permitted.",
                            Exception = null
                        };
                }
            }
            catch (Exception ex)
            {
                return new Response
                {
                    State = ResponseState.Exception,
                    Message = ResponseMessage.Exception,
                    ErrorText = null,
                    Exception = ex
                };
            }
        }
        /// <summary>
        /// Helper method to check if file has valid file extension
        /// </summary>
        /// <param name="fileName">Filename of file sent by client</param>
        /// <param name="data">File data from file sent by client</param>
        /// <returns></returns>
        private static Response<string> IsValidFileExtension(string fileName, Stream data)
        {
            if (string.IsNullOrEmpty(fileName) || data == null || data.Length == 0)
            {
                return new Response<string>
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
                return new Response<string>
                {
                    State = ResponseState.Error,
                    Message = ResponseMessage.MiscError,
                    ErrorText = "Unknown format",
                    Exception = null
                };
            }

            return new Response<string>
            {
                State = ResponseState.Success,
                Message = ResponseMessage.Success,
                ErrorText = null,
                Exception = null,
                ResponseObject = ext
            };
        }
    }
}
