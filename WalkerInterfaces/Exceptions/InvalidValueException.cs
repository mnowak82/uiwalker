using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WalkerInterfaces.Exceptions
{
    /// <inheritdoc />
    public class InvalidFieldValueException : BusinessApplicationWalkerException
    {
        /// <summary>
        /// pole
        /// </summary>
        public string FieldName { get; private set; }

        /// <summary>
        /// nieprawidłowa wartość
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// tworzy wyjątek informujący o próbie ustawienia nieprawidłowej wartości w polu
        /// </summary>
        public InvalidFieldValueException(string fieldName, string value, Exception innerException)
            : base(string.Format("Invalid value '{0}' for field '{1}'!", value, fieldName), innerException)
        {
            FieldName = fieldName;
            Value = value;
        }
    }
}
