using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace Client_task_manager
{
    public class CommonMethods
    {
        public static void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, Constants.Error, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static bool IsLineEmpty(string line)
        {
            return line == "";
        }

        public static bool IsPassword(string password)
        {
            return Regex.IsMatch(password, Constants.PasswordPattern);
        }

        public static bool IsEmail(string email)
        {
            return Regex.IsMatch(email, Constants.EmailPattern);
        }
    }
}
