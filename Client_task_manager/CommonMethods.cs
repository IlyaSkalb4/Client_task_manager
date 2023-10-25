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
        //private DialogResult ShowInfoMessage(string message)
        //{
        //    return MessageBox.Show(message, MyConstant.Informantion, MessageBoxButtons.OK, MessageBoxIcon.Information);
        //}

        //private DialogResult ShowWarningMessage(string message)
        //{
        //    return MessageBox.Show(message, MyConstant.Warning, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
        //}

        //private DialogResult ShowErrorMessage(string message)
        //{
        //    return MessageBox.Show(message, MyConstant.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
        //}

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
