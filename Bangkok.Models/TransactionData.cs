using System;
using System.Collections.Generic;
using System.Text;

namespace Bangkok.Models
{
    public class TransactionData
    {
        public string ID { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
        public DateTime TransactionDT { get; set; }
        public Status Status { get; set; }
        /// <summary>
        /// Navigation property to EnumStatus
        /// </summary>
        public virtual EnumStatus EnumStatus { get; set; }
    }
}
