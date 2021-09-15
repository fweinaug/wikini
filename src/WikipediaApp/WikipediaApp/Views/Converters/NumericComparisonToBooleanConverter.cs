using System;
using Windows.UI.Xaml.Data;

namespace WikipediaApp
{
  public enum NumericComparisonType
  {
    EqualTo = 0,
    GreaterThan,
    GreaterThanOrEqualTo,
    LowerThan,
    LowerThanOrEqualTo,
  }

  public class NumericComparisonToBooleanConverter : IValueConverter
  {
    public int ComparisonValue { get; set; }

    public NumericComparisonType ComparisonType { get; set; }

    public object Convert(object value, Type targetType, object parameter, string language)
    {
      if (value is int intValue)
      {
        switch (ComparisonType)
        {
          case NumericComparisonType.EqualTo:
            return intValue == ComparisonValue;
          case NumericComparisonType.GreaterThan:
            return intValue > ComparisonValue;
          case NumericComparisonType.GreaterThanOrEqualTo:
            return intValue >= ComparisonValue;
          case NumericComparisonType.LowerThan:
            return intValue < ComparisonValue;
          case NumericComparisonType.LowerThanOrEqualTo:
            return intValue <= ComparisonValue;
        }
      }

      return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
      throw new NotImplementedException();
    }
  }
}