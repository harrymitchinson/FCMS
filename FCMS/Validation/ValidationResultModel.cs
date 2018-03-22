using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FCMS.Validation
{
    public class ValidationError
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public String Property { get; set; }

        public String Message { get; set; }

        public ValidationError(String property, String message)
        {
            this.Property = String.IsNullOrEmpty(property) ? null : property;
            this.Message = message;
        }
    }

    public class ValidationResultModel
    {
        public String Message { get; set; }

        public List<ValidationError> Errors { get; set; }

        public ValidationResultModel()
        {

        }

        public ValidationResultModel(ModelStateDictionary modelState)
        {
            this.Message = "Model validation failed";
            this.Errors = modelState.Keys
                .SelectMany(key => modelState[key].Errors.Select(x => new ValidationError(key, x.ErrorMessage)))
                .ToList();
        }
    }
}
