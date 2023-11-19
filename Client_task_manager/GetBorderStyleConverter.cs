using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace Client_task_manager
{
    public class GetBorderStyleConverter : IValueConverter //Клас для отримання стилю.
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) //Метод, який видає стиль залежно від вхідного тексту.
        {
            if (value is string text)
            {
                if (text == Constants.High)
                {
                    return (Style)Application.Current.Resources[Constants.PriorityHighBorder];
                }
                else if (text == Constants.Low)
                {
                    return (Style)Application.Current.Resources[Constants.PriorityLowBorder];
                }
                else if (text == Constants.Normal)
                {
                    return (Style)Application.Current.Resources[Constants.PriorityNormalBorder];
                }
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) //Метод для викидання винятку.
        {
            throw new NotImplementedException();
        }
    }
}
