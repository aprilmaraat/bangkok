using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Net.Http.Headers;
using System.IO;
using Bangkok.Web.Models;

namespace Bangkok.Web.Utilities
{
    public static class FileHelper
    {
        private static readonly string[] _permittedExtensions = { ".xml", ".csv" };

        public static Response<byte[]> ProcessStreamedFile(ContentDispositionHeaderValue contentDisposition)
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    var isValid = IsValidFileExtension(
                        contentDisposition.FileName.Value, memoryStream);

                    if(isValid.State == ResponseState.Success)
                        return new Response<byte[]>
                        {
                            State = ResponseState.Success,
                            Message = ResponseMessage.Success,
                            ErrorText = null,
                            Exception = null,
                            ResponseObject = memoryStream.ToArray()
                        };

                    return new Response<byte[]>
                    {
                        State = ResponseState.Error,
                        Message = ResponseMessage.MiscError,
                        ErrorText = "Unknown format",
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

        public static Response IsValidFileExtension(string fileName, Stream data)
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
