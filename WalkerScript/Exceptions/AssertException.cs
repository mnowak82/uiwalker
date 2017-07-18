using WalkerInterfaces.Exceptions;

namespace WalkerScript.Exceptions
{
    public class AssertException : BusinessApplicationWalkerException
    {
        public string Value { get; private set; }
        public string SecondValue { get; private set; }

        public AssertException(string value, string operation, string secondValue)
            : base(string.Format("Assert failed: value '{0}' {1} '{2}'", value, operation, secondValue), null)
        {
            Value = value;
            SecondValue = secondValue;
        }
    }
}
