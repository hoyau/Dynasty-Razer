using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DynastyRazer.Helpers
{
    public class NandConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            List<bool> t = new List<bool>();
            foreach(object value in values)
            {
                if (value is bool)
                    t.Add((bool)value);
            }
;
            foreach(bool r in t)
            {
                if (r)
                    return false;
            }

            return true;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
