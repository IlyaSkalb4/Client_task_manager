using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using Classes_for_transferring_users;

namespace Client_task_manager
{
    public partial class SignInWindow : Window
    {
        private NetworkManager networkManager = null;

        private UserLogin userLogin = null;

        public SignInWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ServerSide.Start();

            networkManager = new NetworkManager();
        }

        private void logInButton_Click(object sender, RoutedEventArgs e)
        {
            ReadyPackage sendPackage;

            string email = emailTextBox.Text;
            string password = passwordTextBox.Text;

            bool flagCheck = false;

            emailWarningTextBlock.Text = "";
            passwordWarningTextBlock.Text = "";

            if (CommonMethods.IsLineEmpty(email))
            {
                emailWarningTextBlock.Text = Constants.EnterEmail;
                flagCheck = true;
            }
            else if (!CommonMethods.IsEmail(email))
            {
                emailWarningTextBlock.Text = Constants.IncorrectEmail;
                flagCheck = true;
            }

            if (CommonMethods.IsLineEmpty(password))
            {
                passwordWarningTextBlock.Text = Constants.EnterPassword;
                flagCheck = true;
            }
            else if(!CommonMethods.IsPassword(password))
            {
                passwordWarningTextBlock.Text = Constants.IncorrectPassword;
                flagCheck = true;
            }

            if(flagCheck)
            { 
                return;
            }

            userLogin = new UserLogin { UserEmail = email, UserPassword = password };

            sendPackage = new ReadyPackage { ObjType = Constants.Login, Data = userLogin };

            SendAndReceivePackageAsync(sendPackage);
        }

        private void signUpButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();

            SignUpWindow signUpWindow = new SignUpWindow();
            signUpWindow.ShowDialog();

            Show();
        }

        private async void SendAndReceivePackageAsync(ReadyPackage readyPackage)
        {
            logInButton.IsEnabled = false;
            emailTextBox.IsEnabled = false;
            passwordTextBox.IsEnabled = false;
            signUpButton.IsEnabled = false;

            if (await networkManager.SendAndReceivePackageAsync(readyPackage))
            {
                if (networkManager.UserTasks != null)
                {
                    Hide();

                    MainWindow mainWindow = new MainWindow();
                    mainWindow.UserTasks = networkManager.UserTasks;
                    mainWindow.MainUserLogin = userLogin;
                    mainWindow.ShowDialog();

                    Close();
                }
            }
            else
            {
                string errorType = networkManager.ErrorType;

                if (errorType == Constants.Email)
                {
                    emailWarningTextBlock.Text = Constants.IncorrectEmailSignIn;
                }
                else if (errorType == Constants.Password)
                {
                    passwordWarningTextBlock.Text = Constants.IncorrectPasswordSignIn;
                }
                else
                {
                    CommonMethods.ShowErrorMessage(networkManager.ErrorMessage);
                }
            }

            logInButton.IsEnabled = true;
            emailTextBox.IsEnabled = true;
            passwordTextBox.IsEnabled = true;
            signUpButton.IsEnabled = true;
        }
    }
}
