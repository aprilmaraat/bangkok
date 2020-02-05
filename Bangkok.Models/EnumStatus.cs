using System;

namespace Bangkok.Models
{
    public enum Status : byte
    {
        /// <summary>
        /// CSV: Approved; XML: Approved;
        /// </summary>
        A = 1,
        /// <summary>
        /// CSV: Failed; XML: Rejected;
        /// </summary>
        R = 2,
        /// <summary>
        /// CSV: Finished; XML: Done;
        /// </summary>
        D = 3,
    }
    public class EnumStatus
    {
        public Status ID { get; set; }
        public char StatusInitial { get; set; }
    }
}
