using System;
using System.ComponentModel.DataAnnotations;
using ACAG.Abacus.CalendarConnector.Language;

namespace ACAG.Abacus.CalendarConnector.Models.Common
{
  [AttributeUsage(AttributeTargets.Property)]
  public class RequiredCustomLoginTypeAttribute : RequiredAttribute
  {
    private string PropertyName { get; set; }

    public RequiredCustomLoginTypeAttribute(string propertyName)
    {
      PropertyName = propertyName;
    }
    protected override ValidationResult IsValid(object value, ValidationContext context)
    {
      object instance = context.ObjectInstance;
      Type type = instance.GetType();

      var propertyValue = type.GetProperty(PropertyName).GetValue(instance);
      if (propertyValue == null)
      {
        return ValidationResult.Success;
      }

      var exchangeModel = (ExchangeLoginTypeModel) type.GetProperty(PropertyName).GetValue(instance);

      switch (exchangeModel.Type)
      {
        case ExchangeLoginEnumType.WebLogin:
          {
            if (string.IsNullOrWhiteSpace(value?.ToString()))
            {
              return new ValidationResult(ErrorMessageString, new[] { context.MemberName });
            }
            return ValidationResult.Success;
          }
        case ExchangeLoginEnumType.NetworkLogin:
          {
            return ValidationResult.Success;
          }
      }
      return ValidationResult.Success;
    }
  }
}
