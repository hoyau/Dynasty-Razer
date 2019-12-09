using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace DynastyRazer.Helpers
{
    public class NandConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var relevantValues = new List<bool>();

            foreach (var value in values)
            {
                if (value is bool)
                    relevantValues.Add((bool)value);
            }

            return !relevantValues.Any(value => value);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
