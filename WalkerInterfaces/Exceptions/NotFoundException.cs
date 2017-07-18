using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WalkerInterfaces.Exceptions
{
    /// <inheritdoc />
    public class NotFoundException : BusinessApplicationWalkerException
    {
        /// <summary>
        /// nie znaleziony rodzaj elementu, ew. zakres w którym jest poszukiwany
        /// </summary>
        public string Element { get; private set; }

        /// <summary>
        /// nazwa nie znalezionego pola/elementu
        /// </summary>
        public string FieldName { get; private set; }

        /// <summary>
        /// tworzy wyjątek informujący o nieznalezieniu elementu
        /// </summary>
        public NotFoundException(string element, string fieldName, Exception innerException = null)
            : base(string.Format("{0} '{1}' not found!", element, fieldName), innerException)
        {
            Element = element;
            FieldName = fieldName;
        }
    }
}
