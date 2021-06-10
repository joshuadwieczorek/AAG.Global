using System;

namespace AAG.Global.Security
{
    public sealed class CryptographyException : Exception
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public CryptographyException(String message, Exception? innerException) : base(message, innerException) { }
    }
}