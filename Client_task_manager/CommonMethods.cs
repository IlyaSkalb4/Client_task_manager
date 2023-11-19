using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace Client_task_manager
{
    public class CommonMethods //Клас, який зберігає методи для використання всіма класами.
    {
        public static void ShowErrorMessage(string message) //Метод, який виводить помилку.
        {
            MessageBox.Show(message, Constants.Error, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static bool IsPassword(string password) //Метод, який перевіряє пароль по патерну.
        {
            return Regex.IsMatch(password, Constants.PasswordPattern);
        }

        public static bool IsEmail(string email) //Метод, який перевіряє електрону пошту по патерну.
        {
            return Regex.IsMatch(email, Constants.EmailPattern);
        }
    }
}
