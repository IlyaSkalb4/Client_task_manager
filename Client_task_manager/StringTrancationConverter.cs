using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Client_task_manager
{
    public class StringTruncationConverter : IValueConverter
    {
        public static int NumberOfSymbols { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string text)
            {
                return text.Length > NumberOfSymbols ? text.Substring(0, NumberOfSymbols) + "..." : text;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class GetBorderStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string text)
            {
                if (text == "High")
                {
                    return (Style)Application.Current.Resources["priorityHighBorder"];
                }
                else if (text == "Low")
                {
                    return (Style)Application.Current.Resources["priorityLowBorder"];
                }
                else if (text == "Normal")
                {
                    return (Style)Application.Current.Resources["priorityNormalBorder"];
                }
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
