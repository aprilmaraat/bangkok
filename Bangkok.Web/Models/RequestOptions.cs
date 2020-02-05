using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bangkok.Models;

namespace Bangkok.Web.Models
{
    public enum OptionType : byte 
    {
        ByCurrency = 1,
        ByDateRange = 2,
        ByStatus = 3,
    }
    public class RequestOptions
    {
        public OptionType OptionType { get; set; }
        public string CurrencyCode { get; set; }
        public DateTime FromDT { get; set; }
        public DateTime ToDT { get; set; }
        public Status Status { get; set; }
    }
}
