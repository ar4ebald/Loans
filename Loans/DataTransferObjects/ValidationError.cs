using System;
using Newtonsoft.Json;

namespace Loans.DataTransferObjects
{
    public class ValidationError
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Field { get; }

        public string Message { get; }

        public ValidationError(string field, string message)
        {
            Field = String.IsNullOrEmpty(field) ? null : field;
            Message = message;
        }
    }
}