using System;
using System.Collections.Generic;

namespace AAG.Global.Security
{
    public class AuthorizationException : Exception
    {
        public AuthorizationException() : base(string.Empty) { }

        public AuthorizationException(string message, Dictionary<object, object> data = null) : base(message)
        {
            if (data is null) return;
            foreach (var (key, value) in data)
                this.Data.Add(key, value);
        }
    }
}