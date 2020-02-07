using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinyCsvParser.Mapping;
using Bangkok.Models;
using TinyCsvParser.TypeConverter;

namespace Bangkok.Web.Models
{
    public class CsvMapper : CsvMapping<TransactionData>
    {
        public CsvMapper() : base()
        {
            MapProperty(0, x => x.ID);
            MapProperty(1, x => x.Amount);
            MapProperty(2, x => x.CurrencyCode);
            MapProperty(3, x => x.TransactionDT, new DateTimeConverter());
            MapProperty(4, x => x.Status, new EnumConverter<Status>());
        }
    }
}
