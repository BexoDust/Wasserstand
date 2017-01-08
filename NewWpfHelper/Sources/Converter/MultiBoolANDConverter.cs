using System;
using System.Windows.Data;

namespace NGMP.WPF
{
    public class MultiBoolANDConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool result = values.Length > 0;

            foreach (object boolean in values)
            {
                bool temp; 
                bool.TryParse(boolean.ToString(), out temp);

                result &= temp;
            }

            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {

            throw new NotSupportedException("ConvertBack should never be called");

        }

    }
}
