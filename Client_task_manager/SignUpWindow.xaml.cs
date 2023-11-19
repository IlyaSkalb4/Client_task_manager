using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Threading.Tasks;
using Classes_for_transferring_users;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Controls.Primitives;

namespace Client_task_manager
{
    public partial class SignUpWindow : Window //Клас, що реалізує вікно реєстрації нового користувача.
    {
        private NetworkManager networkManager = null;

        private PasswordHiding passwordHiding = null;

        private string passwordLine;
        private string repeatPasswordLine;

        public SignUpWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) //Метод, який виділяє пам'ять для необхідних класів під час завантаження вікна.
        {
            networkManager = new NetworkManager();

            passwordHiding = new PasswordHiding();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e) //Метод, який змінює колір шрифту при отриманні фокусу переданого текстового поля.
        {
            TextBox textBox = (TextBox)sender;

            string textBoxName = textBox.Name;
            string newText = CheckTextBoxNames(textBoxName);

            if (newText == "")
            {
                return;
            }

            if (textBox.Text == newText)
            {
                textBox.Text = "";
                textBox.Foreground = Brushes.Black;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e) //Метод, який змінює колір шрифту при втраті фокусу будь-якого переданого текстового поля.
        {
            TextBox textBox = (TextBox)sender;

            string textBoxName = textBox.Name;
            string newText = CheckTextBoxNames(textBoxName);

            if (newText == "")
            {
                return;
            }

            if (String.IsNullOrEmpty(textBox.Text))
            {
                textBox.Text = newText;
                textBox.Foreground = Brushes.Gray;
            }

        }

        private void PasswordRepeatTextBox_TextChanged(object sender, TextChangedEventArgs e) //Метод, який змінює введені символи на крапку у passwordRepeatTextBox.
        {
            TextBox textBox = (TextBox)sender;

            string textBoxText = textBox.Text;

            if (textBoxText != Constants.RepeatPassword)
            {
                passwordHiding.HidePasswordLine(textBoxText);

                repeatPasswordLine = passwordHiding.OriginalPasswordLine;

                textBox.Text = passwordHiding.Points;

                textBox.CaretIndex = textBox.Text.Length;
            }
        }

        private void PasswordTextBox_TextChanged(object sender, TextChangedEventArgs e) //Метод, який змінює введені символи на крапку у passwordTextBox.
        {
            TextBox textBox = (TextBox)sender;

            string textBoxText = textBox.Text;

            if (textBoxText != Constants.Password)
            {
                passwordHiding.HidePasswordLine(textBoxText);

                passwordLine = passwordHiding.OriginalPasswordLine;

                textBox.Text = passwordHiding.Points;

                textBox.CaretIndex = textBox.Text.Length;
            }
        }

        private void SignUpButton_Click(object sender, RoutedEventArgs e) //Метод, який під час натискання на кнопку signUpButton перевіряє поля, введені користувачем, і якщо всі перевірки пройшли успішно, запускає надсилання запиту на реєстрацію нового користувача.
        {
            bool flagCheck = false;

            firstNameWarningTextBlock.Text = "";
            lastNameWarningTextBlock.Text = "";
            emailWarningTextBlock.Text = "";
            passwordWarningTextBlock.Text = "";
            passwordRepeatWarningTextBlock.Text = "";

            string firstName = firstNameTextBox.Text;
            string lastName = lastNameTextBox.Text;
            string email = emailTextBox.Text;

            if (String.IsNullOrEmpty(firstName) || firstName == Constants.FirstName)
            {
                firstNameWarningTextBlock.Text = Constants.MustBeFilled;
                flagCheck = true;
            }

            if (String.IsNullOrEmpty(lastName) || lastName == Constants.LastName)
            {
                lastNameWarningTextBlock.Text = Constants.MustBeFilled;
                flagCheck = true;
            }

            if (String.IsNullOrEmpty(email) || email == Constants.Email)
            {
                emailWarningTextBlock.Text = Constants.MustBeFilled;
                flagCheck = true;
            }
            else if (!CommonMethods.IsEmail(email))
            {
                emailWarningTextBlock.Text = Constants.IncorrectEmail;
                flagCheck = true;
            }

            if (String.IsNullOrEmpty(passwordLine) || passwordLine == Constants.Password)
            {
                passwordWarningTextBlock.Text = Constants.MustBeFilled;
                flagCheck = true;
            }
            else if (!CommonMethods.IsPassword(passwordLine))
            {
                passwordWarningTextBlock.Text = Constants.IncorrectPassword;
                flagCheck = true;
            }

            if (String.IsNullOrEmpty(repeatPasswordLine) || repeatPasswordLine == Constants.RepeatPassword)
            {
                passwordRepeatWarningTextBlock.Text = Constants.MustBeFilled;
                flagCheck = true;
            }
            else if (passwordLine != repeatPasswordLine)
            {
                passwordRepeatWarningTextBlock.Text = Constants.PasswordNotMatch;
                flagCheck = true;
            }

            if (flagCheck)
            {
                return;
            }

            NewUser newUser = new NewUser {FirstName = firstName, LastName = lastName, UserEmail = email, UserPassword = passwordLine };

            SendPackageAsync(new ReadyPackage { ObjType = Constants.Registration, Data = newUser });
        }

        private async void SendPackageAsync(ReadyPackage readyPackage) //Метод, який надсилає запит на реєстрацію нового користувача. У разі успіху виводить повідомлення про успішне надсилання, у разі невдачі відображає помилку.
        {
            if (await Task.Run(() => networkManager.SendAndReceivePackageAsync(readyPackage)))
            {
                MessageBox.Show($"Registration request has been sent!\n{networkManager.ErrorMessage}");
            }
            else
            {
                CommonMethods.ShowErrorMessage(networkManager.ErrorMessage);
            }

            Close();
        }

        private string CheckTextBoxNames(string textBoxName) //Метод, який повертає стандартний текст поля за переданою назвою TextBox.
        {
            if (textBoxName == firstNameTextBox.Name)
            {
                return Constants.FirstName;
            }
            else if (textBoxName == lastNameTextBox.Name)
            {
                return Constants.LastName;
            }
            else if (textBoxName == emailTextBox.Name)
            {
                return Constants.Email;
            }
            else if (textBoxName == passwordTextBox.Name)
            {
                return Constants.Password;
            }
            else if (textBoxName == passwordRepeatTextBox.Name)
            {
                return Constants.RepeatPassword;
            }
            else
            {
                return "";
            }
        }
    }
}
