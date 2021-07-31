using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace RestAPI2.Helper
{
    public class ArrayModelBinder:IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext modelBindingContext)
        {
            if (!modelBindingContext.ModelMetadata.IsEnumerableType)
            {
                modelBindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            }

            var value = modelBindingContext.ValueProvider.GetValue(modelBindingContext.ModelName).ToString();

            if (string.IsNullOrEmpty(value))
            {
                modelBindingContext.Result = ModelBindingResult.Success(null);
                return Task.CompletedTask;

            }

            var elementType = modelBindingContext.ModelType.GetType().GenericTypeArguments[0];

            var convertor = TypeDescriptor.GetConverter(elementType);

            var values = value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(x =>

                convertor.ConvertFromString(x.Trim())).ToArray();

            var typedValues = Array.CreateInstance(elementType, values.Length);
            values.CopyTo(typedValues, 0);
            modelBindingContext.Model = typedValues;

            // return a successful result, passing in the Model 
            modelBindingContext.Result = ModelBindingResult.Success(modelBindingContext.Model);
            return Task.CompletedTask;
        }
    }
}
