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
    public class StringTruncationConverter : IValueConverter //Клас для обрізання тексту.
    {
        public static int NumberOfSymbols //Кількість символів, що залишаються. 
        {
            get;
            set;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) //Метод, який обрізає текст.
        {
            if (value is string text)
            {
                return text.Length > NumberOfSymbols ? text.Substring(0, NumberOfSymbols) + Constants.Dot3X : text;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) //Метод для викидання винятку.
        {
            throw new NotImplementedException();
        }
    }
}
