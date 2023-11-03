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
            UserLogin userLogin;

            ReadyPackage sendPackage;

            string email = emailTextBox.Text;
            string password = passwordTextBox.Text;

            bool check = false;

            emailWarningTextBlock.Text = "";
            passwordWarningTextBlock.Text = "";

            if (CommonMethods.IsLineEmpty(email))
            {
                emailWarningTextBlock.Text = Constants.EnterEmail;
                check = true;
            }
            else if (!CommonMethods.IsEmail(email))
            {
                emailWarningTextBlock.Text = Constants.IncorrectEmail;
                check = true;
            }

            if (CommonMethods.IsLineEmpty(password))
            {
                passwordWarningTextBlock.Text = Constants.EnterPassword;
                check = true;
            }
            else if(!CommonMethods.IsPassword(password))
            {
                passwordWarningTextBlock.Text = Constants.IncorrectPassword;
                check = true;
            }

            if(check)
            { 
                return;
            }

            userLogin = new UserLogin { UserEmail = email, UserPassword = password };

            sendPackage = new ReadyPackage { ObjType = Constants.Login, Data = userLogin, RepeatStatus = true };

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

            if(await networkManager.SendAndReceivePackageAsync(readyPackage))
            {
                string errorType = networkManager.ErrorType;
                string errorMessage = networkManager.ErrorMessage;

                if (!CommonMethods.IsLineEmpty(errorType))
                {
                    if (errorType == Constants.Email)
                    {
                        emailWarningTextBlock.Text = errorMessage;
                    }
                    else if (errorType == Constants.Password)
                    {
                        passwordWarningTextBlock.Text = errorMessage;
                    }
                }

                readyPackage = new ReadyPackage { ObjType = Constants.UserTask, Data = "", RepeatStatus = true };

                if (await networkManager.SendAndReceivePackageAsync(readyPackage))
                {
                    if (networkManager.UserTasks != null)
                    {
                        Hide();

                        MainWindow mainWindow = new MainWindow();
                        mainWindow.UserTasks = networkManager.UserTasks;
                        mainWindow.ShowDialog();

                        Close();
                    }
                }
            }

            logInButton.IsEnabled = true;
            emailTextBox.IsEnabled = true;
            passwordTextBox.IsEnabled = true;
            signUpButton.IsEnabled = true;
        }
    }
}
