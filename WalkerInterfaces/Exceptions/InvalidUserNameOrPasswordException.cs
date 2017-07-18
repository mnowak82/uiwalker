using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WalkerInterfaces.Exceptions
{
    /// <inheritdoc />
    public class InvalidUserNameOrPasswordException : BusinessApplicationWalkerException
    {
        /// <summary>
        /// użyta nazwa użytkownika
        /// </summary>
        public string UserName { get; private set; }

        /// <summary>
        /// użyte hasło
        /// </summary>
        public string Password { get; private set; }

        /// <summary>
        /// Wyjątek informujący o błędnych danych logowania
        /// </summary>
        public InvalidUserNameOrPasswordException(string userName, string password)
            : base(string.Format("Invalid user name '{0}' or password '{1}'!", userName, password), null)
        {
            UserName = userName;
            Password = password;
        }
    }
}
