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
    /// <summary>
    /// Interaction logic for SignUpWindow.xaml
    /// </summary>
    public partial class SignUpWindow : Window
    {
        private NetworkManager networkManager = null;

        public SignUpWindow()
        {
            InitializeComponent();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            networkManager = new NetworkManager();
        }

        private void singUpButton_Click(object sender, RoutedEventArgs e)
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
            string password = passwordTextBox.Text;
            string passwordRepeat = passwordRepeatTextBox.Text;

            if (firstName == "" || firstName == Constants.FirstName)
            {
                firstNameWarningTextBlock.Text = Constants.MustBeFilled;
                flagCheck = true;
            }

            if (lastName == "" || lastName == Constants.LastName)
            {
                lastNameWarningTextBlock.Text = Constants.MustBeFilled;
                flagCheck = true;
            }

            if (email == "" || email == Constants.Email)
            {
                emailWarningTextBlock.Text = Constants.MustBeFilled;
                flagCheck = true;
            }
            else if (!CommonMethods.IsEmail(email))
            {
                emailWarningTextBlock.Text = Constants.IncorrectEmail;
                flagCheck = true;
            }

            if (password == "" || password == Constants.Password)
            {
                passwordWarningTextBlock.Text = Constants.MustBeFilled;
                flagCheck = true;
            }
            else if (!CommonMethods.IsPassword(password))
            {
                passwordWarningTextBlock.Text = Constants.IncorrectPassword;
                flagCheck = true;
            }

            if (passwordRepeat == "" || passwordRepeat == Constants.RepeatPassword)
            {
                passwordRepeatWarningTextBlock.Text = Constants.MustBeFilled;
                flagCheck = true;
            }
            else if (password != passwordRepeat)
            {
                passwordRepeatWarningTextBlock.Text = Constants.PasswordNotMatch;
                flagCheck = true;
            }

            if (flagCheck)
            {
                return;
            }

            NewUser newUser = new NewUser {FirstName = firstName, LastName = lastName, UserEmail = email, UserPassword = password };

            SendPackageAsync(new ReadyPackage { ObjType = Constants.Registration, Data = newUser });
        }

        private async void SendPackageAsync(ReadyPackage readyPackage)
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

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            string textBoxName = textBox.Name;
            string newText = CheckTextBoxNames(textBoxName);
            
            if(newText == "")
            {
                return;
            }
            
            if(textBox.Text == newText)
            {
                textBox.Text = "";
                textBox.Foreground = Brushes.Black;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
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

        private string CheckTextBoxNames(string textBoxName)
        {
            if (textBoxName == "firstNameTextBox")
            {
                return Constants.FirstName;
            }
            else if (textBoxName == "lastNameTextBox")
            {
                return Constants.LastName;
            }
            else if (textBoxName == "emailTextBox")
            {
                return Constants.Email;
            }
            else if (textBoxName == "passwordTextBox")
            {
                return Constants.Password;
            }
            else if (textBoxName == "passwordRepeatTextBox")
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
