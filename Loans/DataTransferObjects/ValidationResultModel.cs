using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace Loans.DataTransferObjects
{
    public class ValidationResultModel
    {
        public static readonly ValidationResultModel Success =
            new ValidationResultModel(ValidationStatus.Ok);

        public ValidationStatus Status { get; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<ValidationError> Errors { get; }

        private ValidationResultModel(ValidationStatus status)
        {
            Status = status;
        }

        public ValidationResultModel(ModelStateDictionary state) : this(ValidationStatus.Failed)
        {
            Errors = state.SelectMany(pair =>
                pair.Value.Errors.Select(error => new ValidationError(pair.Key, error.ErrorMessage))
            ).ToList();
        }
    }
}