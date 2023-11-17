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
using System.Windows.Controls;
using System.Windows.Media;

namespace Client_task_manager
{
    public partial class SignInWindow : Window
    {
        private NetworkManager networkManager = null;

        private UserLogin userLogin = null;

        private string passwordLine;

        private int previouseLengthPasswordLine;

        public SignInWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ServerSide.Start();

            networkManager = new NetworkManager();

            passwordLine = "";
        }

        private void logInButton_Click(object sender, RoutedEventArgs e)
        {
            ReadyPackage sendPackage;

            string email = emailTextBox.Text;

            bool flagCheck = false;

            emailWarningTextBlock.Text = "";
            passwordWarningTextBlock.Text = "";

            if (CommonMethods.IsLineEmpty(email) || email == Constants.Email)
            {
                emailWarningTextBlock.Text = Constants.EnterEmail;
                flagCheck = true;
            }
            else if (!CommonMethods.IsEmail(email))
            {
                emailWarningTextBlock.Text = Constants.IncorrectEmail;
                flagCheck = true;
            }

            if (CommonMethods.IsLineEmpty(passwordLine))
            {
                passwordWarningTextBlock.Text = Constants.EnterPassword;
                flagCheck = true;
            }
            else if(!CommonMethods.IsPassword(passwordLine))
            {
                passwordWarningTextBlock.Text = Constants.IncorrectPassword;
                flagCheck = true;
            }

            if(flagCheck)
            { 
                return;
            }

            userLogin = new UserLogin { UserEmail = email, UserPassword = passwordLine };

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

                    passwordLine = "";

                    passwordTextBox.Text = Constants.Password;
                    passwordTextBox.Foreground = Brushes.Gray;

                    emailTextBox.Text = Constants.Email;
                    emailTextBox.Foreground = Brushes.Gray;

                    mainWindow.ShowDialog();

                    if(mainWindow.FlagToExit)
                    {
                        Close();
                    }

                    Show();
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

        private void emailTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if(textBox.Text == Constants.Email)
            {
                textBox.Text = "";
                textBox.Foreground = Brushes.Black;
            }
        }

        private void emailTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if(String.IsNullOrEmpty(textBox.Text))
            {
                textBox.Text = Constants.Email;
                textBox.Foreground = Brushes.Gray;
            }
        }

        private void passwordTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (textBox.Text == Constants.Password)
            {
                passwordLine = "";
                textBox.Text = "";
                textBox.Foreground = Brushes.Black;
            }
        }

        private void passwordTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (String.IsNullOrEmpty(textBox.Text))
            {
                textBox.Text = Constants.Password;
                textBox.Foreground = Brushes.Gray;
            }
        }

        private void passwordTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string textBoxText = textBox.Text;

            if (textBoxText != Constants.Password)
            {
                char[] symbolsArray = textBoxText.ToCharArray();
                int symbolsArrayLength = symbolsArray.Length;

                if (symbolsArrayLength < previouseLengthPasswordLine)
                {
                    textBox.Text = "";
                    passwordLine = "";
                    previouseLengthPasswordLine = 0;
                }
                else if (previouseLengthPasswordLine < 1)
                {
                    passwordLine = textBoxText;

                    previouseLengthPasswordLine = symbolsArrayLength;

                    textBox.Text = new string('*', symbolsArrayLength);
                }
                else
                {
                    if (symbolsArrayLength < 1)
                    {
                        passwordLine = "";
                    }
                    else
                    {
                        char lastIndexSymbolsArray = symbolsArray[symbolsArrayLength - 1];

                        if (lastIndexSymbolsArray != '*')
                        {
                            passwordLine += symbolsArray[symbolsArrayLength - 1];

                            symbolsArray[symbolsArrayLength - 1] = '*';

                            textBox.Text = new string(symbolsArray);
                        }
                    }

                    previouseLengthPasswordLine = symbolsArray.Length;
                }

                textBox.CaretIndex = textBox.Text.Length;
            }
        }
    }
}
