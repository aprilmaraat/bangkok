using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bangkok.Models;

namespace Bangkok.Web.Models
{
    /// <summary>
    /// The option type the client requests
    /// To get transactions based on the value of this condition
    /// </summary>
    public enum OptionType : byte 
    {
        ByCurrency = 1,
        ByDateRange = 2,
        ByStatus = 3,
        All = 4
    }
    /// <summary>
    /// Request body to be sent by client and used to construct condtion for retrieving transactions
    /// </summary>
    public class RequestOptions
    {
        public OptionType OptionType { get; set; }
        public string CurrencyCode { get; set; }
        public DateTime FromDT { get; set; }
        public DateTime ToDT { get; set; }
        public Status Status { get; set; }
    }
}
