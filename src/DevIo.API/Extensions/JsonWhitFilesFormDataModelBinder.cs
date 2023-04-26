using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace DevIo.API.Extensions
{
    public class JsonWhitFilesFormDataModelBinder : IModelBinder
    {
        private readonly IOptions<MvcNewtonsoftJsonOptions> _jsonOptions;
        private readonly FormFileModelBinder _formFileBinder;

        public JsonWhitFilesFormDataModelBinder(IOptions<MvcNewtonsoftJsonOptions> jsonOptions, ILoggerFactory loggerFactory)
        {
            _jsonOptions = jsonOptions;
            _formFileBinder = new FormFileModelBinder(loggerFactory);
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if(bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.FieldName);
            if(valueResult == ValueProviderResult.None) 
            {
                var message = bindingContext.ModelMetadata.ModelBindingMessageProvider.MissingBindRequiredValueAccessor(bindingContext.FieldName);
                bindingContext.ModelState.TryAddModelError(bindingContext.FieldName, message);
                return;
            }

            var rawValue = valueResult.FirstValue;
            var model = JsonConvert.DeserializeObject(rawValue, bindingContext.ModelType, _jsonOptions.Value.SerializerSettings);

            foreach (var property in bindingContext.ModelMetadata.Properties)
            {
                if(property.ModelType != typeof(IFormFile))
                    continue;

                var fieldName = property.BinderModelName ?? property.PropertyName;
                var modelName = fieldName;
                var propertyModel = property.PropertyGetter(bindingContext.Model);
                ModelBindingResult propertyResult;

                using(bindingContext.EnterNestedScope(property, fieldName, modelName,propertyModel))
                {
                    await _formFileBinder.BindModelAsync(bindingContext);
                    propertyResult = bindingContext.Result;
                }

                if(propertyResult.IsModelSet) 
                {
                    property.PropertySetter(model, propertyResult.Model);
                }
                else if(property.IsBindingRequired) 
                {
                    var message = property.ModelBindingMessageProvider.MissingBindRequiredValueAccessor(fieldName);
                    bindingContext.ModelState.TryAddModelError(modelName, message);
                }
            }

            bindingContext.Result = ModelBindingResult.Success(model);
        }
    }
}
