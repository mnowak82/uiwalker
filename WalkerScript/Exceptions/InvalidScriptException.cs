using System;
using System.Runtime.CompilerServices;

namespace WalkerScript.Exceptions
{
    public class InvalidScriptException : Exception
    {
        public InvalidScriptException(string message, params object[] args)
            : this(null, message, args)
        {
        }

        public InvalidScriptException(Exception inner, string message, params object[] args)
            : base(args.Length > 0 ? string.Format(message, args) : message, inner)
        {
        }
    }
}