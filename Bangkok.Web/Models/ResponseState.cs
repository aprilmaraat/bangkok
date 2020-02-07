using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bangkok.Web.Models
{
    /// <summary>
    /// The state of the Response
    /// </summary>
    public enum ResponseState
    {
        /// <summary>
        /// Success response state
        /// </summary>
        Success,
        /// <summary>
        /// Error response state
        /// </summary>
        Error,
        /// <summary>
        /// Exception response state
        /// </summary>
        Exception
    }
}
