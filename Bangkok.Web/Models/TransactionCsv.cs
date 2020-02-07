using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bangkok.Models;
using System.ComponentModel.DataAnnotations;

namespace Bangkok.Web.Models
{
    /// <summary>
    /// Helper model for Csv data conversion
    /// Mapped to TransactionData object
    /// </summary>
    public class TransactionCsv
    {
        public string ID { get; set; }
        public string Amount { get; set; }
        public string CurrencyCode { get; set; }
        public string TransactionDT { get; set; }
        public string Status { get; set; }
    }
}
