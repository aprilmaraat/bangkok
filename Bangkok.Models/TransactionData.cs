using System;
using Newtonsoft.Json;

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
        [JsonIgnore]
        public virtual EnumStatus EnumStatus { get; set; }
    }
}
