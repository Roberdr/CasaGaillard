using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CasaGaillard.Core.Models;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class DecimalRangeAttribute : ValidationAttribute, IClientModelValidator
{
    private readonly decimal _minimum;
    private readonly decimal _maximum;

    public DecimalRangeAttribute(string minimum, string maximum)
    {
        _minimum = decimal.Parse(minimum, NumberStyles.Number, CultureInfo.InvariantCulture);
        _maximum = decimal.Parse(maximum, NumberStyles.Number, CultureInfo.InvariantCulture);
        ErrorMessage = "El valor debe estar entre {0} y {1}.";
    }

    public override bool IsValid(object? value)
    {
        if (value is null) return true;
        if (!decimal.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), NumberStyles.Number, CultureInfo.InvariantCulture, out var number))
            return false;
        return number >= _minimum && number <= _maximum;
    }

    public override string FormatErrorMessage(string name) => string.Format(
        CultureInfo.CurrentCulture, ErrorMessageString, _minimum, _maximum);

    public void AddValidation(ClientModelValidationContext context)
    {
        Merge(context.Attributes, "data-val", "true");
        Merge(context.Attributes, "data-val-range", FormatErrorMessage(context.ModelMetadata.GetDisplayName()));
        Merge(context.Attributes, "data-val-range-min", _minimum.ToString(CultureInfo.InvariantCulture));
        Merge(context.Attributes, "data-val-range-max", _maximum.ToString(CultureInfo.InvariantCulture));
    }

    private static void Merge(IDictionary<string, string> attributes, string key, string value)
    {
        if (!attributes.ContainsKey(key)) attributes.Add(key, value);
    }
}
