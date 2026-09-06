using System;
using System.Globalization;
using System.Web.Mvc;

namespace CasaGaillard.ModelBinders
{

    public class DecimalModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (value == null || string.IsNullOrEmpty(value.AttemptedValue))
                return null;

            var attempted = value.AttemptedValue.Replace(".", ",");
            decimal result;

            if (decimal.TryParse(attempted, NumberStyles.Any, new CultureInfo("es-ES"), out result))
                return result;

            bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Número inválido");
            return null;
        }
    }

    public class DecimalModelBinder1 : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueResult == null) return GetDefaultValue(bindingContext.ModelType);

            var raw = valueResult.AttemptedValue?.Trim();
            if (string.IsNullOrEmpty(raw)) return GetDefaultValue(bindingContext.ModelType);

            decimal parsed;
            // Intentar con la cultura actual (por ejemplo es-ES acepta coma)
            if (decimal.TryParse(raw, NumberStyles.Number, CultureInfo.CurrentCulture, out parsed))
                return CastToProperType(parsed, bindingContext.ModelType);

            // Reemplazar coma por punto y probar con Invariant
            var alt = raw.Replace(',', '.');
            if (decimal.TryParse(alt, NumberStyles.Number, CultureInfo.InvariantCulture, out parsed))
                return CastToProperType(parsed, bindingContext.ModelType);

            bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Número no válido");
            return GetDefaultValue(bindingContext.ModelType);
        }

        private object CastToProperType(decimal parsed, Type modelType)
        {
            if (modelType == typeof(decimal)) return parsed;
            if (modelType == typeof(decimal?)) return (decimal?)parsed;
            // Si es otro tipo numérico, intentar convertir
            try
            {
                return Convert.ChangeType(parsed, modelType, CultureInfo.InvariantCulture);
            }
            catch
            {
                return GetDefaultValue(modelType);
            }
        }

        private object GetDefaultValue(Type modelType)
        {
            if (modelType == typeof(decimal)) return default(decimal);
            return null; // para decimal? y otros
        }
    }
}
