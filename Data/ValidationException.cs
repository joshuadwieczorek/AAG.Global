using System;
using System.Collections.Generic;

namespace AAG.Global.Data
{
    public class ValidationException : Exception
    {
        public List<ValidationError> ValidationErrors { get; set; } = new List<ValidationError>();

        public ValidationException(string message) : base(message) { }

        public ValidationException(List<ValidationError> validationErrors)
        {
            ValidationErrors = validationErrors;
        }
    }
}