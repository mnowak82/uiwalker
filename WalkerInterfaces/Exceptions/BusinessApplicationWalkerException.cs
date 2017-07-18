using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WalkerInterfaces.Exceptions
{
    /// <summary>
    /// wyjątki w ramach testów biznesowej aplikacji
    /// </summary>
    public abstract class BusinessApplicationWalkerException : Exception
    {
        /// <summary>
        /// konstruktor bazowy
        /// </summary>
        protected BusinessApplicationWalkerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
