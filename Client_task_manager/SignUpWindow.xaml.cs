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

            if (firstName == "")
            {
                firstNameWarningTextBlock.Text = Constants.MustBeFilled;
                flagCheck = true;
            }

            if (lastName == "")
            {
                lastNameWarningTextBlock.Text = Constants.MustBeFilled;
                flagCheck = true;
            }

            if (email == "")
            {
                emailWarningTextBlock.Text = Constants.MustBeFilled;
                flagCheck = true;
            }
            else if (!CommonMethods.IsEmail(email))
            {
                emailWarningTextBlock.Text = Constants.IncorrectEmail;
                flagCheck = true;
            }

            if (password == "")
            {
                passwordWarningTextBlock.Text = Constants.MustBeFilled;
                flagCheck = true;
            }
            else if (!CommonMethods.IsPassword(password))
            {
                passwordWarningTextBlock.Text = Constants.IncorrectPassword;
                flagCheck = true;
            }

            if (passwordRepeat == "")
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
    }
}
